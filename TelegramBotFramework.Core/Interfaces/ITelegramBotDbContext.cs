using Microsoft.EntityFrameworkCore;
using System;
using TelegramBotFramework.Core.SQLiteDb;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface ITelegramBotDbContext:IDisposable
    {
        DbSet<TelegramBotUser> TelegramBotUsers { get; set; }
        DbSet<TelegramBotSetting> TelegramBotSettings { get; set; }
        DbSet<TelegramBotGroup> TelegramBotGroups { get; set; }
        void SaveChanges();
    }
}
