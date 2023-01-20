using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens
{
    /// <summary>
    /// Утилита для работы с токенами
    /// </summary>
    public static class TokenUtils
    {
        /// <summary>
        /// Список "статических" токенов с соответствием лексемы и типа токена
        /// </summary>
        private static readonly Dictionary<TokenType, string> tokenNames = new Dictionary<TokenType, string>()
        {
            { TokenType.ADD, "+" },
            { TokenType.SUBTRACT, "-" },
            { TokenType.MULTIPLY, "*" },
            { TokenType.DIVIDE, "/" },
            { TokenType.INT_TO_FLOAT, "i2f"},
            { TokenType.OPEN_BRACKET, "(" },
            { TokenType.CLOSE_BRACKET, ")" },
            { TokenType.IDENTIFIER, "id" }
        };

        /// <summary>
        /// Токены-операторы
        /// </summary>
        private static readonly List<TokenType> operators =
            new List<TokenType> { TokenType.ADD, TokenType.SUBTRACT, TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.INT_TO_FLOAT };

        /// <summary>
        /// Регулярное выражение для проверки на разрешённые символы "нестатического" токена
        /// </summary>
        public static string NonStaticTokenAllowedSymbolPattern
        {
            get { return @"[A-Za-z0-9_.\[\]]"; }
        }

        /// <summary>
        /// Регулярное выражение для проверки на возможное соответствие "нестатическому" токену
        /// </summary>
        public static string ProbablyNumericConstantPattern
        {
            get { return "[0-9]"; }
        }

        /// <summary>
        /// Регулярное выражение для проверки на соответствие константе целого типа
        /// </summary>
        public static string IntConstantPattern
        {
            get { return "^[0-9]+$"; }
        }

        /// <summary>
        /// Регулярное выражение для проверки на соответствие константе вещественного типа
        /// </summary>
        public static string FloatConstantPattern
        {
            get { return "^[0-9]+.[0-9]+$"; }
        }

        /// <summary>
        /// Регулярное выражение для проверки на возможное соответствие идентификатору
        /// </summary>
        public static string ProbablyIdentifierPattern
        {
            get { return "[A-Za-z_]"; }
        }

        /// <summary>
        /// Регулярное выражение для проверки на соответствие идентификатору
        /// </summary>
        public static string IdentifierPattern
        {
            get { return @"^[A-Za-z_][A-Za-z_0-9]*(\[[fFiI]\])?$"; }
        }

        /// <summary>
        /// Список команд промежуточного кода, соответствующих операторам
        /// </summary>
        private static readonly Dictionary<TokenType, string> operatorsPortable = new Dictionary<TokenType, string>()
        {
            { TokenType.ADD, "add" },
            { TokenType.SUBTRACT, "sub" },
            { TokenType.MULTIPLY, "mul" },
            { TokenType.DIVIDE, "div" },
            { TokenType.INT_TO_FLOAT, "i2f" }
        };

        /// <summary>
        /// Получает имя токена по его типу
        /// </summary>
        /// <param name="tokenType">Тип токена</param>
        /// <returns>Имя токена</returns>
        public static string GetTokenName(TokenType tokenType)
        {
            if (tokenNames.ContainsKey(tokenType))
            {
                return tokenNames[tokenType];
            }
            throw new LexicalException("У числовых констант нет общего имени токена.");
        }

        /// <summary>
        /// Определяет тип токена по лексеме
        /// </summary>
        /// <param name="lexem">Лексема</param>
        /// <returns>Тип токена</returns>
        public static TokenType GetTokenType(string lexem)
        {
            foreach (TokenType key in tokenNames.Keys)
            {
                if (tokenNames[key].Equals(lexem))
                {
                    return key;
                }
            }
            if (Regex.IsMatch(lexem, IdentifierPattern))
            {
                return TokenType.IDENTIFIER;
            }
            if (Regex.IsMatch(lexem, IntConstantPattern))
            {
                return TokenType.INT_CONSTANT;
            }
            if (Regex.IsMatch(lexem, FloatConstantPattern))
            {
                return TokenType.FLOAT_CONSTANT;
            }
            throw new LexicalException($"Лексема \"{lexem}\" не распознана");
        }

        /// <summary>
        /// Проверка, что выражение начинается со "статической" лексемы
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="token">Полученный токен</param>
        /// <returns>true - если выражение начинается с любой "статической" лексемы, false - в противном случае</returns>
        public static bool StartsWithStaticToken(string expression, out string token)
        {
            token = "";
            foreach (string staticToken in tokenNames.Values)
            {
                if (!staticToken.Equals(tokenNames[TokenType.IDENTIFIER]) && expression.StartsWith(staticToken))
                {
                    token = staticToken;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Ищет ближайшую к началу выражения "статическую" лексему
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <returns>Индекс найденной лексемы, -1 - если лексема не найдена</returns>
        public static int IndexOfStaticToken(string expression)
        {
            int minIndex = expression.Length;
            foreach (string staticToken in tokenNames.Values)
            {
                if (!staticToken.Equals(tokenNames[TokenType.IDENTIFIER]))
                {
                    int index = expression.IndexOf(staticToken);
                    if (index != -1 && minIndex > index)
                    {
                        minIndex = index;
                    }
                }
            }
            if (minIndex == expression.Length)
            {
                minIndex = -1;
            }
            return minIndex;
        }

        /// <summary>
        /// Проверяет, что "нестатическая" лексема содержит только разрешённые символы
        /// </summary>
        /// <param name="lexem">Проверяемая лексема</param>
        /// <param name="errorIndex">Индекс ошибочного символа</param>
        public static void CheckAllowedSymbolsForNonStaticToken(string lexem, out int errorIndex)
        {
            char c;
            errorIndex = -1;
            for (int i = 0; i < lexem.Length; i++)
            {
                if (!Regex.IsMatch((c = lexem[i]).ToString(), TokenUtils.NonStaticTokenAllowedSymbolPattern))
                {
                    errorIndex = i;
                    throw new LexicalException($"Недопустимый символ \"{c}\"");
                }
            }
        }

        /// <summary>
        /// Проверяет, является ли лексема идентификатором
        /// </summary>
        /// <param name="lexem">Проверяемая лексема</param>
        /// <returns>true - если это идентификатор, false - в противном случае</returns>
        public static bool IsMatchIdentifier(string lexem)
        {
            if (Regex.IsMatch(lexem, ProbablyIdentifierPattern))
            {
                if (Regex.IsMatch(lexem, IdentifierPattern))
                {
                    return true;
                }
                if (Regex.IsMatch(lexem[0].ToString(), IntConstantPattern))
                {
                    throw new LexicalException($"Идентификатор \"{lexem}\" не может начинаться с цифры");
                }
                throw new LexicalException($"Идентификатор \"{lexem}\" с указанием типа должен иметь вид имя_идентификатора[f/F/i/I]");
            }
            return false;
        }

        /// <summary>
        /// Проверяет, является ли лексема числовой константой
        /// </summary>
        /// <param name="lexem">Проверяемая лексема</param>
        /// <returns>true - если это числовая константа, false - в противном случае</returns>
        public static bool IsMatchNumericConstant(string lexem)
        {
            if (Regex.IsMatch(lexem, ProbablyNumericConstantPattern))
            {
                if (Regex.IsMatch(lexem, IntConstantPattern) || Regex.IsMatch(lexem, FloatConstantPattern))
                {
                    return true;
                }
                throw new LexicalException($"Неправильно задана константа \"{lexem}\"");
            }
            return false;
        }

        /// <summary>
        /// Проверяет, является ли токен оператором
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <returns>true - если это оператор, false - в противном случае</returns>
        public static bool IsOperator(Token token)
        {
            return operators.Contains(token.Type);
        }

        /// <summary>
        /// Проверяет, является ли токен числовой константой/идентификатором
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <returns>true - если это числовая константа/идентификатор, false - в противном случае</returns>
        public static bool IsNumberOrIdentifier(Token token)
        {
            return token is NumericConstant || token is Identifier;
        }

        /// <summary>
        /// Получает приоритет оператора
        /// </summary>
        /// <param name="token">Оператор</param>
        /// <returns>приоритет операции: 1 - для "+" и "-", 2 - для "*" и "/", -1 - если это не оператор </returns>
        public static int GetOperatorPriority(Token token)
        {
            TokenType type = token.Type;
            if (type == TokenType.ADD || type == TokenType.SUBTRACT)
            {
                return 1;
            }
            if (type == TokenType.MULTIPLY || type == TokenType.DIVIDE)
            {
                return 2;
            }
            return -1;
        }

        /// <summary>
        /// Получает имя команды промежуточного кода по типу оператора
        /// </summary>
        /// <param name="operator">Оператор</param>
        /// <returns>Имя команды промежуточного кода, если токен не яляется операцией - пустую строку</returns>
        public static string GetOperatorPortableCommand(Token @operator)
        {
            if (operatorsPortable.ContainsKey(@operator.Type))
            {
                return operatorsPortable[@operator.Type];
            }
            return "";
        }
    }
}