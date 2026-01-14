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
        private readonly Dictionary<ParameterType, Parameter> _parameters;

        /// <summary>
        /// Коллектор ошибок валидации.
        /// </summary>
        private readonly List<ValidationError> _errorCollector;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameters с параметрами
        /// по умолчанию.
        /// </summary>
        public Parameters()
        {
            _parameters = new Dictionary<ParameterType, Parameter>
            {
                { ParameterType.LegDiameterD1,
                    new Parameter(25, 25, 70) },
                { ParameterType.FootrestDiameterD2,
                    new Parameter(10, 10, 50) },
                { ParameterType.SeatDiameterD,
                    new Parameter(300, 300, 500) },
                { ParameterType.FootrestHeightH1,
                    new Parameter(200, 200, 400) },
                { ParameterType.StoolHeightH,
                    new Parameter(700, 700, 900) },
                { ParameterType.SeatDepthS,
                    new Parameter(20, 20, 100) },
            };

            _errorCollector = new List<ValidationError>();
        }

        /// <summary>
        /// Удаляет ошибки для конкретного типа параметра.
        /// </summary>
        public void RemoveErrorsForParameter(ParameterType parameterType)
        {
            _errorCollector.RemoveAll(e =>
                e.AffectedParameters.Contains(parameterType));
        }

        /// <summary>
        /// Проверяет, есть ли ошибки в коллекторе.
        /// </summary>
        public bool HasErrors => _errorCollector.Count > 0;

        /// <summary>
        /// Получает значение параметра по его типу.
        /// </summary>
        public int GetValue(ParameterType parameterType)
        {
            return _parameters[parameterType].Value;
        }

        /// <summary>
        /// Устанавливает значение параметра с валидацией.
        /// </summary>
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
                    fieldName
                );
                _errorCollector.Add(error);
                return false;
            }

            ValidateDependencies();

            return true;
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

            if (d1 >= D / 6.0)
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.SeatDiameterD
                };

                var error = new ValidationError(
                    affectedParams,
                    $"Диаметр ножки (d1 = {d1}) должен быть " +
                    $"конструктивно меньше диаметра сидения: " +
                    $"d1 < D/6 (D/6 = {Math.Round(D / 6.0, 1)})"
                );
                _errorCollector.Add(error);
            }

            if (d2 > d1)
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.FootrestDiameterD2
                };

                var error = new ValidationError(
                    affectedParams,
                    $"Диаметр подножки (d2 = {d2}) не должен превышать " +
                    $"диаметр ножки (d1 = {d1})"
                );
                _errorCollector.Add(error);
            }

            if (S >= 3 * d1)
            {
                var affectedParams = new List<ParameterType>
                {
                    ParameterType.LegDiameterD1,
                    ParameterType.SeatDepthS
                };

                var error = new ValidationError(
                    affectedParams,
                    $"Вылет сидения (S = {S}) ограничен соотношением: " +
                    $"S < 3*d1 (3*d1 = {3 * d1})"
                );
                _errorCollector.Add(error);
            }
        }

        /// <summary>
        /// Добавляет ошибку формы в коллектор.
        /// </summary>
        public void AddFormError(List<ParameterType> affectedParameters,
            string message, string fieldName = null)
        {
            foreach (var paramType in affectedParameters)
            {
                RemoveErrorsForParameter(paramType);
            }

            var error = new ValidationError(affectedParameters, message,
                fieldName);
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
    }
}