using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public class TelegramBotDbContext : DbContext
    {
        public DbSet<TelegramBotUser> Users { get; set; }
        public DbSet<TelegramBotSetting> Settings { get; set; }
        public DbSet<TelegramBotGroup> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=telegrambot.db");
        }
    }
}
