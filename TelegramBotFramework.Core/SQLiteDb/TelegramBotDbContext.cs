//using Microsoft.Data.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.IO;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public class TelegramBotDefaultSqLiteDbContext : DbContext, ITelegramBotDbContext
    {
        public readonly string _db;
        private readonly bool _inMemory;
        public TelegramBotDefaultSqLiteDbContext(string dbName, bool inMemory)
        {
            _db = dbName;
            _inMemory = inMemory;
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
            }
            else
            {
               optionsBuilder.UseSqlite($"Data Source={_db}.db");
            }
        }
    }
}
