using System;
using TelegramBotFramework.Core;

namespace TelegramBotFramework.ConsoleApp
{
    public class Program
    {
        static void Main()
        {
            var wr = new TelegramBotWrapper("", 42);     
            Console.WriteLine("Started. Press any key to stop");
            Console.ReadLine();
        }
    }
}
