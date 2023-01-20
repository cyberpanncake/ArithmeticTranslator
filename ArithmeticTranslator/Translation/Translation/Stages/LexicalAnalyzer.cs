using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Лексический анализатор
    /// </summary>
    public static class LexicalAnalyzer
    {
        /// <summary>
        /// Анализирует выражение, преобразуя его в последовательность токенов и таблицу символов
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="tokens">Последовательность токенов</param>
        /// <param name="symbols">Таблица символов</param>
        public static void Analyze(string expression, out List<Token> tokens, out Dictionary<int, Symbol> symbols)
        {
            tokens = new List<Token>();
            symbols = new Dictionary<int, Symbol>();
            int currentStartIndex = 0;
            while (currentStartIndex < expression.Length)
            {
                AnalyzeStartStaticTokenSequence(expression, ref currentStartIndex, tokens);
                if (currentStartIndex < expression.Length)
                {
                    AnalyzeNonStaticToken(expression, ref currentStartIndex, tokens, symbols);
                }
            }
        }

        /// <summary>
        /// Записывает идущие подряд "статические" лексемы (начиная с currentStartIndex) в последовательность токенов
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="currentStartIndex">Текущий индекс, начиная с которого рассматривается выражение</param>
        /// <param name="tokens">Последовательность токенов</param>
        private static void AnalyzeStartStaticTokenSequence(string expression, ref int currentStartIndex, List<Token> tokens)
        {
            expression = expression[currentStartIndex..];
            int expLength = expression.Length;
            int i = 0;
            string lexem = "";
            while (i < expLength && TokenUtils.StartsWithStaticToken(expression[i..expLength], out lexem)
                || Regex.IsMatch(expression[i..expLength], "^\\s"))
            {
                if (!lexem.Equals(""))
                {
                    tokens.Add(new Token(currentStartIndex + i + 1, lexem));
                    i += lexem.Length;
                }
                else
                {
                    i++;
                }
            }
            currentStartIndex += i;
        }

        /// <summary>
        /// Записывает "нестатическую" лексему (начиная с currentStartIndex) в последовательность токенов
        /// и (если идентификатор) таблицу символов
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <param name="currentStartIndex">Текущий индекс, начиная с которого рассматривается выражение</param>
        /// <param name="tokens">Последовательность токенов</param>
        /// <param name="symbols">Таблица символов</param>
        private static void AnalyzeNonStaticToken(string expression, ref int currentStartIndex, List<Token> tokens,
            Dictionary<int, Symbol> symbols)
        {
            expression = expression[currentStartIndex..];
            string lexem = GetStartNonStaticToken(expression);
            int errorIndex = -1;
            try
            {
                TokenUtils.CheckAllowedSymbolsForNonStaticToken(lexem, out errorIndex);
            }
            catch (LexicalException e)
            {
                throw new LexicalException($"{e.Message} на позиции {(currentStartIndex + errorIndex + 1)}");
            }
            try
            {
                WriteNonStaticToken(currentStartIndex + 1, lexem, tokens, symbols);
            }
            catch (LexicalException e)
            {
                throw new LexicalException($"{e.Message} на позиции {(currentStartIndex + 1)}");
            }
            currentStartIndex += lexem.Length;
        }

        /// <summary>
        /// Получает подстроку с "нестатической" лексемой (до следующей "статичекой" лексемы или непечатного символа)
        /// </summary>
        /// <param name="expression">Выражение</param>
        /// <returns>Подстрока с "нестатической" лексемой</returns>
        private static string GetStartNonStaticToken(string expression)
        {
            int expLength = expression.Length;
            int nextStaticTokenIndex = TokenUtils.IndexOfStaticToken(expression);
            if (nextStaticTokenIndex == -1)
            {
                nextStaticTokenIndex = expLength;
            }
            int nextWhitespaceIndex = expression.IndexOf(" ");
            if (nextWhitespaceIndex == -1)
            {
                nextWhitespaceIndex = expLength;
            }
            int lexemLength = Math.Min(nextStaticTokenIndex, nextWhitespaceIndex);
            return expression.Substring(0, lexemLength);
        }

        /// <summary>
        /// Пытается определить тип "нестатической" лексемы и записать её
        /// в последовательность токенов и (для идентификатора) таблицу символов
        /// </summary>
        /// <param name="position">Позиция лексемы в строке выражения</param>
        /// <param name="lexem">Лексема</param>
        /// <param name="tokens">Последовательность токенов</param>
        /// <param name="symbols">Таблица символов</param>
        private static void WriteNonStaticToken(int position, string lexem, List<Token> tokens, Dictionary<int, Symbol> symbols)
        {
            if (TokenUtils.IsMatchIdentifier(lexem))
            {
                WriteIdentifier(position, lexem, tokens, symbols);
            }
            else if (TokenUtils.IsMatchNumericConstant(lexem))
            {
                WriteNumericConstant(position, lexem, tokens);
            }
            else
            {
                throw new LexicalException($"Лексема \"{lexem}\" не распознана");
            }
        }

        /// <summary>
        /// Записывает идентификатор в послежовательность токенов и таблицу символов
        /// </summary>
        /// <param name="position">Позиция идентификатора в строке выражения</param>
        /// <param name="name">Имя идентификатора</param>
        /// <param name="tokens">Последовательность токенов</param>
        /// <param name="symbols">Таблица символов</param>
        private static void WriteIdentifier(int position, string name, List<Token> tokens, Dictionary<int, Symbol> symbols)
        {
            IdentifierType identifierType = IdentifierType.INTEGER;
            string identifierName = name;
            if (name.EndsWith(']'))
            {
                identifierName = name[0..(name.Length - 3)];
                identifierType = name[^2] == 'i' || name[^2] == 'I' ? IdentifierType.INTEGER : IdentifierType.FLOAT;
            }
            if (SymbolUtils.ContainsIdentifierName(symbols, identifierName, out int? attribute))
            {
                if (symbols[Convert.ToInt32(attribute)].Type != identifierType)
                {
                    throw new SemanticException($"Идентификатор \"{name}\" не соответствует по типу своему первому объявлению");
                }
                tokens.Add(new Identifier(position, Convert.ToInt32(attribute), identifierType));
            }
            else
            {
                attribute = symbols.Count + 1;
                symbols.Add(Convert.ToInt32(attribute), new Symbol(identifierName, identifierType, IdentifierCreationType.USER));
                tokens.Add(new Identifier(position, Convert.ToInt32(attribute), identifierType));
            }
        }

        /// <summary>
        /// Записывает числовую константу в последовательность токенов
        /// </summary>
        /// <param name="position">Позиция числовой константы в строке выражения</param>
        /// <param name="name">Имя числовой константы</param>
        /// <param name="tokens">Последовательность токенов</param>
        private static void WriteNumericConstant(int position, string name, List<Token> tokens)
        {
            if (Regex.IsMatch(name, TokenUtils.IntConstantPattern))
            {
                name = long.Parse(name).ToString();
                tokens.Add(new NumericConstant(position, TokenType.INT_CONSTANT, name));
            }
            else if (Regex.IsMatch(name, TokenUtils.FloatConstantPattern))
            {
                name = float.Parse(name.Replace('.', ',')).ToString().Replace(',', '.');
                tokens.Add(new NumericConstant(position, TokenType.FLOAT_CONSTANT, name));
            }
        }
    }
}