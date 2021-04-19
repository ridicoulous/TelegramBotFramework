using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Example.SimpleBotExample
{
    public class SimpleTelegramBot : TelegramBotWrapper
    {
        public SimpleTelegramBot(ITelegramBotOptions opts) : base(opts)
        {
        }
    }
}
