using System;

namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Ошибка при трансляции
    /// </summary>
    public abstract class TranslationException : Exception
    {
        /// <summary>
        /// Тип ошибки (соответствует этапу трансляции)
        /// </summary>
        protected TranslationExceptionType type;

        public TranslationException(string message) : base(message) { }

        /// <summary>
        /// Дополняет сообщение исключения строкой с типом ошибки
        /// </summary>
        /// <returns>Полное сообщение исключения</returns>
        public string GetMessageWithType()
        {
            return $"{TranslationExceptionUtils.ExceptionTypeToString(type)}!{(Message.Length == 0 ? "" : " " + Message)}";
        }
    }
}