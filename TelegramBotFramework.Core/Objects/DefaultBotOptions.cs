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
        public DefaultBotOptions(string key, long adminId)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            AdminId = adminId;
        }

        public string Key {get;set;}
        public string Alias { get; set; } = "TelegramBot";
        public long AdminId {get;set;}
        public bool ShouldApprooveNewUsers { get; set; } = false;
        public string PaymentToken {get;set;}
        public string Directory { get; set; } = "TelegramBot";
        public string WebHookUrl {get;set;}
        public bool InMemoryDb { get; set; } = false;
        /// <summary>
        /// Write all internal logs to a file instead of only printing them on the console screen
        /// </summary>
        public bool FileLog { get; set; } = true;
        /// <summary>
        /// Allows external modules and scans for them in the AddonModules directory
        /// </summary>
        public bool AddonModules { get; set; } = true;
        /// <summary>
        /// Enables default modules such as Admin and DefaultBotModule
        /// </summary>
        public bool DefaultModules { get; set; } = true;
    }
}
