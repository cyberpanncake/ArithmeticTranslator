# ArithmeticTranslator

Транслятор и интерпретатор простых арифметических выражений.

Арифметические выражения задаются в файле, расположенном рядом с .exe. Выражения могут включать:
1. Операции +, -(бинарный), *, /
2. Числа - простые и вещественные (разделитель - точка)
3. Переменные, заданные по правилам именования переменных в C#. Можно указать тип переменной:
* Целый - "var" (по умолчанию), "var[i]", "var[I]"
* Вещественный - "var[f]", "var[F]"
4. Круглые скобки

Транслятор реализует все основные стадии трансляции:

(input.txt - файл с входным выражением, принимаемый на вход всех стадий трансляции. Остальные файлы, указанные в параметрах, являются выходными и генерируются программой)

1. Лексический анализ

cmd> Translation.exe input.txt lex tokens.txt symbols.txt

tokens.txt - последовательность токенов выражения

symbols.txt - таблица символов

2. Синтаксический анализ

cmd> Translation.exe input.txt syn tree.txt

tree.txt - синтаксическое дерево

3. Семантический анализ

cmd> Translation.exe input.txt sem tree_mod.txt

tree_mod.txt - модифицированное синтаксическое дерево (добавлены операции приведения типа, там где это нужно, проверка деления на 0)

4.1. Генерация промежуточного кода

cmd> Translation.exe input.txt gen1 [opt] code.txt symbols.txt

code.txt - трёхадресный код (формат кода - <код_операции> <результат> <операнд1> [<операнд2>])

symbols.txt - таблица символов

4.2. Генерация постфиксной записи

cmd> Translation.exe input.txt gen2 [opt] postfix.txt symbols.txt

postfix.txt - постфиксная запись выражения

symbols.txt - таблица символов

В 4.1. и 4.2 параметр opt дополнительно производит оптимизацию вычислений

4.3. Генерация бинарного формата промежуточного кода

cmd> Translation.exe input.txt gen3 code.bin symbols.txt

code.bin - бинарный формат промежуточного кода

symbols.txt - таблица символов

Интерпретатор принимает на вход файл с промежуточным кодом в бинарном формате и выполняет его (запрашивает у пользователя значения переменных и вычисляет результат арифметического выражения).

cmd> Interpretation.exe code.bin

code.bin - входной файл с кодом, полученный в режиме gen3 транслятора
