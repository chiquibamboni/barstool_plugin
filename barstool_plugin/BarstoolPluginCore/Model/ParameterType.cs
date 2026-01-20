using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarstoolPluginCore.Model
{
    /// <summary>
    /// Типы параметров барного стула
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// Диаметр ножки стула (D1)
        /// </summary>
        LegDiameterD1,

        /// <summary>
        /// Диаметр подставки для ног (D2)
        /// </summary>
        FootrestDiameterD2,

        /// <summary>
        /// Диаметр сиденья (D)
        /// </summary>
        SeatDiameterD,

        /// <summary>
        /// Высота подставки для ног от пола (H1)
        /// </summary>
        FootrestHeightH1,

        /// <summary>
        /// Общая высота стула (H)
        /// </summary>
        StoolHeightH,

        /// <summary>
        /// Вылет сиденья (S)
        /// </summary>
        SeatDepthS,

        /// <summary>
        /// Количество ножек (C)
        /// </summary>
        LegCountC
    }
}