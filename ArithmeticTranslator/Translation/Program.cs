using Translation.Translation;
using TranslationInterpretationLibrary.Exceptions.TranslationExceptions;
using TranslationInterpretationLibrary.Exceptions.TranslationObjectExceptions;
using System;

namespace Translation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                try
                {
                    string filenameInput = args[0];
                    TranslationStage finalStage = TranslationStageUtils.GetTranslationStage(args[1]);
                    string[] otherArgs = new string[args.Length - 2];
                    Array.Copy(args, 2, otherArgs, 0, args.Length - 2);
                    Translator.Translate(filenameInput, finalStage, otherArgs);
                }
                catch (TranslationException e)
                {
                    Console.WriteLine(e.GetMessageWithType());
                }
                catch (InvalidTranslationObjectArgumentException e)
                {
                    Console.WriteLine(e.GetMessageWithType());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine("Трансляция завершена.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Неверное количество входных параметров!");
            Console.ReadLine();
        }
    }
}