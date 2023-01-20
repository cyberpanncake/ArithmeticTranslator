using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using TranslationInterpretationLibrary.Rules.PortableCodeRules;
using TranslationInterpretationLibrary.Rules.SyntaxRules;
using System.Collections.Generic;
using System.Text;

namespace Translation.Translation.Utils
{
    /// <summary>
    /// Утилита для преобразования в строку различных объектов, получаемых во время трансляции
    /// </summary>
    public static class Stringifier
    {
        /// <summary>
        /// Список соответствия типа токена и его словесного описания
        /// </summary>
        private static readonly Dictionary<TokenType, string> tokensInfo = new Dictionary<TokenType, string>()
        {
            { TokenType.ADD, "операция сложения" },
            { TokenType.SUBTRACT, "операция вычитания" },
            { TokenType.MULTIPLY, "операция умножения" },
            { TokenType.DIVIDE, "операция деления" },
            { TokenType.OPEN_BRACKET, "открывающая скобка" },
            { TokenType.CLOSE_BRACKET, "закрывающая скобка" },
            { TokenType.INT_CONSTANT, "константа целого типа" },
            { TokenType.FLOAT_CONSTANT, "константа вещественного типа" },
            { TokenType.IDENTIFIER, "идентификатор" }
        };

        /// <summary>
        /// Преобразует последовательность токенов в строку с добавлением описания к каждому токену
        /// </summary>
        /// <param name="tokens">Последовательность токенов</param>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Строковое представление послежовательности токенов</returns>
        public static string TokensToString(List<Token> tokens, Dictionary<int, Symbol> symbols)
        {
            StringBuilder result = new StringBuilder();
            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.IDENTIFIER:
                        result.Append($"{IdentifierToString((Identifier)token, symbols)}\n");
                        break;
                    default:
                        result.Append($"{token}\t - {tokensInfo[token.Type]}\n");
                        break;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Преобразует идентификатор в строку с описанием
        /// </summary>
        /// <param name="token">Идентификатор</param>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Строковое представление идентификатора с описанием</returns>
        private static string IdentifierToString(Identifier identifier, Dictionary<int, Symbol> symbols)
        {
            string identifierType = identifier.IdentifierType == IdentifierType.INTEGER ? "целого" : "вещественного";
            return $"{identifier}\t - {tokensInfo[identifier.Type]} с именем {symbols[identifier.Attribute].IdentifierName} {identifierType} типа";
        }

        /// <summary>
        /// Преобразует таблицу символов в строку
        /// </summary>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Строковое представление таблицы символов</returns>
        public static string SymbolsToString(Dictionary<int, Symbol> symbols)
        {
            StringBuilder result = new StringBuilder();
            foreach (int attribute in symbols.Keys)
            {
                result.Append($"{attribute}\t - {symbols[attribute]}\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// Преобразует синтаксическое дерево в строку
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <returns>Строковое представление синтаксического дерева</returns>
        public static string SyntaxTreeToString(SyntaxTree syntaxTree)
        {
            StringBuilder result = new StringBuilder();
            StringifySyntaxTreeRecursive(result, syntaxTree.Root, 0, new List<bool>());
            return result.ToString();
        }

        /// <summary>
        /// Рекурсивное преобразование каждого узла синтаксического дерева в строку
        /// </summary>
        /// <param name="result">Получившаяся строка</param>
        /// <param name="currentNode">Текущий узел дерева</param>
        /// <param name="level">Уровень текущего узла</param>
        /// <param name="areParentsLeft">Были ли каждый из родительских узлов текущего левыми дочерними узлами или нет</param>
        private static void StringifySyntaxTreeRecursive(StringBuilder result, SyntaxNode currentNode,
            int level, List<bool> areParentsLeft)
        {
            for (int i = 0; i < level - 1; i++)
            {
                result.Append(areParentsLeft[i] ? " │   " : "     ");
            }
            if (level > 0)
            {
                result.Append(areParentsLeft[^1] ? " ├───" : " └───");
            }
            result.Append($"{(currentNode.Value.Type == TokenType.INT_TO_FLOAT ? "Int2Float" : currentNode.Value.ToString())}\n");
            if (currentNode.LeftChild != null)
            {
                areParentsLeft.Add(true);
                StringifySyntaxTreeRecursive(result, currentNode.LeftChild, level + 1, areParentsLeft);
                areParentsLeft.RemoveAt(areParentsLeft.Count - 1);
            }
            if (currentNode.RightChild != null)
            {
                areParentsLeft.Add(false);
                StringifySyntaxTreeRecursive(result, currentNode.RightChild, level + 1, areParentsLeft);
                areParentsLeft.RemoveAt(areParentsLeft.Count - 1);
            }
        }

        /// <summary>
        /// Преобразует последовательность токенов в постфиксной записи в строку
        /// </summary>
        /// <param name="postfix">Последовательность токенов в постфиксной записи</param>
        /// <returns>Строка постфиксной записи</returns>
        public static string PostfixToString(List<Token> postfix)
        {
            StringBuilder result = new StringBuilder();
            foreach (Token token in postfix)
            {
                result.Append(token);
            }
            return result.ToString();
        }

        /// <summary>
        /// Преобразует промежуточный код в строку
        /// </summary>
        /// <param name="portableCode">Промежуточный код</param>
        /// <returns>Строковое представление промежуточного кода</returns>
        public static string PortableCodeToString(List<ThreeAddressCommand> portableCode)
        {
            StringBuilder result = new StringBuilder();
            foreach (ThreeAddressCommand operation in portableCode)
            {
                result.Append($"{operation}\n");
            }
            return result.ToString();
        }

        /// <summary>
        /// Преобразует таблицу символов в строку
        /// </summary>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Строковое представление таблицы символов</returns>
        public static string SymbolsPortableToString(Dictionary<int, Symbol> symbols)
        {
            StringBuilder result = new StringBuilder();
            foreach (int key in symbols.Keys)
            {
                Symbol symbol = symbols[key];
                result.Append($"<id,{key}>\t - {symbol.IdentifierName}, {symbol.Type.ToString().ToLower()}\n");
            }
            return result.ToString();
        }
    }
}