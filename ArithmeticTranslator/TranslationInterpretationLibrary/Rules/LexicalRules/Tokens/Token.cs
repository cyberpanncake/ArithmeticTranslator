using System;
using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;

namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens
{
    /// <summary>
    /// Токен арифметического выражения
    /// </summary>
    [Serializable]
    public class Token
    {
        /// <summary>
        /// Позиция токена в строке выражения (с учётом непечатных символов)
        /// </summary>
        public int? Position { get; }

        /// <summary>
        /// Тип токена
        /// </summary>
        public TokenType Type { get; }

        public Token(TokenType type)
        {
            Position = null;
            Type = type;
        }

        public Token(int position, TokenType type)
        {
            CheckPosition(position);
            Position = position;
            Type = type;
        }

        public Token(int position, string lexem)
        {
            CheckPosition(position);
            Position = position;
            try
            {
                Type = TokenUtils.GetTokenType(lexem);
            }
            catch (LexicalException)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TOKEN,
                    $"Лексема \"{lexem}\" не является токеном.");
            }
        }

        /// <summary>
        /// Проверяет правильность позиции токена
        /// </summary>
        /// <param name="position">Позиция токена</param>
        private static void CheckPosition(int position)
        {
            if (position <= 0)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TOKEN,
                    "Позиция токена должна быть положительной.");
            }
        }

        override
        public string ToString()
        {
            return $"<{TokenUtils.GetTokenName(Type)}>";
        }
    }
}