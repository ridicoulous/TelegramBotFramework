using Microsoft.Data.Sqlite;
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
                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                keepAliveConnection.Open();
                optionsBuilder.UseSqlite(keepAliveConnection);
                this.Database.EnsureCreated();
            }
            else
            {
                optionsBuilder.UseSqlite($"Data Source={_db}.db");
            }
        }
    }
}
