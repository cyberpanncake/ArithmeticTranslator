using System;
using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;

namespace TranslationInterpretationLibrary.Rules.PortableCodeRules
{
    /// <summary>
    /// Трёхадресная команда
    /// </summary>
    [Serializable]
    public class ThreeAddressCommand
    {
        /// <summary>
        /// Оператор
        /// </summary>
        public Token Operator { get; }

        /// <summary>
        /// Приёмник результата операции (идентификатор)
        /// </summary>
        public Identifier Receiver { get; }

        /// <summary>
        /// Левый операнд (идентификатор или число)
        /// </summary>
        public Token LeftOperand { get; }

        /// <summary>
        /// Правый операнд (идентификатор или число)
        /// </summary>
        public Token RightOperand { get; }

        public ThreeAddressCommand(Token @operator, Identifier receiver, Token leftOperand, Token rightOperand)
        {
            CheckParameters(@operator, receiver, leftOperand, rightOperand);
            Operator = @operator;
            Receiver = receiver;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        /// <summary>
        /// Проверяет правильные типы токенов трёхадресной команды
        /// </summary>
        /// <param name="operator">Оператор</param>
        /// <param name="receiver">Приёмник результата операции</param>
        /// <param name="leftOperand">Левый операнд</param>
        /// <param name="rightOperand">Правый операнд</param>
        private void CheckParameters(Token @operator, Identifier receiver, Token leftOperand, Token rightOperand)
        {
            if (@operator == null || !TokenUtils.IsOperator(@operator))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TREE_ADDRESS_COMMAND,
                    "Первый параметр должен быть оператором.");
            }
            if (receiver == null)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TREE_ADDRESS_COMMAND,
                    "Приёмник результата операции должен быть идентификатором.");
            }
            if (leftOperand == null && rightOperand == null)
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TREE_ADDRESS_COMMAND,
                    "Оба операнда не могут отсутствовать.");
            }
            if (leftOperand != null && !TokenUtils.IsNumberOrIdentifier(leftOperand))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TREE_ADDRESS_COMMAND,
                    "Левый операнд должен быть идентификатором или числовой константой.");
            }
            if (rightOperand != null && !TokenUtils.IsNumberOrIdentifier(rightOperand))
            {
                throw new InvalidTranslationObjectArgumentException(TranslationObjectType.TREE_ADDRESS_COMMAND,
                    "Правый операнд должен быть идентификатором или числовой константой.");
            }
        }

        override
        public string ToString()
        {
            string leftOperand = LeftOperand == null ? "" :
                " " + (LeftOperand is NumericConstant leftNumber ? leftNumber.Name : LeftOperand.ToString());
            string rightOperand = RightOperand == null ? "" :
                " " + (RightOperand is NumericConstant rightNumber ? rightNumber.Name : RightOperand.ToString());
            return $"{TokenUtils.GetOperatorPortableCommand(Operator)} {Receiver}{leftOperand}{rightOperand}";
        }
    }
}