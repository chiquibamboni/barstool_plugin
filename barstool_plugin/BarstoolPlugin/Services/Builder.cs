using BarstoolPluginCore.Model;
using Kompas6API5;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BarstoolPlugin.Services
{
    /// <summary>
    /// Строит 3D-модель барного стула в КОМПАС-3D.
    /// </summary>
    public class Builder
    {
        /// <summary>
        /// Обертка для работы с API КОМПАС-3D.
        /// </summary>
        private readonly Wrapper _wrapper;

        /// <summary>
        /// Инициализирует новый экземпляр класса.
        /// </summary>
        public Builder()
        {
            _wrapper = new Wrapper();
        }

        /// <summary>
        /// Главный сценарий построения модели.
        /// </summary>
        public void Build(Parameters parameters)
        {
            _wrapper.AttachOrRunCAD();
            _wrapper.CreateDocument3D();

            double legDiameter = parameters.GetValue(
                ParameterType.LegDiameterD1);
            double footrestDiameter = parameters.GetValue(
                ParameterType.FootrestDiameterD2);
            double seatDiameter = parameters.GetValue(
                ParameterType.SeatDiameterD);
            double footrestHeight = parameters.GetValue(
                ParameterType.FootrestHeightH1);
            double stoolHeight = parameters.GetValue(
                ParameterType.StoolHeightH);
            double seatDepth = parameters.GetValue(
                ParameterType.SeatDepthS);
            double thicknessSeat = 40;

            double legHeight = stoolHeight - thicknessSeat;
            double footrestHeightUp = legHeight - footrestHeight;
            double distanceFromCenter = ((seatDiameter / 2) - seatDepth
                - (legDiameter / 2)) / Math.Sqrt(2);

            BuildSeat(seatDiameter, thicknessSeat);
            BuildLag(legDiameter, legHeight, distanceFromCenter);
            BuildFootrest(footrestDiameter, footrestHeightUp,
                distanceFromCenter);

            try
            {
                var directory = GetModelsDirectory();
                var filePath = Path.Combine(directory,
                    CreateFileName(parameters));

                SaveModel(filePath);
            }
            finally
            {
                _wrapper.CloseActiveDocument();
            }
        }

        /// <summary>
        /// Строит сидение стула.
        /// </summary>
        /// <param name="seatDiameter">Диаметр сидения (D)</param>
        /// <param name="thicknessSeat">Толщина сидения (S)</param>
        private void BuildSeat(double seatDiameter, double thicknessSeat)
        {
            object sketch = _wrapper.CreateSketchOnPlane("XOY");
            try
            {
                _wrapper.DrawCircle(0, 0, seatDiameter / 2);
            }
            finally
            {
                _wrapper.FinishSketch(sketch);
            }

            _wrapper.Extrude(sketch, thicknessSeat, true);
        }

        /// <summary>
        /// Строит ножки барного стула.
        /// </summary>
        /// <param name="legDiameter">Диаметр ножки (D1)</param>
        /// <param name="legHeight">Высота ножек (H)</param>
        /// <param name="distanceFromCenter">Расстояние от центра до центра
        /// ножки</param>
        private void BuildLag(double legDiameter, double legHeight,
            double distanceFromCenter)
        {
            object sketch = _wrapper.CreateSketchOnPlane("XOY");
            try
            {
                _wrapper.DrawCircle(distanceFromCenter,
                    distanceFromCenter, legDiameter / 2);
                _wrapper.DrawCircle(distanceFromCenter,
                    -distanceFromCenter, legDiameter / 2);
                _wrapper.DrawCircle(-distanceFromCenter,
                    -distanceFromCenter, legDiameter / 2);
                _wrapper.DrawCircle(-distanceFromCenter,
                    distanceFromCenter, legDiameter / 2);
            }
            finally
            {
                _wrapper.FinishSketch(sketch);
            }

            _wrapper.Extrude(sketch, legHeight, false);
        }

        /// <summary>
        /// Строит подножку стула.
        /// </summary>
        /// <param name="footrestDiameter">Диаметр подножки (D2)</param>
        /// <param name="footrestHeightUp">Высота подножки (H1)</param>
        /// <param name="distanceFromCenter">Расстояние от центра до центра
        /// ножки</param>
        private void BuildFootrest(double footrestDiameter,
            double footrestHeightUp, double distanceFromCenter)
        {
            object sketch1 = _wrapper.CreateSketchOnPlane("YOZ");
            try
            {
                _wrapper.DrawCircle(footrestHeightUp, distanceFromCenter,
                    footrestDiameter / 2);
                _wrapper.DrawCircle(footrestHeightUp, -distanceFromCenter,
                    footrestDiameter / 2);
            }
            finally
            {
                _wrapper.FinishSketch(sketch1);
            }

            _wrapper.Extrude(sketch1, distanceFromCenter * 2, false, true);

            object sketch2 = _wrapper.CreateSketchOnPlane("XOZ");
            try
            {
                _wrapper.DrawCircle(distanceFromCenter, footrestHeightUp,
                    footrestDiameter / 2);
                _wrapper.DrawCircle(-distanceFromCenter, footrestHeightUp,
                    footrestDiameter / 2);
            }
            finally
            {
                _wrapper.FinishSketch(sketch2);
            }

            _wrapper.Extrude(sketch2, distanceFromCenter * 2, false, true);
        }

        /// <summary>
        /// Сохраняет построенную модель в файл.
        /// </summary>
        /// <param name="path">Путь к файлу модели.</param>
        public void SaveModel(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Путь к файлу не задан.",
                    nameof(path));
            }

            _wrapper.SaveAs(path);
        }

        /// <summary>
        /// Формирует уникальное имя файла по параметрам и времени.
        /// </summary>
        private static string CreateFileName(Parameters p)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Barstool_D{0:0}_D1{1:0}_D2{2:0}_H{3:0}_H1{4:0}_S{5:0}_" +
                "{6:yyyyMMdd_HHmmss_fff}.m3d",
                p.GetValue(ParameterType.SeatDiameterD),
                p.GetValue(ParameterType.LegDiameterD1),
                p.GetValue(ParameterType.FootrestDiameterD2),
                p.GetValue(ParameterType.StoolHeightH),
                p.GetValue(ParameterType.FootrestHeightH1),
                p.GetValue(ParameterType.SeatDepthS),
                DateTime.Now);
        }

        /// <summary>
        /// Возвращает путь к директории, в которую будет сохраняться
        /// 3d-документ.
        /// </summary>
        private static string GetModelsDirectory()
        {
            var documents = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            var directory = Path.Combine(documents, "BarstoolPlugin",
                "Models");
            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}