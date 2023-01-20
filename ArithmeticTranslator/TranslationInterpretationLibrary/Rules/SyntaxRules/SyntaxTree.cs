using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using System.Collections.Generic;

namespace TranslationInterpretationLibrary.Rules.SyntaxRules
{
    /// <summary>
    /// Синтаксическое дерево
    /// </summary>
    public class SyntaxTree
    {
        /// <summary>
        /// Корневой узел дерева
        /// </summary>
        public SyntaxNode Root { set; get; }

        public SyntaxTree(SyntaxNode root)
        {
            Root = root;
        }

        /// <summary>
        /// Генерирует синтаксическое дерево на основе постфиксной записи выражения
        /// </summary>
        /// <param name="postfixTokens">Последовательность токенов в постфиксной записи выражения</param>
        public SyntaxTree(Stack<Token> postfixTokens)
        {
            if (postfixTokens.Count != 0)
            {
                Root = new SyntaxNode(null, postfixTokens.Pop());
                GenerateTreeRecursive(postfixTokens, Root);
            }
        }

        /// <summary>
        /// Рекурсивно генерирует синтаскическое дерево на основе постфиксной записи выражения
        /// </summary>
        /// <param name="postfixTokens">Последовательность токенов в постфиксной записи выражения</param>
        /// <param name="currentNode">Текущий узел</param>
        private void GenerateTreeRecursive(Stack<Token> postfixTokens, SyntaxNode currentNode)
        {
            if (TokenUtils.IsOperator(currentNode.Value))
            {
                if (postfixTokens.Count != 0)
                {
                    currentNode.RightChild = new SyntaxNode(currentNode, postfixTokens.Pop());
                    GenerateTreeRecursive(postfixTokens, currentNode.RightChild);
                }
                if (postfixTokens.Count != 0)
                {
                    currentNode.LeftChild = new SyntaxNode(currentNode, postfixTokens.Pop());
                    GenerateTreeRecursive(postfixTokens, currentNode.LeftChild);
                }
            }
        }

        /// <summary>
        /// Создаёт копию синтаксического дерева
        /// </summary>
        /// <returns>Копия синтаксического дерева</returns>
        public SyntaxTree Copy()
        {
            if (Root != null)
            {
                SyntaxNode root = new SyntaxNode(null, Root.Value)
                {
                    OperationResult = Root.OperationResult
                };
                CopyRecursive(root, Root);
                return new SyntaxTree(root);
            }
            return new SyntaxTree(Root);
        }

        /// <summary>
        /// Создаёт копию синтаксического дерева рекурсивно
        /// </summary>
        /// <param name="currentNodeCopy">Текущий скопированный узел</param>
        /// <param name="currentNode">Текущий копируемый узел</param>
        private void CopyRecursive(SyntaxNode currentNodeCopy, SyntaxNode currentNode)
        {
            if (currentNode.LeftChild != null)
            {
                currentNodeCopy.LeftChild = new SyntaxNode(currentNodeCopy, currentNode.LeftChild.Value);
                currentNodeCopy.LeftChild.OperationResult = currentNode.LeftChild.OperationResult;
                CopyRecursive(currentNodeCopy.LeftChild, currentNode.LeftChild);
            }
            if (currentNode.RightChild != null)
            {
                currentNodeCopy.RightChild = new SyntaxNode(currentNodeCopy, currentNode.RightChild.Value);
                currentNodeCopy.RightChild.OperationResult = currentNode.RightChild.OperationResult;
                CopyRecursive(currentNodeCopy.RightChild, currentNode.RightChild);
            }
        }
    }
}