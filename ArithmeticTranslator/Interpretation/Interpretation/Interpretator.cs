using Interpretation.Interpretation.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TranslationInterpretationLibrary.Rules.BinaryCodeRules;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Identifiers;
using TranslationInterpretationLibrary.Rules.LexicalRules.Tokens.Symbols;
using TranslationInterpretationLibrary.Rules.PortableCodeRules;

namespace Interpretation.Interpretation
{
    /// <summary>
    /// Интерпретатор промежуточного кода в двоичном формате
    /// </summary>
    public static class Interpretator
    {
        /// <summary>
        /// Считывает промежуточный код и таблицу символов из файла в двоичном формате и выполняет полученный код
        /// </summary>
        /// <param name="filename">Входной файл</param>
        public static void Execute(string filename)
        {
            PortableCodeAndSymbolsWrapper wrapper = ReadCodeFromFile(filename);
            Dictionary<int, NumericData> data = CreateData(wrapper.Symbols);
            Console.WriteLine("\nВычисление арифметического выражения:");
            List<ThreeAddressCommand> portableCode = wrapper.PortableCode;
            ExecuteCode(portableCode, data);
            Console.WriteLine($"\nРезультат: {data[portableCode[^1].Receiver.Attribute]}");
        }

        /// <summary>
        /// Считывает промежуточный код и таблицу символов из файла в двоичном формате
        /// </summary>
        /// <param name="filename">Имя входного файла</param>
        /// <returns>Обёртка промежуточного кода и таблицы символов</returns>
        private static PortableCodeAndSymbolsWrapper ReadCodeFromFile(string filename)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using FileStream stream = new FileStream(filename, FileMode.Open);
            return (PortableCodeAndSymbolsWrapper)formatter.Deserialize(stream);
        }

        /// <summary>
        /// Создаёт ячейки данных для хранения переменных, заданных пользователем в исходном выражении,
        /// и приёмников промежуточных результатов, сгенерированных транслятором
        /// </summary>
        /// <param name="symbols">Таблица символов</param>
        /// <returns>Словарь ячеек данных с ключами - атрибутами токенов (указателями на строку таблицы символов)</returns>
        private static Dictionary<int, NumericData> CreateData(Dictionary<int, Symbol> symbols)
        {
            Dictionary<int, NumericData> data = new Dictionary<int, NumericData>();
            foreach (int attribute in symbols.Keys)
            {
                Symbol symbol = symbols[attribute];
                if (symbol.CreationType == IdentifierCreationType.USER)
                {
                    data.Add(attribute, CreateUserDataByInput(symbol));
                }
                else
                {
                    data.Add(attribute, CreateProgrammData(symbol));
                }
            }
            return data;
        }

        /// <summary>
        /// Создаёт ячейку данных переменной, заданной пользователем в исходном выражении,
        /// с помощью ввода с консоли её значения
        /// </summary>
        /// <param name="symbol">Строка таблицы символов</param>
        /// <returns>Ячейку данных пользовательской переменной</returns>
        private static NumericData CreateUserDataByInput(Symbol symbol)
        {
            while (true)
            {
                try
                {
                    Console.Write($"Введите значение {symbol.IdentifierName} ({(symbol.Type == IdentifierType.INTEGER ? "целое" : "вещественное")}): ");
                    if (symbol.Type == IdentifierType.INTEGER)
                    {
                        int value = int.Parse(Console.ReadLine());
                        return new IntData(value);
                    }
                    else
                    {
                        float value = float.Parse(Console.ReadLine().Replace(".", ","));
                        return new FloatData(value);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Неверный ввод, повторите попытку!");
                }
            }
        }

        /// <summary>
        /// Создаёт ячейку данных переменной-приёмника промежуточного результата, сгенерированного транслятором
        /// </summary>
        /// <param name="symbol">Строка таблицы символов</param>
        /// <returns>Ячейку данных программной переменной</returns>
        private static NumericData CreateProgrammData(Symbol symbol)
        {
            if (symbol.Type == IdentifierType.INTEGER)
            {
                return new IntData();
            }
            return new FloatData();
        }

        /// <summary>
        /// Исполняет промежуточный код, сохраняя результаты каждой команды в соответствующую приёмнику ячейку
        /// и печатая каждую операцию в консоль
        /// </summary>
        /// <param name="portableCode">Промежуточный код</param>
        /// <param name="data">Ячейки данных</param>
        private static void ExecuteCode(List<ThreeAddressCommand> portableCode, Dictionary<int, NumericData> data)
        {
            try
            {
                foreach (ThreeAddressCommand command in portableCode)
                {
                    switch (command.Operator.Type)
                    {
                        case TokenType.INT_TO_FLOAT:
                            ExecuteIntToFloatOperation(command, data);
                            break;
                        default:
                            ExecuteBinaryOperation(command, data);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Выполняет бинарную операцию с печатью в консоль
        /// </summary>
        /// <param name="command">Трёхадресная команда</param>
        /// <param name="data">Ячейки данных</param>
        private static void ExecuteBinaryOperation(ThreeAddressCommand command, Dictionary<int, NumericData> data)
        {
            NumericData leftOperand = GetOperand(command.LeftOperand, data);
            NumericData rightOperand = GetOperand(command.RightOperand, data);
            NumericData receiver;
            try
            {
                float result = CalculateOperation(leftOperand, rightOperand, command.Operator.Type);
                receiver = command.Receiver.IdentifierType == IdentifierType.INTEGER ? new IntData((int)result) : (NumericData)new FloatData(result);
                if (!float.IsFinite(result))
                {
                    throw new Exception();
                }
                data[command.Receiver.Attribute] = receiver;
                Console.WriteLine($"{leftOperand} {TokenUtils.GetTokenName(command.Operator.Type)} {rightOperand} = {receiver}");
            }
            catch (Exception)
            {
                throw new Exception($"Ошибка выполнения операции {leftOperand} {TokenUtils.GetTokenName(command.Operator.Type)} {rightOperand}");
            }
        }

        /// <summary>
        /// Получает ячейку данных из операнда
        /// </summary>
        /// <param name="operand">Операнд</param>
        /// <param name="data">Ячейки данных</param>
        /// <returns>Ячейка данных, соответствувющая операнду</returns>
        private static NumericData GetOperand(Token operand, Dictionary<int, NumericData> data)
        {
            if (operand is Identifier identifier)
            {
                return data[identifier.Attribute];
            }
            return GetNumericConstant((NumericConstant)operand);
        }

        /// <summary>
        /// Получает ячейку данных числовой константы
        /// </summary>
        /// <param name="constant">Числовая константа</param>
        /// <returns>Ячейка данных числовой константы</returns>
        private static NumericData GetNumericConstant(NumericConstant constant)
        {
            float value = constant.GetValue();
            if (constant.Type == TokenType.INT_CONSTANT)
            {
                return new IntData((int)value);
            }
            return new FloatData(value);
        }

        /// <summary>
        /// Вычисляет результат бинарной операции
        /// </summary>
        /// <param name="leftOperand">Левый операнд</param>
        /// <param name="rightOperand">Правый операнд</param>
        /// <param name="operatorType">Тип оператора</param>
        /// <returns>Результат операции</returns>
        private static float CalculateOperation(NumericData leftOperand, NumericData rightOperand, TokenType operatorType)
        {
            float leftNumber = (leftOperand is IntData leftInt ? leftInt.Value : ((FloatData)leftOperand).Value);
            float rightNumber = (rightOperand is IntData rightInt ? rightInt.Value : ((FloatData)rightOperand).Value);
            return operatorType switch
            {
                TokenType.ADD => leftNumber + rightNumber,
                TokenType.SUBTRACT => leftNumber - rightNumber,
                TokenType.MULTIPLY => leftNumber * rightNumber,
                _ => leftNumber / (rightOperand is IntData ? (int)rightNumber : rightNumber),
            };
        }

        /// <summary>
        /// Выполняет операцию приведения типа IntToFloat
        /// </summary>
        /// <param name="command">Трёхадресная команда</param>
        /// <param name="data">Ячейки данных</param>
        private static void ExecuteIntToFloatOperation(ThreeAddressCommand command, Dictionary<int, NumericData> data)
        {
            IntData operand = (IntData)GetOperand(command.RightOperand, data);
            FloatData receiver = new FloatData(operand.Value);
            data[command.Receiver.Attribute] = receiver;
            Console.WriteLine($"{operand} -> {receiver}");
        }
    }
}