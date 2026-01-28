using BarstoolPluginCore.Model;
using Kompas6Constants3D;
using System;
using System.Globalization;
using System.IO;

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
        private Wrapper _wrapper;

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
        /// <param name="parameters">Объект Parameters,
        /// содержащий все параметры барного стула</param>
        public void Build(Parameters parameters)
        {
            _wrapper.AttachOrRunCAD();
            _wrapper.CreateDocument3D();

            double legDiameter = parameters.GetValue(
                ParameterType.LegDiameterD1);
            int legCount = parameters.GetValue(
                ParameterType.LegCountC);
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
            double distanceFromCenter = (seatDiameter / 2) - seatDepth
                - (legDiameter / 2);

            BuildSeat(seatDiameter, thicknessSeat);
            BuildLegs(legDiameter, legHeight, distanceFromCenter, legCount);
            BuildFootrest(footrestDiameter, footrestHeightUp,
                distanceFromCenter);
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
        /// Строит ножки барного стула в зависимости от их количества.
        /// </summary>
        /// <param name="legDiameter">Диаметр ножки (d1)</param>
        /// <param name="legHeight">Высота ножек (H)</param>
        /// <param name="placementRadius">Радиус расположения ножек
        /// от центра</param>
        /// <param name="legCount">Количество ножек (C)</param>
        private void BuildLegs(double legDiameter, double legHeight,
            double placementRadius, int legCount)
        {
            object sketch = _wrapper.CreateSketchOnPlane("XOY");
            try
            {
                for (int i = 0; i < legCount; i++)
                {
                    double angle = 2 * Math.PI * i / legCount;
                    double x = placementRadius * Math.Cos(angle);
                    double y = placementRadius * Math.Sin(angle);
                    _wrapper.DrawCircle(x, y, legDiameter / 2);
                }
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
        /// <param name="footrestHeightUp">Высота расположения подножки
        /// от пола</param>
        /// <param name="distanceFromCenter">Расстояние от центра до
        /// центра ножки</param>
        private void BuildFootrest(double footrestDiameter,
            double footrestHeightUp, double distanceFromCenter)
        {

            object offsetPlane = _wrapper.CreateOffsetPlane(
                (short)Obj3dType.o3d_planeXOY, -footrestHeightUp);

            object trajectorySketch =
                _wrapper.CreateSketchOnPlaneEntity(offsetPlane);
            try
            {
                _wrapper.DrawCircle(0, 0, distanceFromCenter);
            }
            finally
            {
                _wrapper.FinishSketch(trajectorySketch);
            }

            object profileSketch = _wrapper.CreateSketchOnPlane("YOZ");
            try
            {
                _wrapper.DrawCircle(footrestHeightUp, distanceFromCenter,
                    footrestDiameter / 2);
            }
            finally
            {
                _wrapper.FinishSketch(profileSketch);
            }

            _wrapper.CreateElementByTrajectory(
                profileSketch, trajectorySketch);
        }

        /// <summary>
        /// Закрывает текущий документ без сохранения.
        /// </summary>
        public void CloseDocument()
        { 
            _wrapper.CloseActiveDocument(); 
        }
    }
}