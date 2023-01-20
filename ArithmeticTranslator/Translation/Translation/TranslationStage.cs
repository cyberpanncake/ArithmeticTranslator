namespace Translation.Translation
{
    /// <summary>
    /// Этапы трансляции
    /// </summary>
    public enum TranslationStage
    {
        /// <summary>
        /// Лексический анализ
        /// </summary>
        LEXICAL_ANALYSIS,

        /// <summary>
        /// Синтаксический анализ
        /// </summary>
        SYNTAX_ANALYSIS,

        /// <summary>
        /// Семантический анализ
        /// </summary>
        SEMANTIC_ANALYSIS,

        /// <summary>
        /// Генерация промежуточного кода
        /// </summary>
        PORTABLE_CODE_GENERATION,

        /// <summary>
        /// Генерация постфиксной записи выражения
        /// </summary>
        POSTFIX_GENERATION,

        /// <summary>
        /// Генерация двоичного формата промежуточного кода и таблицы символов
        /// </summary>
        BINARY_CODE_GENERATION
    }
}