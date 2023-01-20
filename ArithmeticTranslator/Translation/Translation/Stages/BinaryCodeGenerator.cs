using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TranslationInterpretationLibrary.Rules.BinaryCodeRules;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using TranslationInterpretationLibrary.Rules.PortableCodeRules;

namespace Translation.Translation.Stages
{
    /// <summary>
    /// Генератор двоичного формата промежуточного кода и таблицы символов
    /// </summary>
    public static class BinaryCodeGenerator
    {
        /// <summary>
        /// Преобразует промежуточный код и таблицу символов в двоичный формат и записывает их в файл
        /// </summary>
        /// <param name="portableCode">Промежуточный код</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="filename">Имя выходного файла</param>
        public static void GenerateAndWriteInFile(List<ThreeAddressCommand> portableCode, Dictionary<int, Symbol> symbols, string filename)
        {
            PortableCodeAndSymbolsWrapper wrapper = new PortableCodeAndSymbolsWrapper(portableCode, symbols);
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream stream = new FileStream(filename, FileMode.OpenOrCreate);
            formatter.Serialize(stream, wrapper);
        }
    }
}