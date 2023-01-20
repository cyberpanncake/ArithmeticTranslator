using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using TranslationInterpretationLibrary.Rules.SemanticRules;
using TranslationInterpretationLibrary.Rules.SyntaxRules;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Сематический анализатор
    /// </summary>
    class SemanticAnalyzer
    {
        /// <summary>
        /// Модифицирует синтаксическое дерево, дополняя его приведением типов
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        public static void Analyze(SyntaxTree syntaxTree)
        {
            DetermineOperationResultRecursive(syntaxTree.Root);
        }

        /// <summary>
        /// Рекурсивно опредеяет тип результата операции для каждого узла синтаксического дерева
        /// </summary>
        /// <param name="currentNode">Текущий узел синтаксического дерева</param>
        private static void DetermineOperationResultRecursive(SyntaxNode currentNode)
        {
            if (TokenUtils.IsOperator(currentNode.Value))
            {
                CheckDivisionByZero(currentNode);
                DetermineOperationResultRecursive(currentNode.LeftChild);
                DetermineOperationResultRecursive(currentNode.RightChild);
                DetermineCurrentOperationResult(currentNode);
                return;
            }
            DetermineNumberOrIdentifierResultType(currentNode);
        }

        /// <summary>
        /// Проверяет деление на 0 в узле синтаксического дерева
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void CheckDivisionByZero(SyntaxNode node)
        {
            Token rightChildToken = node.RightChild.Value;
            if (node.Value.Type == TokenType.DIVIDE && rightChildToken is NumericConstant numericConstant)
            {
                float number = float.Parse(numericConstant.Name.Replace('.', ','));
                if (number == 0)
                {
                    throw new SemanticException($"Деление на ноль на позиции {node.Value.Position}");
                }
            }
        }


        /// <summary>
        /// Определяет тип результата операции в узле, когда для дочерних узлов тип результата уже известен
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        public static void DetermineCurrentOperationResult(SyntaxNode node)
        {
            if (node.LeftChild != null && node.RightChild != null)
            {
                if (node.LeftChild.OperationResult == OperationResultType.INTEGER
                    && node.RightChild.OperationResult == OperationResultType.INTEGER)
                {
                    node.OperationResult = OperationResultType.INTEGER;
                    return;
                }
                node.OperationResult = OperationResultType.FLOAT;
                CastNodeChildrenTypes(node);
            }
        }

        /// <summary>
        /// Добавляет в дерево операцию приведения типа к дочерним узлам node, если их тип результата - целое число
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void CastNodeChildrenTypes(SyntaxNode node)
        {
            SyntaxNode leftChild = node.LeftChild;
            SyntaxNode rightChild = node.RightChild;
            if (leftChild.OperationResult == OperationResultType.INTEGER)
            {
                SyntaxNode intermediateParent = new SyntaxNode(leftChild.Parent, new Token(TokenType.INT_TO_FLOAT))
                {
                    RightChild = leftChild,
                    OperationResult = OperationResultType.FLOAT
                };
                leftChild.Parent.LeftChild = intermediateParent;
                leftChild.Parent = intermediateParent;
            }
            if (rightChild.OperationResult == OperationResultType.INTEGER)
            {
                SyntaxNode intermediateParent = new SyntaxNode(rightChild.Parent, new Token(TokenType.INT_TO_FLOAT))
                {
                    RightChild = rightChild,
                    OperationResult = OperationResultType.FLOAT
                };
                rightChild.Parent.RightChild = intermediateParent;
                rightChild.Parent = intermediateParent;
            }
        }

        /// <summary>
        /// Определяет тип результата узла для числовой константы или идентификатора
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void DetermineNumberOrIdentifierResultType(SyntaxNode node)
        {
            Token token = node.Value;
            if (token is Identifier identifier)
            {
                node.OperationResult = identifier.IdentifierType == IdentifierType.INTEGER ?
                    OperationResultType.INTEGER : OperationResultType.FLOAT;
            }
            else if (token is NumericConstant)
            {
                node.OperationResult = token.Type == TokenType.INT_CONSTANT ?
                    OperationResultType.INTEGER : OperationResultType.FLOAT;
            }
        }
    }
}