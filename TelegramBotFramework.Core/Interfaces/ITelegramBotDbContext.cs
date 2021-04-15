using Microsoft.EntityFrameworkCore;
using System;
using TelegramBotFramework.Core.SQLiteDb;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface ITelegramBotDbContext:IDisposable
    {
        DbSet<TelegramBotUser> Users { get; set; }
        DbSet<TelegramBotSetting> Settings { get; set; }
        DbSet<TelegramBotGroup> Groups { get; set; }     
    }
}
