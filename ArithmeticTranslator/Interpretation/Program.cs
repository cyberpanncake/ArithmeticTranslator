using Interpretation.Interpretation;
using System;

namespace Interpretation
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                try
                {
                    string filenameInput = args[0];
                    Interpretator.Execute(filenameInput);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Неверное количество входных параметров!");
            Console.ReadLine();
        }
    }
}