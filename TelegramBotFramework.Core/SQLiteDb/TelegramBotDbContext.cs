using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.IO;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public class TelegramBotDbContext : DbContext
    {
        private readonly string _db;    

        public TelegramBotDbContext(string dbName)
        {
            _db = dbName;        
        }
        public DbSet<TelegramBotUser> Users { get; set; }
        public DbSet<TelegramBotSetting> Settings { get; set; }
        public DbSet<TelegramBotGroup> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_db}.db");
        }
    }
}
