using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.SyntaxRules;
using System.Collections.Generic;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Генератор постфиксной записи по синтаксическому дереву
    /// </summary>
    public static class PostfixGenerator
    {
        /// <summary>
        /// Генерирует последовательность токенов в постфиксной записи по синтаксическому дереву
        /// в оптимальном или неоптимальном режиме
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="optimal">Работа генератора в оптимальном или неоптимальном режиме</param>
        /// <returns>Последовательность токенов в постфиксной записи</returns>
        public static List<Token> Generate(SyntaxTree syntaxTree, bool optimal = false)
        {
            syntaxTree = syntaxTree.Copy();
            if (optimal)
            {
                SyntaxTreeOptimizer.Optimize(syntaxTree);
            }
            List<Token> postfix = new List<Token>();
            SyntaxTreeToPostfixRecursive(postfix, syntaxTree.Root);
            return postfix;
        }

        /// <summary>
        /// Рекурсивно обходит синтаксическое дерево, преобразуя его в постфиксную запись
        /// </summary>
        /// <param name="result">Получившаяся строка</param>
        /// <param name="currentNode">Текущий узел дерева</param>
        private static void SyntaxTreeToPostfixRecursive(List<Token> postfix, SyntaxNode currentNode)
        {
            if (currentNode.LeftChild != null)
            {
                SyntaxTreeToPostfixRecursive(postfix, currentNode.LeftChild);
            }
            if (currentNode.RightChild != null)
            {
                SyntaxTreeToPostfixRecursive(postfix, currentNode.RightChild);
            }
            postfix.Add(currentNode.Value);
        }
    }
}