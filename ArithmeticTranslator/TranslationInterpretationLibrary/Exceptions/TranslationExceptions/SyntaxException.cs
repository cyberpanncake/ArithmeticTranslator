namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Синтаксическая ошибка
    /// </summary>
    public class SyntaxException : TranslationException
    {
        public SyntaxException(string message) : base(message)
        {
            type = TranslationExceptionType.SYNTAX;
        }
    }
}