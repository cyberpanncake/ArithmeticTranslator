using System;
using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;

namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Serializable]
    public class Identifier : Token
    {
        /// <summary>
        /// Значение атрибута
        /// </summary>
        public int Attribute { get; }

        /// <summary>
        /// Тип переменной
        /// </summary>
        public IdentifierType IdentifierType { get; }

        /// <summary>
        /// Тип генерации идентификатора
        /// </summary>
        public IdentifierCreationType CreationType { get; }

        public Identifier(int position, int attribute, IdentifierType identifierType,
            IdentifierCreationType creationType = IdentifierCreationType.USER) : base(position, TokenType.IDENTIFIER)
        {
            CheckAttribute(attribute);
            Attribute = attribute;
            IdentifierType = identifierType;
            CreationType = creationType;
        }

        public Identifier(int attribute, IdentifierType identifierType,
            IdentifierCreationType creationType = IdentifierCreationType.USER) : base(TokenType.IDENTIFIER)
        {
            CheckAttribute(attribute);
            Attribute = attribute;
            IdentifierType = identifierType;
            CreationType = creationType;
        }

        /// <summary>
        /// Проверяет правильность атрибута
        /// </summary>
        /// <param name="attribute">Атрибут</param>
        private static void CheckAttribute(int attribute)
        {
            if (attribute <= 0)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.IDENTIFIER, "Атрибут должен быть больше 0!");
            }
        }

        override
        public string ToString()
        {
            return $"<{TokenUtils.GetTokenName(Type)},{Attribute}>";
        }
    }
}