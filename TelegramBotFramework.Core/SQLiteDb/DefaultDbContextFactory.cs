using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public class DefaultDbContextFactory : IDbContextFactory<TelegramBotDefaultSqLiteDbContext>
    {
        private readonly string _db;
        private readonly bool _inMemory;
        public DefaultDbContextFactory(string dbName, bool inMemory)
        {
            _db = dbName;
            _inMemory = inMemory;
        }
        public TelegramBotDefaultSqLiteDbContext CreateDbContext()
        {
            return new TelegramBotDefaultSqLiteDbContext(_db, _inMemory);
        }
    }
}
