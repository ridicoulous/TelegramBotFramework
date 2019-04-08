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
    public class NewTeleBot : TelegramBotWrapper
    {
        public int sd;
        public NewTeleBot(int val) : base("855824339:AAH8b31siVTDZG_Eu7RMQDz4IF0GPYzK4O0", 166938818, null, "MarketMaking")
        {
            sd = val;
        }
    }
    [TelegramBotModule(Author = "ridicoulous", Name = "MarketMaking", Version = "1.0")]
    public class MarketMakingBotModule : TelegramBotModuleBase
    {
        NewTeleBot _wrapper;
        public MarketMakingBotModule(NewTeleBot wrapper) : base(wrapper)
        {
            _wrapper = wrapper;
        }
        [ChatCommand(Triggers = new[] { "b" }, HideFromInline = true, DontSearchInline = true)]
        public CommandResponse GetConfigs(CommandEventArgs args)
        {
            var balance = int.Parse(args.Parameters)*_wrapper.sd;
            return new CommandResponse($"Your balance is *{balance}*", parseMode: ParseMode.Markdown);
        }
    }
}
