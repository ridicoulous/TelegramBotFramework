using System.Linq;
using Telegram.Bot.Types;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.SQLiteDb;
using TelegramBotFramework.Core.SQLiteDb.Extensions;

namespace TelegramBotFramework.Core.Helpers
{
    public static class GroupHelper
    {
        public static TelegramBotGroup GetGroup(ITelegramBotDbContext db, Update update = null)
        {
            var from = update?.Message?.Chat;
            if (from == null) return null;
            var u = db.Groups.FirstOrDefault(c => c.ID == from.Id) ?? new TelegramBotGroup
            {
                GroupId = from.Id
            };
            u.Name = from.Title;
            u.UserName = from.Username;
            u.Save(db);
            return u;
        }
    }
}
