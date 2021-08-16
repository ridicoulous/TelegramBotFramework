using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Telegram.Bot.Types;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;
using TelegramBotFramework.Core.SQLiteDb.Extensions;

namespace TelegramBotFramework.Core.Helpers
{
    public static class UserHelper
    {
        public static TelegramBotUser GetTelegramUser(ITelegramBotDbContext db, int adminId, Update update = null, InlineQuery query = null, CallbackQuery cbQuery = null, bool logPoint = true)
        {           
            using(db)
            {
                var from = update?.Message.From ?? query?.From ?? cbQuery?.From;
                if (from == null) return null;
                var u = db.TelegramBotUsers.AsNoTracking().FirstOrDefault(x => x.UserId == from.Id) ?? new TelegramBotUser
                {
                    FirstSeen = DateTime.Now,
                    Points = 0,
                    Debt = 0,
                    IsBotAdmin = false,
                    UserId = from.Id
                };
                u.UserName = from.Username;
                if (query?.Location != null)
                    u.Location = $"{query.Location.Latitude},{query.Location.Longitude}";
                u.Name = (from.FirstName + " " + from.LastName).Trim();
                if (logPoint)
                {
                    var where = update != null ? update.Message.Chat.Title ?? "Private" : "Using inline query";
                    u.LastHeard = DateTime.Now;
                    u.LastState = "talking in " + where;
                    u.Points += update?.Message?.Text?.Length ?? 0 * 10;
                }
                u.Save(db);
                return u;
            }
           
        }

        public static TelegramBotUser GetTarget(this ITelegramBotDbContext db, CommandEventArgs args)
        {
            return  args.Message.GetTarget(args.Parameters, args.SourceUser, db);
        }
    }
}
