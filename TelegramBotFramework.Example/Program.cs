using Microsoft.Extensions.Configuration;
using System;
using TelegramBotFramework.Example.SimpleBotExample;

namespace TelegramBotFramework.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
           Befor you start, create your own config file named appsettings.json and fill it with next params:
           {
            "key": "1234445:AAasdasdasdfghghjghjA",
            "admin": 10000000
           }. 
            Set at this file properties Copy to output directory: Copy if newer and run this application.
            TelegramBotWrapper provides a simple bots menue. Just send /menu or /42 and test this examp
            */
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);            
            var configuration = builder.Build();

           

            var bot = new SimpleTelegramBot(configuration["key"], int.Parse(configuration["admin"]));
            bot.Run();
            Console.ReadLine();
        }
    }
}
