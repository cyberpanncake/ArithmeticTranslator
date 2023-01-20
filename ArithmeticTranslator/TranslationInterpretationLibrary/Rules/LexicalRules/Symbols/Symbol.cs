using System;
using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;

namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols
{
    /// <summary>
    /// Строка таблицы символов
    /// </summary>
    [Serializable]
    public class Symbol
    {
        /// <summary>
        /// Имя идентификатора
        /// </summary>
        public string IdentifierName { get; }

        /// <summary>
        /// Тип идентификатора
        /// </summary>
        public IdentifierType Type { get; }

        public IdentifierCreationType CreationType { get; }

        public Symbol(string identifierName, IdentifierType type, IdentifierCreationType creationType)
        {
            CheckIdentifierName(identifierName);
            IdentifierName = identifierName;
            Type = type;
            CreationType = creationType;
        }

        /// <summary>
        /// Проверяет правильность имени идентификатора
        /// </summary>
        /// <param name="identifierName">Имя идентификатора</param>
        private static void CheckIdentifierName(string identifierName)
        {
            if (!TokenUtils.IsMatchIdentifier(identifierName))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.SYMBOL,
                    "Имя идентификатора должно быть непустым и соответствовать формату записи идентификатора!");
            }
        }

        override
        public string ToString()
        {
            return $"{IdentifierName} [{(Type == IdentifierType.INTEGER ? "целый" : "вещественный")}]";
        }
    }
}