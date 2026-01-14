using System;
using System.Collections.Generic;

namespace BarstoolPluginCore.Model
{
    /// <summary>
    /// Хранилище для ошибки валидации.
    /// </summary>
    public class ValidationError
    {
        private readonly List<ParameterType> _affectedParameters;
        private readonly string _message;
        private readonly string _fieldName;

        /// <summary>
        /// Инициализирует новый экземпляр класса ValidationError.
        /// </summary>
        /// <param name="affectedParameters">
        /// Список затронутых параметров.</param>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="fieldName">Название поля (опционально).</param>
        public ValidationError(List<ParameterType> affectedParameters,
            string message, string fieldName = null)
        {
            if (affectedParameters == null || affectedParameters.Count == 0)
            {
                throw new ArgumentException("Список параметров не " +
                    "может быть пустым.", nameof(affectedParameters));
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Сообщение об ошибке не " +
                    "может быть пустым.", nameof(message));
            }

            _affectedParameters = new List<ParameterType>(affectedParameters);
            _message = message;
            _fieldName = fieldName;
        }

        /// <summary>
        /// Получает сообщение об ошибке.
        /// </summary>
        public string Message => _message;

        /// <summary>
        /// Получает название поля (если задано).
        /// </summary>
        public string FieldName => _fieldName;

        /// <summary>
        /// Получает список затронутых параметров.
        /// </summary>
        public List<ParameterType> AffectedParameters => _affectedParameters;
    }
}