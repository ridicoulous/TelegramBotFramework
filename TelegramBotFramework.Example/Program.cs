using Microsoft.Extensions.Configuration;
using System;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Example.SimpleBotExample;

namespace TelegramBotFramework.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
           Before you start, create your own config file named appsettings.json and fill it with next params:
           {
            "key": "1234445:AAasdasdasdfghghjghjA",
            "admin": 10000000
           }. 
            Set at this file properties Copy to output directory: Copy if newer and run this application.
            TelegramBotWrapper provides a simple bots menue. Just send /menu or /42 and test this examp
            */
            //var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //var configuration = builder.Build();

            var opts = new DefaultBotOptions("1685679177:AAF6nqRBkfpOu8aWq_pqMu3ZKXjwlkska48", 166938818);


            var bot = new SimpleTelegramBot(opts);
            bot.Run();
            Console.ReadLine();
        }
    }
}
