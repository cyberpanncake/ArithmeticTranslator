using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.SyntaxRules;
using System.Collections.Generic;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Синтаксический анализатор
    /// </summary>
    public static class SyntaxAnalyzer
    {
        /// <summary>
        /// Строит синтаксическое дерево по последовательности токенов,
        /// предварительно преобразовав их в постфиксную запись
        /// </summary>
        /// <param name="tokens">Последовательность токенов</param>
        /// <returns>Синтаксическое дерево</returns>
        public static SyntaxTree Analyze(List<Token> tokens)
        {
            CheckNeighboursForEachToken(tokens);
            List<Token> postfixTokens = ParseInfixToPostfix(tokens);
            return new SyntaxTree(new Stack<Token>(postfixTokens));
        }

        /// <summary>
        /// Проверяет, корректны ли типы соседних справа токенов для каждого токена,
        /// а также корректны ли типы первого и последнего токена
        /// </summary>
        /// <param name="tokens">Последовательность токенов</param>
        private static void CheckNeighboursForEachToken(List<Token> tokens)
        {
            CheckFirstToken(tokens);
            CheckLastToken(tokens);
            for (int i = 1; i < tokens.Count; i++)
            {
                CheckRightNeighbourToken(tokens[i - 1], tokens[i]);
            }
        }

        /// <summary>
        /// Проверяет, корректный ли тип у первого токена в последовательности
        /// </summary>
        /// <param name="tokens">Последовательность токенов</param>
        private static void CheckFirstToken(List<Token> tokens)
        {
            Token firstToken = tokens[0];
            if (TokenUtils.IsOperator(firstToken) || firstToken.Type == TokenType.CLOSE_BRACKET)
            {
                throw new SyntaxException($"Выражение не может начинаться с {(TokenUtils.IsOperator(firstToken) ? "оператора" : "закрывающей скобки")} {firstToken} на позиции {firstToken.Position}.");
            }
        }

        /// <summary>
        /// Проверяет, корректный ли тип у соседнего справа токена
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="rightNeighbour">Соседний справа токен</param>
        private static void CheckRightNeighbourToken(Token token, Token rightNeighbour)
        {
            if (IsNumberOrIdentifierWithAllowedNeighbour(token, rightNeighbour))
                return;
            if (IsOperatorWithAllowedNeighbour(token, rightNeighbour))
                return;
            if (IsOpenBracketWithAllowedNeighbour(token, rightNeighbour))
                return;
            if (IsCloseBracketWithAllowedNeighbour(token, rightNeighbour))
                return;
        }

        /// <summary>
        /// Проверяет, что это идентификатор и корректный тип соседа справа для него
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="rightNeighbour">Соседний справа токен</param>
        /// <returns>true - если это идентификатор и токен справа от него корректный, false - если это не идентификатор</returns>
        private static bool IsNumberOrIdentifierWithAllowedNeighbour(Token token, Token rightNeighbour)
        {
            if (TokenUtils.IsNumberOrIdentifier(token))
            {
                if (!(TokenUtils.IsOperator(rightNeighbour) || rightNeighbour.Type == TokenType.CLOSE_BRACKET))
                {
                    throw new SyntaxException($"После {(token.Type == TokenType.IDENTIFIER ? "идентификатора" : "числа")} {token} на позиции {token.Position} отсутствует оператор или закрывающая скобка.");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, что это оператор и корректный тип соседа справа для него
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="rightNeighbour">Соседний справа токен</param>
        /// <returns>true - если это оператор и токен справа от него корректный, false - если это не оператор</returns>
        private static bool IsOperatorWithAllowedNeighbour(Token token, Token rightNeighbour)
        {
            if (TokenUtils.IsOperator(token))
            {
                if (!(TokenUtils.IsNumberOrIdentifier(rightNeighbour) || rightNeighbour.Type == TokenType.OPEN_BRACKET))
                {
                    throw new SyntaxException($"После оператора {token} на позиции {token.Position} отсутствует число/идентификатор или открывающая скобка.");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, что это открывающая скобка и корректный тип соседа справа для него
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="rightNeighbour">Соседний справа токен</param>
        /// <returns>true - если это открывающая скобка и токен справа от него корректный, false - если это не открывающая скобка</returns>
        private static bool IsOpenBracketWithAllowedNeighbour(Token token, Token rightNeighbour)
        {
            if (token.Type == TokenType.OPEN_BRACKET)
            {
                if (!(TokenUtils.IsNumberOrIdentifier(rightNeighbour) || rightNeighbour.Type == TokenType.OPEN_BRACKET))
                {
                    throw new SyntaxException($"После открывающей скобки {token} на позиции {token.Position} отсутствует число/идентификатор/открывающая скобка.");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, что это закрывающая скобка и корректный тип соседа справа для него
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="rightNeighbour">Соседний справа токен</param>
        /// <returns>true - если это закрывающая скобка и токен справа от него корректный, false - если это не закрывающая скобка</returns>
        private static bool IsCloseBracketWithAllowedNeighbour(Token token, Token rightNeighbour)
        {
            if (token.Type == TokenType.CLOSE_BRACKET)
            {
                if (!(TokenUtils.IsOperator(rightNeighbour) || rightNeighbour.Type == TokenType.CLOSE_BRACKET))
                {
                    throw new SyntaxException($"После закрывающей скобки {token} на позиции {token.Position} отсутствует оператор/закрывающая скобка.");
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, корректный ли тип у последнего токена в последовательности
        /// </summary>
        /// <param name="tokens">Последовательность токенов</param>
        private static void CheckLastToken(List<Token> tokens)
        {
            Token lastToken = tokens[^1];
            if (TokenUtils.IsOperator(lastToken) || lastToken.Type == TokenType.OPEN_BRACKET)
            {
                throw new SyntaxException($"Выражение не может заканчиваться {(TokenUtils.IsOperator(lastToken) ? "оператором" : "открывающей скобкой")} {lastToken} на позиции {lastToken.Position}.");
            }
        }

        /// <summary>
        /// Преобразует последовательность токенов из инфиксной записи в постфиксную
        /// </summary>
        /// <param name="tokens">Последовательность токенов в инфиксной записи</param>
        /// <returns>Последовательность токенов в постфиксной записи</returns>
        private static List<Token> ParseInfixToPostfix(List<Token> tokens)
        {
            Queue<Token> postfixTokens = new Queue<Token>();
            Stack<Token> stack = new Stack<Token>();
            Queue<Token> infixTokens = new Queue<Token>(tokens);
            while (infixTokens.Count != 0)
            {
                Token currentToken = infixTokens.Dequeue();
                CheckAndProcessNumberOrIdentifier(currentToken, postfixTokens);
                CheckAndProcessOperator(currentToken, stack, postfixTokens);
                CheckAndProcessOpenBracket(currentToken, stack);
                CheckAndProcessCloseBracket(currentToken, stack, postfixTokens);
                CheckAndProcessEndOfExpression(infixTokens, stack, postfixTokens);
            }
            return new List<Token>(postfixTokens);
        }

        /// <summary>
        /// Проверяет и работает с чиловой константой/идентификатором
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="postfixTokens">Выходные токены в постфиксной записи</param>
        private static void CheckAndProcessNumberOrIdentifier(Token token, Queue<Token> postfixTokens)
        {
            if (TokenUtils.IsNumberOrIdentifier(token))
            {
                postfixTokens.Enqueue(token);
            }
        }

        /// <summary>
        /// Проверяет и работает с оператором
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="stack">Вспомогательный стек (для приоритетов операций и ПСП)</param>
        /// <param name="postfixTokens">Выходные токены в постфиксной записи</param>
        private static void CheckAndProcessOperator(Token token, Stack<Token> stack, Queue<Token> postfixTokens)
        {
            if (TokenUtils.IsOperator(token))
            {
                while (stack.Count != 0 && TokenUtils.IsOperator(stack.Peek())
                    && TokenUtils.GetOperatorPriority(stack.Peek()) >= TokenUtils.GetOperatorPriority(token))
                {
                    postfixTokens.Enqueue(stack.Pop());
                }
                stack.Push(token);
            }
        }

        /// <summary>
        /// Проверяет и работает с открывающей скобкой
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="stack">Вспомогательный стек (для приоритетов операций и ПСП)</param>
        private static void CheckAndProcessOpenBracket(Token token, Stack<Token> stack)
        {
            if (token.Type == TokenType.OPEN_BRACKET)
            {
                stack.Push(token);
            }
        }

        /// <summary>
        /// Проверяет и работает с закрывающей скобкой
        /// </summary>
        /// <param name="token">Проверяемый токен</param>
        /// <param name="stack">Вспомогательный стек (для приоритетов операций и ПСП)</param>
        /// <param name="postfixTokens">Выходные токены в постфиксной записи</param>
        private static void CheckAndProcessCloseBracket(Token token, Stack<Token> stack, Queue<Token> postfixTokens)
        {
            if (token.Type == TokenType.CLOSE_BRACKET)
            {
                while (stack.Count != 0 && stack.Peek().Type != TokenType.OPEN_BRACKET)
                {
                    postfixTokens.Enqueue(stack.Pop());
                }
                if (stack.Count == 0)
                {
                    throw new SyntaxException($"Для закрывающей скобки на позиции {token.Position} отсутствует открывающая.");
                }
                stack.Pop();
            }
        }

        /// <summary>
        /// Проверяет, что входная последовательность токенов пуста и завершает работу со стеком
        /// </summary>
        /// <param name="infixTokens">Входные токены в инфиксной записи</param>
        /// <param name="stack">Вспомогательный стек (для приоритетов операций и ПСП)</param>
        /// <param name="postfixTokens">Выходные токены в постфиксной записи</param>
        private static void CheckAndProcessEndOfExpression(Queue<Token> infixTokens, Stack<Token> stack, Queue<Token> postfixTokens)
        {
            if (infixTokens.Count == 0)
            {
                while (stack.Count != 0)
                {
                    if (stack.Peek().Type == TokenType.OPEN_BRACKET)
                    {
                        throw new SyntaxException($"Для открывающей скобки на позиции {stack.Peek().Position} отсутствует закрывающая.");
                    }
                    postfixTokens.Enqueue(stack.Pop());
                }
            }
        }
    }
}