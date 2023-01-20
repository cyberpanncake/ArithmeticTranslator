using System.Collections.Generic;

namespace TranslationInterpretationLibrary.Exceptions.TranslationExceptions
{
    /// <summary>
    /// Утилита для работы с ошибками при трансляции
    /// </summary>
    internal class TranslationExceptionUtils
    {
        /// <summary>
        /// Строки для дописывания в сообщения ошибок для каждого типа
        /// </summary>
        private static readonly Dictionary<TranslationExceptionType, string> exceptionInfo =
            new Dictionary<TranslationExceptionType, string>()
        {
            { TranslationExceptionType.LEXICAL, "Лексическая ошибка" },
            { TranslationExceptionType.SYNTAX, "Синтаксическая ошибка" },
            { TranslationExceptionType.SEMANTIC, "Семантическая ошибка" },
            { TranslationExceptionType.OPTIMIZATION, "Ошибка оптимизации" }
        };


        /// <summary>
        /// Получает строку с типом ошибки
        /// </summary>
        /// <param name="type">Тип ошибки</param>
        /// <returns>Строку с типом ошибки, пустую строку - если ошибки такого типа нет</returns>
        public static string ExceptionTypeToString(TranslationExceptionType type)
        {
            if (exceptionInfo.ContainsKey(type))
            {
                return exceptionInfo[type];
            }
            return "";
        }
    }
}