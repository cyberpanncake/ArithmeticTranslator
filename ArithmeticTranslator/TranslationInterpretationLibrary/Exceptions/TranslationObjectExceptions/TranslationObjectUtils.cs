using System.Collections.Generic;

namespace TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions
{
    /// <summary>
    /// Утилита для работы с типами объектов трансляции
    /// </summary>
    internal class TranslationObjectUtils
    {
        /// <summary>
        /// Строки для дописывания в сообщения об ошибках создания объектов трансляции
        /// </summary>
        private static readonly Dictionary<TranslationObjectType, string> objectInfo = new Dictionary<TranslationObjectType, string>()
        {
            { TranslationObjectType.TOKEN, "токена" },
            { TranslationObjectType.IDENTIFIER, "идентификатора" },
            { TranslationObjectType.NUMERIC_CONSTANT, "числовой константы" },
            { TranslationObjectType.SYMBOL, "строки таблицы символов" },
            { TranslationObjectType.SYNTAX_NODE, "узла синтаксического дерева" },
            { TranslationObjectType.TREE_ADDRESS_COMMAND, "трёхадресной команды" }
        };


        /// <summary>
        /// Получает строку с типом объекта трансляции
        /// </summary>
        /// <param name="type">Тип объекта трансляции</param>
        /// <returns>Строку с типом объекта трансляции, пустую строку - если объекта трансляции такого типа нет</returns>
        public static string TranslationObjectTypeToString(TranslationObjectType type)
        {
            if (objectInfo.ContainsKey(type))
            {
                return objectInfo[type];
            }
            return "";
        }
    }
}