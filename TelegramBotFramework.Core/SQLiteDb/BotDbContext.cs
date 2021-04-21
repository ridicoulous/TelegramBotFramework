using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public abstract class TelegramBotDbContext : DbContext, ITelegramBotDbContext
    {
        public DbSet<TelegramBotUser> Users { get; set; }
        public DbSet<TelegramBotSetting> Settings { get; set; }
        public DbSet<TelegramBotGroup> Groups { get; set; }

        void ITelegramBotDbContext.SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
