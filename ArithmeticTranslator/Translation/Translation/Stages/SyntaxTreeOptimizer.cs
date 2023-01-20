using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using TranslationInterpretationLibrary.Rules.SemanticRules;
using TranslationInterpretationLibrary.Rules.SyntaxRules;
using System;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Оптимизатор синтаксического дерева
    /// </summary>
    public static class SyntaxTreeOptimizer
    {
        /// <summary>
        /// Оптимизирует синтаксическое дерево
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        public static void Optimize(SyntaxTree syntaxTree)
        {
            OptimizeRecursive(syntaxTree, syntaxTree.Root);
            CheckIfSyntaxTreeHasOneNode(syntaxTree);
        }

        /// <summary>
        /// Оптимизирует синтаксическое дерево рекурсивно
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="currentNode">Текущий узел синтаксического дерева</param>
        private static void OptimizeRecursive(SyntaxTree syntaxTree, SyntaxNode currentNode)
        {
            if (currentNode.LeftChild != null)
            {
                OptimizeRecursive(syntaxTree, currentNode.LeftChild);
            }
            if (currentNode.RightChild != null)
            {
                OptimizeRecursive(syntaxTree, currentNode.RightChild);
            }
            if (currentNode.LeftChild != null || currentNode.RightChild != null)
            {
                DeleteUselessIntToFloat(currentNode);
                SemanticAnalyzer.DetermineCurrentOperationResult(currentNode);
            }
            OptimizeChildren(syntaxTree, currentNode);
            OptimizeNode(syntaxTree, currentNode);
        }

        /// <summary>
        /// Удаляет ненужную операцию приведения типа у дочерних узлов узла
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void DeleteUselessIntToFloat(SyntaxNode node)
        {
            SyntaxNode leftChild = node.LeftChild;
            SyntaxNode rightChild = node.RightChild;
            if (leftChild != null && rightChild != null)
            {
                if (leftChild.Value.Type == TokenType.INT_TO_FLOAT && rightChild.OperationResult == OperationResultType.INTEGER)
                {
                    node.LeftChild = leftChild.RightChild;
                    leftChild.RightChild.Parent = node;
                }
                if (leftChild.OperationResult == OperationResultType.INTEGER && rightChild.Value.Type == TokenType.INT_TO_FLOAT)
                {
                    node.RightChild = rightChild.RightChild;
                    rightChild.RightChild.Parent = node;
                }
            }
        }

        /// <summary>
        /// Оптимизирует дочерние узлы
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void OptimizeChildren(SyntaxTree syntaxTree, SyntaxNode node)
        {
            if (node.LeftChild != null && node.LeftChild.Value.Type == TokenType.INT_TO_FLOAT)
            {
                OptimizeNode(syntaxTree, node.LeftChild);
            }
            if (node.RightChild != null && node.RightChild.Value.Type == TokenType.INT_TO_FLOAT)
            {
                OptimizeNode(syntaxTree, node.RightChild);
            }
        }

        /// <summary>
        /// Оптимизирует узел синтаксического дерева
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void OptimizeNode(SyntaxTree syntaxTree, SyntaxNode node)
        {
            if (node.LeftChild != null || node.RightChild != null)
            {
                OptimizeIfChildrenAreNumbers(syntaxTree, node);
                OptimizeIfIntToFloat(syntaxTree, node);
                OptimizeUselessOperations(syntaxTree, node);
            }
        }

        /// <summary>
        /// Оптимизирует операции, где оба операнда являются числовыми константами
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void OptimizeIfChildrenAreNumbers(SyntaxTree syntaxTree, SyntaxNode node)
        {
            SyntaxNode leftChild = node.LeftChild;
            SyntaxNode rightChild = node.RightChild;
            if (leftChild != null && rightChild != null && leftChild.Value is NumericConstant && rightChild.Value is NumericConstant)
            {
                float result = CalculateResult(node);
                CheckResultDivideByZero(node, result);
                NumericConstant resultNumericConstant;
                SyntaxNode resultNode;
                if (result < 0)
                {
                    resultNumericConstant = CreateResultNumericConstant(node, Math.Abs(result));
                    OperationResultType operationResult = resultNumericConstant.Type == TokenType.INT_CONSTANT ?
                        OperationResultType.INTEGER : OperationResultType.FLOAT;
                    resultNode = new SyntaxNode(node.Parent, new Token(TokenType.SUBTRACT), operationResult);
                    SyntaxNode leftOperand;
                    if (operationResult == OperationResultType.INTEGER)
                    {
                        leftOperand = new SyntaxNode(resultNode, NumericConstant.Create(0), OperationResultType.INTEGER);
                    }
                    else
                    {
                        leftOperand = new SyntaxNode(resultNode, NumericConstant.Create((float)0.0), OperationResultType.FLOAT);
                    }
                    SyntaxNode rightOperand = new SyntaxNode(resultNode, resultNumericConstant, operationResult);
                    resultNode.LeftChild = leftOperand;
                    resultNode.RightChild = rightOperand;
                }
                else
                {
                    resultNumericConstant = CreateResultNumericConstant(node, result);
                    OperationResultType operationResult = resultNumericConstant.Type == TokenType.INT_CONSTANT ?
                        OperationResultType.INTEGER : OperationResultType.FLOAT;
                    resultNode = new SyntaxNode(node.Parent, resultNumericConstant, operationResult);
                }
                ReplaceNode(syntaxTree, node, resultNode);
            }
        }

        /// <summary>
        /// Вычислить результат операции в узле
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        /// <returns>Результат операции</returns>
        private static float CalculateResult(SyntaxNode node)
        {
            float leftOperand = ((NumericConstant)node.LeftChild.Value).GetValue();
            float rightOperand = ((NumericConstant)node.RightChild.Value).GetValue();
            float result = 0;
            switch (node.Value.Type)
            {
                case TokenType.ADD:
                    result = leftOperand + rightOperand;
                    break;
                case TokenType.SUBTRACT:
                    result = leftOperand - rightOperand;
                    break;
                case TokenType.MULTIPLY:
                    result = leftOperand * rightOperand;
                    break;
                case TokenType.DIVIDE:
                    result = leftOperand / rightOperand;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Проверяет, что результат операции с числами равен 0 и является правым операндом операции деления,
        /// и в случае успешной проверки кидает исключение
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        /// <param name="result">Результат операции</param>
        private static void CheckResultDivideByZero(SyntaxNode node, float result)
        {
            if (!node.IsRoot() && node.Parent.Value.Type == TokenType.DIVIDE && !node.IsLeftChild() && result == 0)
            {
                throw new OptimizationException($"В результате оптимизации получилось деление на 0 на позиции {node.Parent.Value.Position}.");
            }
        }

        /// <summary>
        /// Создаёт числовую константу результата операции
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        /// <param name="result">Результат операции</param>
        /// <returns>Числовая константа результата операции</returns>
        private static NumericConstant CreateResultNumericConstant(SyntaxNode node, float result)
        {
            if (node.Value.Type == TokenType.DIVIDE || node.OperationResult == OperationResultType.FLOAT)
            {
                return NumericConstant.Create(result);
            }
            return NumericConstant.Create((int)result);
        }

        /// <summary>
        /// Заменяет узел node на результат оптимизации resultNode
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="node">Узел синтаксического дерева</param>
        /// <param name="resultNode">Узел-результат оптимизации</param>
        private static void ReplaceNode(SyntaxTree syntaxTree, SyntaxNode node, SyntaxNode resultNode)
        {
            if (!node.IsRoot())
            {
                if (node.IsLeftChild())
                {
                    node.Parent.LeftChild = resultNode;
                    return;
                }
                node.Parent.RightChild = resultNode;
                return;
            }
            syntaxTree.Root = resultNode;
        }

        /// <summary>
        /// Оптимизирует операцию приведения типов int2float
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void OptimizeIfIntToFloat(SyntaxTree syntaxTree, SyntaxNode node)
        {
            if (node.Value.Type == TokenType.INT_TO_FLOAT && node.RightChild.Value is NumericConstant numericConstant)
            {
                CheckResultDivideByZero(node, int.Parse(numericConstant.Name));
                NumericConstant resultNumericConstant = NumericConstant.Create(float.Parse(numericConstant.Name));
                SyntaxNode resultNode = new SyntaxNode(node.Parent, resultNumericConstant, OperationResultType.FLOAT);
                ReplaceNode(syntaxTree, node, resultNode);
            }
        }

        /// <summary>
        /// Оптимизирует бесполезные операции типа:
        /// A – 0; A * 0; A / 1 и т.д.
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="node">Узел синтаксического дерева</param>
        private static void OptimizeUselessOperations(SyntaxTree syntaxTree, SyntaxNode node)
        {
            if (node.LeftChild != null && node.RightChild != null)
            {
                Token leftOperand = node.LeftChild.Value;
                Token rightOperand = node.RightChild.Value;
                Token resultToken = null;
                OptimizeTokenIfLeftIsIdentifier(ref resultToken, leftOperand, rightOperand, node.Value.Type);
                OptimizeTokenIfRightIsIdentifier(ref resultToken, leftOperand, rightOperand, node.Value.Type);
                if (resultToken != null)
                {
                    if (resultToken is NumericConstant numericConstant)
                    {
                        CheckResultDivideByZero(node, float.Parse(numericConstant.Name));
                    }
                    OperationResultType operationResult;
                    if (resultToken is Identifier identifier)
                    {
                        operationResult = identifier.IdentifierType == IdentifierType.INTEGER ?
                            OperationResultType.INTEGER : OperationResultType.FLOAT;
                    }
                    else
                    {
                        operationResult = resultToken.Type == TokenType.INT_CONSTANT ?
                            OperationResultType.INTEGER : OperationResultType.FLOAT;
                    }
                    SyntaxNode resultNode = new SyntaxNode(node.Parent, resultToken, operationResult);
                    ReplaceNode(syntaxTree, node, resultNode);
                }
            }
        }

        /// <summary>
        /// Оптимизирует бесполезные операции, где левый операнд - идентификатор
        /// </summary>
        /// <param name="resultToken">Токен-результат операции</param>
        /// <param name="leftOperand">Левый операнд</param>
        /// <param name="rightOperand">Правый операнд</param>
        /// <param name="nodeTokenType">Тип токена узла</param>
        private static void OptimizeTokenIfLeftIsIdentifier(ref Token resultToken, Token leftOperand,
            Token rightOperand, TokenType nodeTokenType)
        {
            if (leftOperand is Identifier leftIdentifier)
            {
                if (rightOperand is NumericConstant numericConstant)
                {
                    if (numericConstant.GetValue() == 0)
                    {
                        switch (nodeTokenType)
                        {
                            case TokenType.ADD:                                                                             // A + 0 = A
                            case TokenType.SUBTRACT:                                                                        // A - 0 = A
                                resultToken = leftOperand;
                                break;
                            case TokenType.MULTIPLY:                                                                        // A * 0 = 0
                                resultToken = rightOperand;
                                break;
                        }
                    }
                    if (numericConstant.GetValue() == 1)
                    {
                        if (nodeTokenType == TokenType.MULTIPLY                                                             // A * 1 = A
                            || nodeTokenType == TokenType.DIVIDE)                                                           // A / 1 = A
                        {
                            resultToken = leftOperand;
                        }
                    }
                }
                if (rightOperand is Identifier rightIdentifier && leftIdentifier.Attribute == rightIdentifier.Attribute
                    && nodeTokenType == TokenType.SUBTRACT)                                                                 // A - A = 0
                {
                    resultToken = NumericConstant.Create(0);
                }
            }
        }

        /// <summary>
        /// Оптимизирует бесполезные операции, где правый операнд - идентификатор
        /// </summary>
        /// <param name="resultToken">Токен-результат операции</param>
        /// <param name="leftOperand">Левый операнд</param>
        /// <param name="rightOperand">Правый операнд</param>
        /// <param name="nodeTokenType">Тип токена узла</param>
        private static void OptimizeTokenIfRightIsIdentifier(ref Token resultToken, Token leftOperand,
            Token rightOperand, TokenType nodeTokenType)
        {
            if (leftOperand is NumericConstant numericConstant && rightOperand is Identifier)
            {
                if (numericConstant.GetValue() == 0)
                {
                    switch (nodeTokenType)
                    {
                        case TokenType.ADD:                                                                                 // 0 + A = A
                            resultToken = rightOperand;
                            break;
                        case TokenType.MULTIPLY:                                                                            // 0 * A = 0
                            resultToken = numericConstant;
                            break;
                    }
                }
                if (numericConstant.GetValue() == 1 && nodeTokenType == TokenType.MULTIPLY)                  // 1 * A = A
                {
                    resultToken = rightOperand;
                }
            }
        }

        /// <summary>
        /// Проверяет, не выродилось ли выражение в единственное число/идентификатор
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        private static void CheckIfSyntaxTreeHasOneNode(SyntaxTree syntaxTree)
        {
            SyntaxNode root = syntaxTree.Root;
            if (root.LeftChild == null && root.RightChild == null)
            {
                throw new OptimizationException($"После оптимизации выражение выродилось в единственн{(root.Value is Identifier ? "ый идентификатор" : "ую числовую константу")} {root.Value}.");
            }
        }
    }
}