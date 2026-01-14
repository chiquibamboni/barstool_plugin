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
        /// Подключается к запущенному КОМПАС-3D или запускает новый
        /// процесс.
        /// </summary>
        public void AttachOrRunCAD()
        {
            if (_kompas != null)
            {
                return;
            }

            var t = Type.GetTypeFromProgID("KOMPAS.Application.5");
            if (t == null)
            {
                throw new InvalidOperationException(
                    "Не найден ProgID KOMPAS.Application.5");
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
                    "Kompas не инициализирован. Сначала вызови " +
                    "AttachOrRunCAD().");
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
        /// Возвращает объект эскиза.
        /// </summary>
        public object CreateSketchOnPlane(string plane)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована. Вызови " +
                    "CreateDocument3D().");
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
                                   "Не удалось создать сущность " +
                                   "o3d_sketch.");

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
        public void DrawCircle(double xc, double yc, double radius)
        {
            if (_current2dDoc == null)
            {
                throw new InvalidOperationException(
                    "Нет активного 2D-эскиза. Сначала вызови " +
                    "CreateSketchOnPlane().");
            }

            _current2dDoc.ksCircle(xc, yc, radius, 1);
        }

        /// <summary>
        /// Завершает редактирование эскиза.
        /// </summary>
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
        /// <param name="sketch">Эскиз для выдавливания.</param>
        /// <param name="height">Высота выдавливания (положительное
        /// значение).</param>
        /// <param name="direction">true - выдавливание в нормальном
        /// направлении, false - в обратном</param>
        /// <param name="symmetric">true - симметричное выдавливание в
        /// обе стороны</param>
        public void Extrude(object sketch, double height,
            bool direction = true, bool symmetric = false)
        {
            if (_part == null)
            {
                throw new InvalidOperationException(
                    "Часть не инициализирована. Вызови " +
                    "CreateDocument3D().");
            }

            if (sketch is not ksEntity sketchEntity)
            {
                throw new ArgumentException(
                    "Ожидался объект эскиза (ksEntity).", nameof(sketch));
            }

            ksEntity extr = _part
                .NewEntity((short)Obj3dType.o3d_baseExtrusion)
                           ?? throw new InvalidOperationException(
                               "Не удалось создать сущность " +
                               "o3d_baseExtrusion.");

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
        /// Закрывает текущий 3D-документ КОМПАС-3D и освобождает
        /// COM-ссылки.
        /// Используется для предотвращения накопления открытых документов
        /// и утечек памяти.
        /// </summary>
        public void CloseActiveDocument()
        {
            if (_doc3D == null)
            {
                return;
            }

            try
            {
                _doc3D.close();
            }
            catch
            {
                // Игнорируем
            }
            finally
            {
                // Освобождаем COM-ссылки
                ReleaseComObject(_current2dDoc);
                ReleaseComObject(_part);
                ReleaseComObject(_doc3D);

                _current2dDoc = null;
                _part = null;
                _doc3D = null;
            }
        }

        /// <summary>
        /// Безопасно освобождает COM-объект.
        /// </summary>
        private static void ReleaseComObject(object? comObject)
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
                // Игнорируем
            }
        }
    }
}