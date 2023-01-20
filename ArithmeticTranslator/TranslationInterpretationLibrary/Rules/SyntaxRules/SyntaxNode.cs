using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.SemanticRules;

namespace TranslationInterpretationLibrary.Rules.SyntaxRules
{
    /// <summary>
    /// Узел синтаксического дерева
    /// </summary>
    public class SyntaxNode
    {
        /// <summary>
        /// Родительский узел
        /// </summary>
        public SyntaxNode Parent { get; set; }

        /// <summary>
        /// Значение узла
        /// </summary>
        public Token Value { get; set; }

        /// <summary>
        /// Левый дочерний узел
        /// </summary>
        public SyntaxNode LeftChild { get; set; }

        /// <summary>
        /// Правый дочерний узел
        /// </summary>
        public SyntaxNode RightChild { get; set; }

        /// <summary>
        /// Тип результата операции
        /// </summary>
        public OperationResultType OperationResult { get; set; }

        public SyntaxNode(SyntaxNode parent, Token value, OperationResultType operationResult = OperationResultType.UNKNOWN)
        {
            CheckNodeValue(value);
            Parent = parent;
            Value = value;
            LeftChild = null;
            RightChild = null;
            OperationResult = operationResult;
        }

        /// <summary>
        /// Проверяет правильность значения узла синтаксического дерева
        /// </summary>
        /// <param name="value">Значения узла синтаксического дерева</param>
        private static void CheckNodeValue(Token value)
        {
            if (value == null)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.SYNTAX_NODE,
                    "Значение узла синтаксического дерева не может быть пустым!");
            }
        }

        /// <summary>
        /// Проверяет, является ли узел левым дочерним узлом для своего родительского узла
        /// </summary>
        /// <returns>true - если узел является левым дочерним, false - в противном случае</returns>
        public bool IsLeftChild()
        {
            return Parent.LeftChild == this;
        }

        /// <summary>
        /// Проверяет, является ли узел корнем синтаксического дерева
        /// </summary>
        /// <returns>true - если узел является корнем синтаксического дерева, false - в противном случае</returns>
        public bool IsRoot()
        {
            return Parent == null;
        }
    }
}