namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers
{
    /// <summary>
    /// Тип создания идентификатора
    /// </summary>
    public enum IdentifierCreationType
    {
        /// <summary>
        /// Пользовательская переменная
        /// </summary>
        USER,

        /// <summary>
        /// Идентификатор, созданный при работе транслятора для хранения промежуточного значения
        /// </summary>
        PROGRAM
    }
}