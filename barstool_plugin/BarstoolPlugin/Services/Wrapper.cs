using System;
using System.Runtime.InteropServices;
using Kompas6API5;
using Kompas6Constants;
using Kompas6Constants3D;

namespace BarstoolPlugin.Services
{
    /// <summary>
    /// Обертка для работы с API КОМПАС-3D.
    /// Отвечает за подключение к КОМПАСу и низкоуровневые операции
    /// построения.
    /// </summary>
    public class Wrapper
    {
        /// <summary>
        /// Объект для работы с КОМПАС-3D.
        /// </summary>
        private KompasObject _kompas;

        /// <summary>
        /// 3D-документ.
        /// </summary>
        private ksDocument3D _doc3D;

        /// <summary>
        /// Деталь в 3D документе.
        /// </summary>
        private ksPart _part;

        /// <summary>
        /// Текущий 2D-документ эскиза.
        /// </summary>
        private ksDocument2D _current2dDoc;

        /// <summary>
        /// Подключается к запущенному КОМПАС-3D или запускает новый процесс.
        /// </summary>
        public void AttachOrRunCAD()
        {
            if (_kompas != null)
            {
                try
                {
                    var isVisible = _kompas.Visible;
                    return;
                }
                catch (COMException)
                {
                    ReleaseComObject(_kompas);
                    _kompas = null;
                }
            }

            var t = Type.GetTypeFromProgID("KOMPAS.Application.5");
            if (t == null)
            {
                throw new InvalidOperationException(
                    "Не найден ProgID KOMPAS.Application.5. " +
                    "Убедитесь, что КОМПАС-3D установлен.");
            }

            _kompas = (KompasObject)Activator.CreateInstance(t)
                ?? throw new InvalidOperationException(
                    "Не удалось создать KompasObject.");

            _kompas.Visible = true;
            _kompas.ActivateControllerAPI();
        }


        /// <summary>
        /// Создаёт новый 3D-документ (деталь) и получает верхнюю деталь.
        /// </summary>
        public void CreateDocument3D()
        {
            if (_kompas == null)
            {
                throw new InvalidOperationException(
                    "Kompas не инициализирован. Сначала вызови "
                    + "AttachOrRunCAD().");
            }

            _doc3D = (ksDocument3D)_kompas.Document3D()
                ?? throw new InvalidOperationException(
                    "Не удалось получить ksDocument3D.");
            _doc3D.Create();

            _part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part)
                ?? throw new InvalidOperationException(
                    "Не удалось получить верхнюю деталь.");
        }

        /// <summary>
        /// Создаёт эскиз на базовой плоскости ("XOY", "XOZ", "YOZ").
        /// </summary>
        /// <param name="plane">Название базовой плоскости: "XOY"/"XY",
        /// "XOZ"/"XZ", "YOZ"/"YZ"</param>
        public object CreateSketchOnPlane(string plane)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована. Вызови CreateDocument3D().");
            }

            short planeType = plane?.ToUpperInvariant() switch
            {
                "XOY" or "XY" => (short)Obj3dType.o3d_planeXOY,
                "XOZ" or "XZ" => (short)Obj3dType.o3d_planeXOZ,
                "YOZ" or "YZ" => (short)Obj3dType.o3d_planeYOZ,
                _ => (short)Obj3dType.o3d_planeXOY
            };

            var basePlane = (ksEntity)_part.GetDefaultEntity(planeType)
                ?? throw new InvalidOperationException(
                    "Не удалось получить базовую плоскость.");

            var sketchEntity = (ksEntity)_part
                .NewEntity((short)Obj3dType.o3d_sketch)
                ?? throw new InvalidOperationException(
                    "Не удалось создать сущность o3d_sketch.");

            var sketchDef =
                (ksSketchDefinition)sketchEntity.GetDefinition();
            sketchDef.SetPlane(basePlane);
            sketchEntity.Create();

            _current2dDoc = (ksDocument2D)sketchDef.BeginEdit();
            return sketchEntity;
        }

        /// <summary>
        /// Рисует окружность на активном эскизе.
        /// </summary>
        /// <param name="xc">X-координата центра окружности</param>
        /// <param name="yc">Y-координата центра окружности</param>
        /// <param name="radius">Радиус окружности</param>
        public void DrawCircle(double xc, double yc, double radius)
        {
            if (_current2dDoc == null)
            {
                throw new InvalidOperationException(
                    "Нет активного 2D-эскиза. Сначала вызови "
                    + "CreateSketchOnPlane().");
            }
            _current2dDoc.ksCircle(xc, yc, radius, 1);
        }

        /// <summary>
        /// Создает эскиз на указанной плоскости.
        /// </summary>
        /// <param name="planeEntity">Объект плоскости (ksEntity),
        /// на которой создается эскиз</param>
        /// <returns>Объект эскиза (ksEntity)</returns>
        public object CreateSketchOnPlaneEntity(object planeEntity)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована.");
            }

            if (planeEntity is not ksEntity ksPlaneEntity)
            {
                throw new ArgumentException(
                    "Ожидался объект плоскости (ksEntity).",
                    nameof(planeEntity));
            }

            var sketchEntity = (ksEntity)_part
                .NewEntity((short)Obj3dType.o3d_sketch)
                ?? throw new InvalidOperationException(
                    "Не удалось создать сущность эскиза.");

            var sketchDef = (ksSketchDefinition)sketchEntity.GetDefinition();
            sketchDef.SetPlane(ksPlaneEntity);
            sketchEntity.Create();

            _current2dDoc = (ksDocument2D)sketchDef.BeginEdit();
            return sketchEntity;
        }

        /// <summary>
        /// Завершает редактирование эскиза.
        /// </summary>
        /// <param name="sketch">Объект эскиза (ksEntity),
        /// редактирование которого завершается</param>
        public void FinishSketch(object sketch)
        {
            if (sketch is not ksEntity sketchEntity)
            {
                throw new ArgumentException(
                    "Ожидался объект эскиза (ksEntity).", nameof(sketch));
            }

            var sketchDef =
                (ksSketchDefinition)sketchEntity.GetDefinition();
            sketchDef.EndEdit();
            _current2dDoc = null;
        }

        /// <summary>
        /// Выполняет операцию выдавливания.
        /// </summary>
        /// <param name="sketch">Объект эскиза (ksEntity) для
        /// выдавливания</param>
        /// <param name="height">Высота выдавливания</param>
        /// <param name="direction">Направление выдавливания
        /// (true - прямое, false - обратное)</param>
        /// <param name="symmetric">Флаг симметричного выдавливания
        /// в обе стороны</param>
        public void Extrude(object sketch, double height,
            bool direction = true, bool symmetric = false)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована. Вызови CreateDocument3D().");
            }

            if (sketch is not ksEntity sketchEntity)
            {
                throw new ArgumentException(
                    "Ожидался объект эскиза (ksEntity).", nameof(sketch));
            }

            ksEntity extr = _part
                .NewEntity((short)Obj3dType.o3d_baseExtrusion)
                ?? throw new InvalidOperationException(
                    "Не удалось создать сущность o3d_baseExtrusion.");

            ksBaseExtrusionDefinition def =
                (ksBaseExtrusionDefinition)extr.GetDefinition();
            def.SetSketch(sketchEntity);

            ksExtrusionParam p =
                (ksExtrusionParam)def.ExtrusionParam();

            if (symmetric)
            {
                p.direction = (short)Direction_Type.dtBoth;
                p.typeNormal = (short)End_Type.etBlind;
                p.typeReverse = (short)End_Type.etBlind;
                p.depthNormal = height / 2;
                p.depthReverse = height / 2;
            }
            else if (!direction)
            {
                p.direction = (short)Direction_Type.dtReverse;
                p.typeReverse = (short)End_Type.etBlind;
                p.depthReverse = height;
            }
            else
            {
                p.direction = (short)Direction_Type.dtNormal;
                p.typeNormal = (short)End_Type.etBlind;
                p.depthNormal = height;
            }
            extr.Create();
        }

        /// <summary>
        /// Сохранение модели на диск.
        /// </summary>
        /// <param name="path">Полный путь к файлу для сохранения</param>
        public void SaveAs(string path)
        {
            if (_doc3D == null)
            {
                throw new InvalidOperationException(
                    "Документ не создан. Вызови CreateDocument3D().");
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(
                    "Путь к файлу не задан.", nameof(path));
            }
            _doc3D.SaveAs(path);
        }

        /// <summary>
        /// Создает смещенную плоскость, параллельную одной из базовых.
        /// </summary>
        /// <param name="basePlaneType">Тип базовой плоскости</param>
        /// <param name="offset">Смещение от базовой плоскости</param>
        /// <returns>Объект смещенной плоскости (ksEntity)</returns>
        public object CreateOffsetPlane(short basePlaneType, double offset)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована.");
            }

            var basePlane = (ksEntity)_part.GetDefaultEntity(basePlaneType);
            var offsetPlaneEntity = (ksEntity)_part
                .NewEntity((short)Obj3dType.o3d_planeOffset);
            var offsetPlaneDef =
                (ksPlaneOffsetDefinition)offsetPlaneEntity.GetDefinition();

            offsetPlaneDef.SetPlane(basePlane);
            offsetPlaneDef.offset = offset;
            offsetPlaneDef.direction = true;
            offsetPlaneEntity.Create();

            return offsetPlaneEntity;
        }

        /// <summary>
        /// Создает элемент по траектории (кинематическая операция).
        /// </summary>
        /// <param name="profileSketch">Эскиз сечения</param>
        /// <param name="trajectorySketch">Эскиз траектории</param>
        public void CreateElementByTrajectory(object profileSketch,
            object trajectorySketch)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована.");
            }

            if (profileSketch is not ksEntity profileSketchEntity)
            {
                throw new ArgumentException(
                    "Ожидался эскиз (ksEntity) для сечения.",
                    nameof(profileSketch));
            }

            if (trajectorySketch is not ksEntity trajectorySketchEntity)
            {
                throw new ArgumentException(
                    "Ожидался эскиз (ksEntity) для траектории.",
                    nameof(trajectorySketch));
            }

            ksEntity kinematicElement = _part
                .NewEntity((short)Obj3dType.o3d_baseEvolution)
                ?? throw new InvalidOperationException(
                    "Не удалось создать сущность o3d_baseEvolution.");

            ksBaseEvolutionDefinition definition =
                (ksBaseEvolutionDefinition)kinematicElement.GetDefinition();
            definition.SetSketch(profileSketchEntity);

            ksEntityCollection entityCollection =
                (ksEntityCollection)definition.PathPartArray()
                ?? throw new InvalidOperationException(
                    "Не удалось получить коллекцию траекторий.");

            entityCollection.Clear();
            entityCollection.Add(trajectorySketchEntity);
            kinematicElement.Create();
        }

        /// <summary>
        /// Безопасно освобождает COM-объект.
        /// </summary>
        /// <param name="comObject">COM-объект для освобождения</param>
        private static void ReleaseComObject(object comObject)
        {
            if (comObject == null)
            {
                return;
            }

            try
            {
                if (Marshal.IsComObject(comObject))
                {
                    Marshal.FinalReleaseComObject(comObject);
                }
            }
            catch
            {
                // Игнорируем ошибки освобождения
            }
        }
    }
}