using System;

namespace BarstoolPluginCore.Model
{
    /// <summary>
    /// Представляет параметр модели с ограничениями по минимальному
    /// и максимальному значению.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Текущее значение параметра.
        /// </summary>
        private int _value;

        /// <summary>
        /// Минимальное допустимое значение параметра.
        /// </summary>
        private int _minValue;

        /// <summary>
        /// Максимальное допустимое значение параметра.
        /// </summary>
        private int _maxValue;

        /// <summary>
        /// Инициализирует новый экземпляр класса Parameter.
        /// </summary>  
        /// <param name="defaultValue">Значение по умолчанию.</param>
        /// <param name="minValue">Минимальное значение.</param>
        /// <param name="maxValue">Максимальное значение.</param>
        public Parameter(int defaultValue, int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            this.Value = defaultValue;
        }

        /// <summary>
        /// Получает или задает текущее значение параметра.
        /// Выбрасывает исключение если значение вне диапазона.
        /// </summary>
        public int Value
        {
            get => _value;
            set
            {
                if (!IsValueInRange(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                        value,
                        $"Значение должно быть в диапазоне " +
                        $"[{_minValue}, {_maxValue}]");
                }
                _value = value;
            }
        }

        /// <summary>
        /// Получает минималное допустимое значение параметра.
        /// </summary>
        public int MinValue
        {
            get => _minValue;
            //TODO: validation
            set => _minValue = value;
        }

        /// <summary>
        /// Получает максимальное допустимое значение параметра.
        /// </summary>
        public int MaxValue
        {
            get => _maxValue;
            //TODO: validation

            set => _maxValue = value;
        }

        /// <summary>
        /// Проверяет значение на соответствие диапазону.
        /// </summary>
        private bool IsValueInRange(int value)
        {
            return value >= _minValue && value <= _maxValue;
        }
    }
}