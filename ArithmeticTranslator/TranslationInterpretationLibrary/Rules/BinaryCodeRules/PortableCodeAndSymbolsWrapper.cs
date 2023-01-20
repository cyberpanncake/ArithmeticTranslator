using System;
using System.Collections.Generic;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using TranslationInterpretationLibrary.Rules.PortableCodeRules;

namespace TranslationInterpretationLibrary.Rules.BinaryCodeRules
{
    /// <summary>
    /// Сериализуемая обёртка над промежуточным кодом и таблицей символов
    /// для преобразования в двоичный формат и обратно
    /// </summary>
    [Serializable]
    public class PortableCodeAndSymbolsWrapper
    {
        /// <summary>
        /// Промежуточный код
        /// </summary>
        public List<ThreeAddressCommand> PortableCode { get; }
        /// <summary>
        /// Таблица символов
        /// </summary>
        public Dictionary<int, Symbol> Symbols { get; }

        public PortableCodeAndSymbolsWrapper(List<ThreeAddressCommand> portableCode, Dictionary<int, Symbol> symbols)
        {
            PortableCode = portableCode;
            Symbols = symbols;
        }
    }
}