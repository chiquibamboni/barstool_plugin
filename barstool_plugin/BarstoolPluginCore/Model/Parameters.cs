using System;
using System.Collections.Generic;
using System.Linq;

namespace BarstoolPluginCore.Model
{
    /// <summary>
    /// Управляет параметрами и их валидацией.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Коллекция параметров модели, индексируемая по типу параметра.
        /// </summary>
        private Dictionary<ParameterType, Parameter> _parameters;

        /// <summary>
        /// Коллектор ошибок валидации.
        /// </summary>
        private List<ValidationError> _errorCollector;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameters с параметрами
        /// по умолчанию.
        /// </summary>
        public Parameters()
        {
            _parameters = new Dictionary<ParameterType, Parameter>
            {
                { ParameterType.LegDiameterD1,
                    new Parameter(50, 25, 70) },
                { ParameterType.FootrestDiameterD2,
                    new Parameter(30, 10, 50) },
                { ParameterType.SeatDiameterD,
                    new Parameter(400, 300, 500) },
                { ParameterType.FootrestHeightH1,
                    new Parameter(300, 200, 400) },
                { ParameterType.StoolHeightH,
                    new Parameter(800, 700, 900) },
                { ParameterType.SeatDepthS,
                    new Parameter(60, 20, 100) },
                { ParameterType.LegCountC,
                    new Parameter(4, 3, 6) }
            };
            _errorCollector = new List<ValidationError>();
        }

        /// <summary>
        /// Проверяет, есть ли ошибки в коллекторе.
        /// </summary>
        public bool HasErrors => _errorCollector.Count > 0;

        /// <summary>
        /// Удаляет ошибки для конкретного типа параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра, для
        /// которого удаляются ошибки</param>
        public void RemoveErrorsForParameter(ParameterType parameterType)
        {
            _errorCollector.RemoveAll(e =>
                e.AffectedParameters.Contains(parameterType));
        }

        /// <summary>
        /// Получает значение параметра по его типу.
        /// </summary>
        /// <param name="parameterType">Тип параметра, значение
        /// которого требуется получить</param>
        public int GetValue(ParameterType parameterType)
        {
            return _parameters[parameterType].Value;
        }

        /// <summary>
        /// Получает минимальное допустимое значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра, минимальное
        /// значение которого требуется получить</param>
        public int GetMin(ParameterType parameterType)
        {
            return _parameters[parameterType].MinValue;
        }

        /// <summary>
        /// Получает максимальное допустимое значение параметра.
        /// </summary>
        /// <param name="parameterType">Тип параметра, максимальное
        /// значение которого требуется получить</param>
        public int GetMax(ParameterType parameterType)
        {
            return _parameters[parameterType].MaxValue;
        }

        /// <summary>
        /// Устанавливает значение параметра с валидацией.
        /// </summary>
        /// <param name="parameterType">Тип параметра, значение
        /// которого устанавливается</param>
        /// <param name="value">Новое значение параметра</param>
        /// <param name="fieldName">Имя поля формы (опционально,
        /// для привязки ошибок к UI)</param>
        public bool SetValue(ParameterType parameterType, int value,
            string fieldName = null)
        {
            RemoveErrorsForParameter(parameterType);
            try
            {
                _parameters[parameterType].Value = value;
            }
            catch (ArgumentOutOfRangeException)
            {
                var parameter = _parameters[parameterType];
                var error = new ValidationError(
                    new List<ParameterType> { parameterType },
                    $"Значение {value} не находится в диапазоне " +
                    $"[{parameter.MinValue}, {parameter.MaxValue}]",
                    fieldName);

                _errorCollector.Add(error);
                return false;
            }
            ValidateDependencies();
            return true;
        }

        /// <summary>
        /// Добавляет ошибку формы в коллектор.
        /// </summary>
        /// <param name="affectedParameters">Список типов параметров,
        /// затронутых ошибкой</param>
        /// <param name="message">Текст сообщения об ошибке</param>
        /// <param name="fieldName">Имя поля формы (опционально, для
        /// привязки ошибок к UI)</param>
        public void AddFormError(List<ParameterType> affectedParameters,
            string message, string fieldName = null)
        {
            foreach (var paramType in affectedParameters)
            {
                RemoveErrorsForParameter(paramType);
            }
            var error = new ValidationError(
                affectedParameters, message, fieldName);
            _errorCollector.Add(error);
        }

        /// <summary>
        /// Получает все типы параметров, у которых есть ошибки.
        /// </summary>
        public List<ParameterType> GetAllParametersWithErrors()
        {
            var result = new List<ParameterType>();
            foreach (var error in _errorCollector)
            {
                result.AddRange(error.AffectedParameters);
            }
            return result.Distinct().ToList();
        }

        /// <summary>
        /// Получает все ошибки в виде строки, сгруппированные по полям.
        /// </summary>
        public string GetErrorMessages()
        {
            var messages = new List<string>();
            foreach (var error in _errorCollector)
            {
                if (!string.IsNullOrEmpty(error.FieldName))
                {
                    messages.Add($"{error.FieldName}: {error.Message}");
                }
                else
                {
                    messages.Add(error.Message);
                }
            }
            return string.Join("\n", messages);
        }
        
        /// <summary>
        /// Проверяет зависимости между параметрами.
        /// </summary>
        private void ValidateDependencies()
        {
            _errorCollector.RemoveAll(e => e.AffectedParameters.Count > 1);

            int d1 = GetValue(ParameterType.LegDiameterD1);
            int d2 = GetValue(ParameterType.FootrestDiameterD2);
            int D = GetValue(ParameterType.SeatDiameterD);
            int S = GetValue(ParameterType.SeatDepthS);
            int C = GetValue(ParameterType.LegCountC);

            if (d1 >= D / 6.0)
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.SeatDiameterD
                };
                var message =
                    $"Диаметр ножки ({d1}) слишком велик для диаметра " +
                    $"сиденья ({D}). Рекомендуемое соотношение: " +
                    $"d1 < D/6 (≈{Math.Round(D / 6.0, 1)}). " +
                    "Уменьшите диаметр ножки или увеличте диаметр сиденья.";
                var error = new ValidationError(affectedParams, message);
                _errorCollector.Add(error);
            }

            if (d2 > d1)
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.FootrestDiameterD2
                };
                var message =
                    $"Диаметр подножки ({d2}) не может быть больше " +
                    $"диаметра ножки ({d1}). Уменьшите диаметр подножки " +
                    "или увеличьте диаметр ножки.";
                var error = new ValidationError(affectedParams, message);
                _errorCollector.Add(error);
            }

            if (S >= 3 * d1)
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.SeatDepthS
                };
                var message =
                    $"Вылет сиденья ({S}) слишком велик по сравнению " +
                    $"с диаметром ножки ({d1}). Требуется, чтобы " +
                    $"S < 3*d1 ({3 * d1}). Уменьшите вылет сиденья " +
                    "или увеличьте диаметр ножки.";
                var error = new ValidationError(affectedParams, message);
                _errorCollector.Add(error);
            }

            double legPlacementRadius = (D / 2.0) - S - (d1 / 2.0);
            if (d1 > 2 * legPlacementRadius * Math.Sin(Math.PI / C))
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.SeatDiameterD,
                    ParameterType.LegCountC,
                    ParameterType.SeatDepthS,
                };
                var message =
                    "Ножки стула пересекаются или расположены слишком " +
                    "близко друг к другу. Увеличьте диаметр сиденья (D) " +
                    "или уменьшите количество ножек (C), их диаметр (d1) " +
                    "или вылет сиденья (S).";
                var error = new ValidationError(affectedParams, message);
                _errorCollector.Add(error);
            }
        }

    }
}