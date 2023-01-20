using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;
using System.Text.RegularExpressions;
using System;

namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens
{
    /// <summary>
    /// Числовая константа
    /// </summary>
    [Serializable]
    public class NumericConstant : Token
    {
        /// <summary>
        /// Имя константы (0, 37, 71.5 и т.п.)
        /// </summary>
        public string Name { get; }

        public NumericConstant(int position, TokenType type, string name) : base(position, type)
        {
            CheckParameters(type, name);
            Name = name;
        }

        public NumericConstant(TokenType type, string name) : base(type)
        {
            CheckParameters(type, name);
            Name = name;
        }

        /// <summary>
        /// Создаёт числовую константу по целому числу
        /// </summary>
        /// <param name="value">Целое число</param>
        /// <returns>Числовая константа</returns>
        public static NumericConstant Create(int value)
        {
            return new NumericConstant(TokenType.INT_CONSTANT, value.ToString());
        }

        /// <summary>
        /// Создаёт числовую константу по вещественному числу
        /// </summary>
        /// <param name="value">Вещественное число</param>
        /// <returns>Числовая константа</returns>
        public static NumericConstant Create(float value)
        {
            string name = value.ToString().Replace(',', '.');
            if (name.IndexOf('.') == -1)
            {
                name += ".0";
            }
            return new NumericConstant(TokenType.FLOAT_CONSTANT, name);
        }

        /// <summary>
        /// Возвращает числовое значение константы
        /// </summary>
        /// <returns>Числовое значение константы</returns>
        public float GetValue()
        {
            return float.Parse(Name.Replace('.', ','));
        }

        /// <summary>
        /// Проверяет правильность параметров конструктора
        /// </summary>
        /// <param name="type">Тип токена</param>
        /// <param name="name">Имя числовой константы</param>
        private void CheckParameters(TokenType type, string name)
        {
            if (type != TokenType.INT_CONSTANT && type != TokenType.FLOAT_CONSTANT)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.NUMERIC_CONSTANT,
                    "Неверно задан тип числовой константы.");
            }
            if (!TokenUtils.IsMatchNumericConstant(name))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.NUMERIC_CONSTANT,
                    $"\"{name}\" не соответствует числовой константе.");
            }
            if (type == TokenType.INT_CONSTANT && !Regex.IsMatch(name, TokenUtils.IntConstantPattern))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.NUMERIC_CONSTANT,
                    $"\"{name}\" не соответствует целому числу.");
            }
            if (type == TokenType.FLOAT_CONSTANT && !Regex.IsMatch(name, TokenUtils.FloatConstantPattern))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.NUMERIC_CONSTANT,
                    $"\"{name}\" не соответствует вещественному числу.");
            }
        }

        override
        public string ToString()
        {
            return $"<{Name}>";
        }
    }
}