namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens
{
    /// <summary>
    /// Тип токена
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Операция сложения
        /// </summary>
        ADD,

        /// <summary>
        /// Операция вычитания
        /// </summary>
        SUBTRACT,

        /// <summary>
        /// Операция умножения
        /// </summary>
        MULTIPLY,

        /// <summary>
        /// Операция деления
        /// </summary>
        DIVIDE,

        /// <summary>
        /// Открывающая скобка
        /// </summary>
        OPEN_BRACKET,

        /// <summary>
        /// Закрывающая скобка
        /// </summary>
        CLOSE_BRACKET,

        /// <summary>
        /// Целочисленная константа
        /// </summary>
        INT_CONSTANT,

        /// <summary>
        /// Вещественная константа
        /// </summary>
        FLOAT_CONSTANT,

        /// <summary>
        /// Идентификатор
        /// </summary>
        IDENTIFIER,

        /// <summary>
        /// Преобразование целого числа в вещественное
        /// </summary>
        INT_TO_FLOAT
    }
}