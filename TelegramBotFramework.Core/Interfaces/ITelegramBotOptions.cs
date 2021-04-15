using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface ITelegramBotOptions
    {
        string Key { get; set; }
        string Alias { get; set; }
        int AdminId { get; set; }
        bool ShouldApprooveNewUsers { get; set; }
        string PaymentToken { get; set; }
        string Directory { get; set; }
        string WebHookUrl { get; set; }
        bool InMemoryDb { get; set; }
    }
}
