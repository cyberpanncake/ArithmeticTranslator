namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Синтаксическая ошибка
    /// </summary>
    public class SemanticException : TranslationException
    {
        public SemanticException(string message) : base(message)
        {
            type = TranslationExceptionType.SEMANTIC;
        }
    }
}
