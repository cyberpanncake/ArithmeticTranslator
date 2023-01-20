namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Лексическая ошибка
    /// </summary>
    public class LexicalException : TranslationException
    {
        public LexicalException(string message) : base(message)
        {
            type = TranslationExceptionType.LEXICAL;
        }
    }
}