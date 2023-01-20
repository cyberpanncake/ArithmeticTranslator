namespace TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions
{
    /// <summary>
    /// Типы объектов трансляции
    /// </summary>
    public enum TranslationObjectType
    {
        /// <summary>
        /// Токен
        /// </summary>
        TOKEN,

        /// <summary>
        /// Идентификатор
        /// </summary>
        IDENTIFIER,

        /// <summary>
        /// Числовая константа
        /// </summary>
        NUMERIC_CONSTANT,

        /// <summary>
        /// Запись таблицы символов
        /// </summary>
        SYMBOL,

        /// <summary>
        /// Узел синтаксического дерева
        /// </summary>
        SYNTAX_NODE,

        /// <summary>
        /// Трёхадресная команда
        /// </summary>
        TREE_ADDRESS_COMMAND
    }
}