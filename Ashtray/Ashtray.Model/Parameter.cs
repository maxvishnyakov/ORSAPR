using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Model
{
    // TODO: XML
    public class Parameter : IEquatable<Parameter> 
    {
        /// <summary>
        /// Значение параметра.
        /// </summary>
        private int _value;

        /// <summary>
        /// Минимальное допустимое значение параметра.
        /// </summary>
        private readonly int _minValue;

        /// <summary>
        /// Максимальное допустимое значение параметра.
        /// </summary>
        private readonly int _maxValue;

        /// <summary>
        /// Сообщение о несоблюдении границы минимума.
        /// </summary>
        private readonly string _minErrorMessage;

        /// <summary>
        /// Сообщение о несоблюдении границы максимума.
        /// </summary>
        private readonly string _maxErrorMessage;

        /// <summary>
        /// Список ошибок введенного параметра.
        /// </summary>
        private readonly Dictionary<ParameterType, string> _errors;

        /// <summary>
        /// Тип параметра.
        /// </summary>
        private readonly ParameterType _parameterType;

        /// <summary>
        /// Создает объект класса параметра.
        /// </summary>
        /// <param name="value">Введенное значение параметра.</param>
        /// <param name="minValue">Минимальное возможное значение.</param>
        /// <param name="maxValue">Максимальное возможное значение.</param>
        /// <param name="errorMessage">Сообщение о том, какой это параметр.</param>
        /// <param name="parameterType">Тип параметра.</param>
        /// <param name="errors"></param>
        public Parameter(int value, int minValue, int maxValue,
            string errorMessage, ParameterType parameterType,
            Dictionary<ParameterType, string> errors)
        {
            _errors = errors;
            _minValue = minValue;
            _maxValue = maxValue;
            _minErrorMessage = errorMessage + " не может быть менее " +
                              _minValue + " мм.";
            _maxErrorMessage = errorMessage + " не может быть более " +
                              _maxValue + " мм.";
            _parameterType = parameterType;
            Value = value;
        }

        /// <summary>
        /// Устанавливает и возвращает значение параметра.
        /// </summary>
        public int Value
        {
            get => _value;
            set
            {
                if (CheckRange(value))
                {
                    _value = value;
                }
            }
        }

        /// <summary>
        /// Проверка принадлежности диапазону введенного параметра.
        /// </summary>
        /// <param name="value">Введенное значение параметра.</param>
        public bool CheckRange(double value)
        {
            if (value < _minValue)
            {
                _errors.Add(_parameterType, _minErrorMessage);
                return false;
            }

            if (value > _maxValue)
            {
                _errors.Add(_parameterType, _maxErrorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Проверка на равенство объектов класса.
        /// </summary>
        /// <param name="expected">Сравниваемый объект.</param>
        /// <returns>Возвращает true, если элементы равны,
        /// false - в обратном случае.</returns>
        public bool Equals(Parameter expected)
        {
            return expected != null &&
                   expected.Value.Equals(Value) &&
                   expected._minValue.Equals(_minValue) &&
                   expected._maxValue.Equals(_maxValue) &&
                   expected._minErrorMessage.Equals(_minErrorMessage) &&
                   expected._maxErrorMessage.Equals(_maxErrorMessage) &&
                   expected._errors.Equals(_errors) &&
                   expected._parameterType.Equals(_parameterType);
        }
    }
}
