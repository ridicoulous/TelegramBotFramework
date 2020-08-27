using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.IO;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public class TelegramBotDbContext : DbContext
    {
        public readonly string _db;
        private readonly bool _inMemory;
        public TelegramBotDbContext(string dbName)
        {
            _db = dbName;
            _inMemory = false;
        }
        public TelegramBotDbContext()
        {
            _db = "telegrambot";
            _inMemory = true;
        }
        public DbSet<TelegramBotUser> Users { get; set; }
        public DbSet<TelegramBotSetting> Settings { get; set; }
        public DbSet<TelegramBotGroup> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_inMemory)
            {
                optionsBuilder.UseSqlite($"Data Source=:memory:");
            }
            else
            {
                optionsBuilder.UseSqlite($"Data Source={_db}.db");
            }
        }
    }
}
