using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using TranslationInterpretationLibrary.Rules.PortableCodeRules;
using TranslationInterpretationLibrary.Rules.SyntaxRules;
using Translation.Translation.Stages;
using Translation.Translation.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Translation.Translation
{
    /// <summary>
    /// Транслятор арифметических выражений
    /// </summary>
    public static class Translator
    {
        /// <summary>
        /// Транслирует арифметическое выражение
        /// </summary>
        /// <param name="filenameInput">Имя файла с входных выражением</param>
        /// <param name="finalStage">Этап, после которого нужно завершить трансляцию</param>
        /// <param name="args">Дополнительные параметры, связанные с последним этапом трансляции</param>
        public static void Translate(string filenameInput, TranslationStage finalStage, string[] args)
        {
            if (File.Exists(filenameInput))
            {
                string expression = File.ReadAllText(filenameInput);
                if (expression.Trim().Equals(""))
                {
                    throw new Exception($"Файл \"{filenameInput}\" пуст!");
                }
                LexicalAnalyzer.Analyze(expression, out List<Token> tokens, out Dictionary<int, Symbol> symbols);
                if (finalStage == TranslationStage.LEXICAL_ANALYSIS)
                {
                    WriteTokensAndSymbolsToFiles(tokens, symbols, args);
                    return;
                }
                SyntaxTree syntaxTree = SyntaxAnalyzer.Analyze(tokens);
                if (finalStage == TranslationStage.SYNTAX_ANALYSIS)
                {
                    WriteSyntaxTreeToFile(syntaxTree, args);
                    return;
                }
                SemanticAnalyzer.Analyze(syntaxTree);
                if (finalStage == TranslationStage.SEMANTIC_ANALYSIS)
                {
                    WriteSyntaxTreeToFile(syntaxTree, args);
                    return;
                }
                if (finalStage == TranslationStage.POSTFIX_GENERATION)
                {
                    List<Token> postfix = CheckArgsAndGeneratePostfix(syntaxTree, args);
                    WritePostfixAndSymbolsToFile(postfix, symbols, args);
                    return;
                }
                List<ThreeAddressCommand> portableCode;
                if (finalStage == TranslationStage.PORTABLE_CODE_GENERATION)
                {
                    portableCode = CheckArgsAndGeneratePortableCode(syntaxTree, symbols, args);
                    WritePortableCodeAndSymbolsToFile(portableCode, symbols, args);
                    return;
                }
                portableCode = PortableCodeGenerator.Generate(syntaxTree, symbols, true);
                if (finalStage == TranslationStage.BINARY_CODE_GENERATION)
                {
                    WritePortableCodeAndSymbolsToBinaryFile(portableCode, symbols, args);
                }
                return;
            }
            throw new FileNotFoundException($"Файл \"{filenameInput}\" не существует!");
        }

        /// <summary>
        /// Записывает последовательность токенов и таблицу символов в файлы из дополнительных параметров
        /// </summary>
        /// <param name="tokens">Последовательность токенов</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="args">Дополнительные параметры</param>
        private static void WriteTokensAndSymbolsToFiles(List<Token> tokens, Dictionary<int, Symbol> symbols, string[] args)
        {
            if (args.Length == 2)
            {
                string tokensFilename = args[0];
                string symbolsFilename = args[1];
                if (tokensFilename.Equals(symbolsFilename))
                {
                    throw new Exception("Файлы для токенов и таблицы символов должны иметь разные имена!");
                }
                try
                {
                    File.WriteAllText(tokensFilename, Stringifier.TokensToString(tokens, symbols));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{tokensFilename}\"!");
                }
                try
                {
                    File.WriteAllText(symbolsFilename, Stringifier.SymbolsToString(symbols));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{symbolsFilename}\"!");
                }
                return;
            }
            throw new Exception("Неверное количество входных параметров!");
        }

        /// <summary>
        /// Записывает синтаксическое дерево в файл из дополнительного параметра
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="args">Дополнительные параметры</param>
        private static void WriteSyntaxTreeToFile(SyntaxTree syntaxTree, string[] args)
        {
            if (args.Length == 1)
            {
                string syntaxTreeFilename = args[0];
                try
                {
                    File.WriteAllText(syntaxTreeFilename, Stringifier.SyntaxTreeToString(syntaxTree));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{syntaxTreeFilename}\"!");
                }
                return;
            }
            throw new Exception("Неверное количество входных параметров!");
        }

        /// <summary>
        /// Проверяет наличие параметра оптимальной генерации и генерирует постфиксную запись
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="args">Параметры работы приложения</param>
        /// <returns>Последовательность токенов в постфиксной записи</returns>
        private static List<Token> CheckArgsAndGeneratePostfix(SyntaxTree syntaxTree, string[] args)
        {
            if (args.Length == 3)
            {
                if (args[0].ToLower().Equals("opt"))
                {
                    return PostfixGenerator.Generate(syntaxTree, true);
                }
                throw new Exception("Параметр оптимальной генерации должен иметь вид \"opt\" или \"OPT\"!");
            }
            return PostfixGenerator.Generate(syntaxTree);
        }

        /// <summary>
        /// Записывает последовательность токенов в постфиксной записи и таблицу символов в файл из дополнительного параметра
        /// </summary>
        /// <param name="postfix">Последовательность токенов в постфиксной записи</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="args">Дополнительные параметры</param>
        private static void WritePostfixAndSymbolsToFile(List<Token> postfix, Dictionary<int, Symbol> symbols, string[] args)
        {
            if (args.Length == 2 || args.Length == 3)
            {
                string postfixFilename = args[^2];
                string symbolsFilename = args[^1];
                if (postfixFilename.Equals(symbolsFilename))
                {
                    throw new Exception("Файлы для постфиксной записи и таблицы символов должны иметь разные имена!");
                }
                try
                {
                    File.WriteAllText(postfixFilename, Stringifier.PostfixToString(postfix));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{postfixFilename}\"!");
                }
                try
                {
                    File.WriteAllText(symbolsFilename, Stringifier.SymbolsPortableToString(symbols));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{symbolsFilename}\"!");
                }
                return;
            }
            throw new Exception("Неверное количество входных параметров!");
        }

        /// <summary>
        /// Генерирует промежуточный код на основе синтаксического дерева
        /// </summary>
        /// <param name="syntaxTree">Синтаксическое дерево</param>
        /// <param name="symbols">Таблица символов</param>
        /// <param name="args">Дополнительные параметры</param>
        /// <returns>Промежуточный код (список трёхадресных команд)</returns>
        private static List<ThreeAddressCommand> CheckArgsAndGeneratePortableCode(SyntaxTree syntaxTree,
            Dictionary<int, Symbol> symbols, string[] args)
        {
            if (args.Length == 3)
            {
                if (args[0].ToLower().Equals("opt"))
                {
                    return PortableCodeGenerator.Generate(syntaxTree, symbols, true);
                }
                throw new Exception("Параметр оптимальной генерации должен иметь вид \"opt\" или \"OPT\"!");
            }
            return PortableCodeGenerator.Generate(syntaxTree, symbols);
        }

        /// <summary>
        /// Записывает промежуточный код и дополненную таблицу символов в файлы из дополнительного параметра
        /// </summary>
        /// <param name="portableCode">Промежуточный код</param>
        /// <param name="symbols">Дополненная таблица символов</param>
        /// <param name="args">Дополнительные параметры</param>
        private static void WritePortableCodeAndSymbolsToFile(List<ThreeAddressCommand> portableCode,
            Dictionary<int, Symbol> symbols, string[] args)
        {
            if (args.Length == 2 || args.Length == 3)
            {
                string portableCodeFilename = args[^2];
                string symbolsFilename = args[^1];
                if (portableCodeFilename.Equals(symbolsFilename))
                {
                    throw new Exception("Файлы для промежуточного кода и таблицы символов должны иметь разные имена!");
                }
                try
                {
                    File.WriteAllText(portableCodeFilename, Stringifier.PortableCodeToString(portableCode));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{portableCodeFilename}\"!");
                }
                try
                {
                    File.WriteAllText(symbolsFilename, Stringifier.SymbolsPortableToString(symbols));
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{symbolsFilename}\"!");
                }
                return;
            }
            throw new Exception("Неверное количество входных параметров!");
        }

        /// <summary>
        /// Записывает промежуточный код и таблицу символов в файл из дополнительного параметра в бинарном формате
        /// </summary>
        /// <param name="portableCode">Промежуточный код</param>
        /// <param name="symbols">Дополненная таблица символов</param>
        /// <param name="args">Дополнительные параметры</param>
        private static void WritePortableCodeAndSymbolsToBinaryFile(List<ThreeAddressCommand> portableCode,
            Dictionary<int, Symbol> symbols, string[] args)
        {
            if (args.Length == 1)
            {
                string binaryCodeFilename = args[0];
                try
                {
                    BinaryCodeGenerator.GenerateAndWriteInFile(portableCode, symbols, binaryCodeFilename);
                }
                catch (Exception)
                {
                    throw new Exception($"Некорректное имя выходного файла \"{binaryCodeFilename}\"!");
                }
                return;
            }
            throw new Exception("Неверное количество входных параметров!");
        }
    }
}