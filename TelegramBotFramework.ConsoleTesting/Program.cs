using System;
using TelegramBotFramework.Core;

namespace TelegramBotFramework.ConsoleApp
{
    public class Program
    {
        static void Main()
        {
            var wr = new TelegramBotWrapper("", 32, "42");
            wr.Bot.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(42), "asdasd");
            Console.WriteLine("Started. Press any key to stop");
            Console.ReadLine();
        }
    }
}
