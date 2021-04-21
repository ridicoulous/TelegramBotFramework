using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.DefaultModules
{
    public class SimpleTelegramBot : TelegramBotWrapper
    {
        public SimpleTelegramBot(ITelegramBotOptions opts) : base(opts)
        {
        }
    }
}
