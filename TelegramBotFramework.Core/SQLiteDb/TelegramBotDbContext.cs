﻿//using Microsoft.Data.Sqlite;
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
        public DbSet<TelegramBotUser> TelegramBotUsers { get; set; }
        public DbSet<TelegramBotSetting> TelegramBotSettings { get; set; }
        public DbSet<TelegramBotGroup> TelegramBotGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder modelBuilder)
        {
            base.OnConfiguring(modelBuilder);

            if (InMemory)
            {
                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                keepAliveConnection.Open();
                modelBuilder.UseSqlite(keepAliveConnection);     
                
            }
            else
            {
                modelBuilder.UseSqlite($"Data Source={_db}.db");
            }
        }

        void ITelegramBotDbContext.SaveChanges()
        {
            this.SaveChanges();
        }
    }
}
