using System;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.ConsoleApp
{
    public class Program
    {
        static void Main()
        {
            var wr = new NewTeleBot(42);
            Console.WriteLine("Started. Press any key to stop");
            Console.ReadLine();
        }

    }
    
}
