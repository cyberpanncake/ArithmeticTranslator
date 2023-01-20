using System;

namespace TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions
{
    /// <summary>
    /// Ошибка передачи неверного параметра в конструктор
    /// </summary>
    public class InvalidTranslationObjectArgumentException : Exception
    {
        /// <summary>
        /// Тип объекта трансляции
        /// </summary>
        private readonly TranslationObjectType objectType;

        public InvalidTranslationObjectArgumentException(TranslationObjectType objectType, string message) : base(message)
        {
            this.objectType = objectType;
        }

        /// <summary>
        /// Дополняет сообщение исключения строкой с типом объекта трансляции
        /// </summary>
        /// <returns>Полное сообщение исключения</returns>
        public string GetMessageWithType()
        {
            return $"Ошибка создания {TranslationObjectUtils.TranslationObjectTypeToString(objectType)}!{(Message.Length == 0 ? "" : " " + Message)}";
        }
    }
}