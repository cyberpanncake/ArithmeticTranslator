using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using TranslationInterpretationLibrary.Rules.PortableCodeRules;
using TranslationInterpretationLibrary.Rules.SemanticRules;
using TranslationInterpretationLibrary.Rules.SyntaxRules;
using System.Collections.Generic;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Генератор промежуточного кода
    /// </summary>
    public static class PortableCodeGenerator
    {
        /// <summary>
        /// Генерирует промежуточный код на основе синтаксического дерева и таблицы символов
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="optimal">Работа генератора в оптимальном или неоптимальном режиме</param>
        /// <returns>Промежуточный код (список трёхадресных команд)</returns>
        public static List<ThreeAddressCommand> Generate(SyntaxTree syntaxTree, Dictionary<int, Symbol> symbols, bool optimal = false)
        {
            syntaxTree = syntaxTree.Copy();
            if (optimal)
            {
                SyntaxTreeOptimizer.Optimize(syntaxTree);
            }
            List<ThreeAddressCommand> portableCode = new List<ThreeAddressCommand>();
            GeneratePortableCodeRecursive(syntaxTree.Root, symbols, portableCode, optimal);
            return portableCode;
        }

        /// <summary>
        /// Генерирует промежуточный код на основе рекурсивного обхода синтаксического дерева и табблицы символов
        /// </summary>
        /// <param name="currentNode">Текущий узел синтаксического дерева</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="portableCode">Промежуточный код (список трёхадресных команд)</param>
        /// <param name="optimal">Работа генератора в оптимальном или неоптимальном режиме</param>
        private static void GeneratePortableCodeRecursive(SyntaxNode currentNode, Dictionary<int, Symbol> symbols,
            List<ThreeAddressCommand> portableCode, bool optimal)
        {
            SyntaxNode leftChild = currentNode.LeftChild;
            SyntaxNode rightChild = currentNode.RightChild;
            if (leftChild != null && !TokenUtils.IsNumberOrIdentifier(leftChild.Value))
            {
                GeneratePortableCodeRecursive(leftChild, symbols, portableCode, optimal);
                leftChild = currentNode.LeftChild;
            }
            if (rightChild != null && !TokenUtils.IsNumberOrIdentifier(rightChild.Value))
            {
                GeneratePortableCodeRecursive(rightChild, symbols, portableCode, optimal);
                rightChild = currentNode.RightChild;
            }
            Identifier receiver = GetReseiver(currentNode, symbols, optimal);
            Token leftOperand = leftChild?.Value;
            Token rightOperand = rightChild?.Value;
            portableCode.Add(new ThreeAddressCommand(currentNode.Value, receiver, leftOperand, rightOperand));
            ReplaceNodeWithReceiver(ref currentNode, receiver);
        }

        /// <summary>
        /// Получает переменную-приёмник результата операции.
        /// Если генератор работает в оптимальном режиме, то если в одном из дочерних узлов текущего
        /// есть идентификатор, созданный во время генерации промежуточного кода, то он берётся в качестве приёмника.
        /// В противном случае генерируется новый идентификатор
        /// </summary>
        /// <param name="currentNode">Текущий узел синтаксического дерева</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="optimal">Работа генератора в оптимальном или неоптимальном режиме</param>
        /// <returns>Идентификатор-приёмник результата операции</returns>
        private static Identifier GetReseiver(SyntaxNode currentNode, Dictionary<int, Symbol> symbols, bool optimal)
        {
            if (optimal)
            {
                SyntaxNode leftChild = currentNode.LeftChild;
                if (leftChild != null && leftChild.Value is Identifier leftIdentifier &&
                    leftIdentifier.CreationType == IdentifierCreationType.PROGRAM)
                {
                    return leftIdentifier;
                }
                SyntaxNode rightChild = currentNode.RightChild;
                if (rightChild != null && rightChild.Value is Identifier rightIdentifier &&
                    rightIdentifier.CreationType == IdentifierCreationType.PROGRAM)
                {
                    return rightIdentifier;
                }
            }
            return GenerateReceiver(currentNode, symbols);
        }

        /// <summary>
        /// Генерирует переменную-приёмник результата операции
        /// </summary>
        /// <param name="currentNode">Текущий узел синтаксического дерева</param>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Идентификатор-приёмник результата операции</returns>
        private static Identifier GenerateReceiver(SyntaxNode currentNode, Dictionary<int, Symbol> symbols)
        {
            int attribute = symbols.Count + 1;
            string identifierName = GenerateNextIdentifierName(symbols);
            IdentifierType identifierType = currentNode.OperationResult == OperationResultType.INTEGER ?
                IdentifierType.INTEGER : IdentifierType.FLOAT;
            symbols.Add(attribute, new Symbol(identifierName, identifierType, IdentifierCreationType.PROGRAM));
            return new Identifier(attribute, identifierType, IdentifierCreationType.PROGRAM);
        }

        /// <summary>
        /// Генерирует новое уникальное имя идентификатора
        /// </summary>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Новое уникальное имя идентификатора</returns>
        private static string GenerateNextIdentifierName(Dictionary<int, Symbol> symbols)
        {
            string name = "T";
            int i = 1;
            while (true)
            {
                string testName = name + i;
                bool nameExist = false;
                foreach (Symbol symbol in symbols.Values)
                {
                    if (symbol.IdentifierName.Equals(testName))
                    {
                        nameExist = true;
                        break;
                    }
                }
                if (!nameExist)
                {
                    return testName;
                }
                i++;
            }
        }

        /// <summary>
        /// Заменяет текущий узел на переменную-приёмник
        /// </summary>
        /// <param name="node">Узел синтаксического дерева</param>
        /// <param name="receiver">Переменная-приёмник результата операции</param>
        private static void ReplaceNodeWithReceiver(ref SyntaxNode node, Token receiver)
        {
            SyntaxNode nodeReceiver = new SyntaxNode(node.Parent, receiver)
            {
                OperationResult = node.OperationResult
            };
            if (!node.IsRoot())
            {
                if (node.IsLeftChild())
                {
                    node.Parent.LeftChild = nodeReceiver;
                    return;
                }
                node.Parent.RightChild = nodeReceiver;
                return;
            }
            node = nodeReceiver;
        }
    }
}