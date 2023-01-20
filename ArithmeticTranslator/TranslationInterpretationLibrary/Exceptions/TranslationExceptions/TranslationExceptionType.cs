namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Тип ошибки при трансляции
    /// </summary>
    public enum TranslationExceptionType
    {
        /// <summary>
        /// Лексическая ошибка
        /// </summary>
        LEXICAL,

        /// <summary>
        /// Синтаксическая ошибка
        /// </summary>
        SYNTAX,

        /// <summary>
        /// Семантическая ошибка
        /// </summary>
        SEMANTIC,

        /// <summary>
        /// Ошибка оптимизации
        /// </summary>
        OPTIMIZATION
    }
}