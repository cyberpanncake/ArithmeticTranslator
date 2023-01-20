using System.Collections.Generic;

namespace TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols
{
    /// <summary>
    /// Утилита для работы с таблицей символов
    /// </summary>
    public static class SymbolUtils
    {
        /// <summary>
        /// Проверяет, что таблица символов уже содержит идентификатор с таким именем
        /// </summary>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="identifierName">Проверяемое имя идентификатора</param>
        /// <param name="identifierKey">Ключ найденного идентификатора в таблице символов</param>
        /// <returns>true - если в таблице символов есть такой идентификатор, false - в противном случае</returns>
        public static bool ContainsIdentifierName(Dictionary<int, Symbol> symbols, string identifierName, out int? identifierKey)
        {
            identifierKey = null;
            foreach (int key in symbols.Keys)
            {
                if (symbols[key].IdentifierName.Equals(identifierName))
                {
                    identifierKey = key;
                    return true;
                }
            }
            return false;
        }
    }
}