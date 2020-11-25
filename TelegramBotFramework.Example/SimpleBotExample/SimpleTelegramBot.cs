using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using TelegramBotFramework.Core;

namespace TelegramBotFramework.Example.SimpleBotExample
{
    public class SimpleTelegramBot : TelegramBotWrapper
    {
        public SimpleTelegramBot(string key, int adminId,  IServiceProvider serviceProvider = null, string alias = "TelegramBotFramework", bool needNewUserApproove = false, string paymentToken = null, string dir = "", string webHookUrl = null, bool shouldUseInMemoryDb = false) : base(key, adminId, null, serviceProvider, alias, needNewUserApproove, paymentToken, dir, webHookUrl, shouldUseInMemoryDb)
        {
        }
    }
}
