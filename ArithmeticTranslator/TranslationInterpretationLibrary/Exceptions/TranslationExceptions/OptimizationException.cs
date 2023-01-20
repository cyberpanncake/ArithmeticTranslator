namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Ошибка при оптимизации
    /// </summary>
    public class OptimizationException : TranslationException
    {
        public OptimizationException(string message) : base(message)
        {
            type = TranslationExceptionType.OPTIMIZATION;
        }
    }
}