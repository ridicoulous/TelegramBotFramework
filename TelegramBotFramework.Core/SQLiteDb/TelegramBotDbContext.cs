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
        private readonly string _db;
        public readonly bool InMemory;
        public TelegramBotDefaultSqLiteDbContext(string dbName, bool inMemory)
        {
            _db = dbName;
            InMemory = inMemory;
        }
        public DbSet<TelegramBotUser> Users { get; set; }
        public DbSet<TelegramBotSetting> Settings { get; set; }
        public DbSet<TelegramBotGroup> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (InMemory)
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

        void ITelegramBotDbContext.SaveChanges()
        {
            this.SaveChanges();
        }
    }
}
