using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.Objects
{
    public class DefaultBotOptions : ITelegramBotOptions
    {
        public DefaultBotOptions(string key, int adminId)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            AdminId = adminId;
        }

        public string Key {get;set;}
        public string Alias { get; set; } = "TelegramBot";
        public int AdminId {get;set;}
        public bool ShouldApprooveNewUsers { get; set; } = false;
        public string PaymentToken {get;set;}
        public string Directory { get; set; } = "TelegramBot";
        public string WebHookUrl {get;set;}
        public bool InMemoryDb { get; set; } = false;
    }
}
