using System;
using System.Linq;
using Telegram.Bot.Types;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;
using TelegramBotFramework.Core.SQLiteDb.Extensions;

namespace TelegramBotFramework.Core.Helpers
{
    public static class UserHelper
    {
        public static TelegramBotUser GetTelegramUser(TelegramBotDbContext db, Update update = null, InlineQuery query = null, CallbackQuery cbQuery = null, bool logPoint = true)
        {
            var users = db.Users.ToList();
            var from = update?.Message.From ?? query?.From ?? cbQuery?.From;
            if (from == null) return null;
            var u = db.Users.FirstOrDefault(x => x.UserId == from.Id) ?? new TelegramBotUser
            {
                FirstSeen = DateTime.Now,
                Points = 0,
                Debt = 0,
                IsBotAdmin = false, //Program.LoadedSetting.TelegramDefaultAdminUserId == from.Id,
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
                u.Points += update?.Message.Text.Length ?? 0 * 10;
            }
            u.Save(db);
            return u;
        }

        public static TelegramBotUser GetTarget(CommandEventArgs args)
        {
            return args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
        }

        //public static CommandResponse LinkUser(Instance db, DB.Models.User usr, Update update)
        //{
        //    //get the linking key
        //    try
        //    {
        //        var key = update.Message.Text.Split(' ')[1];
        //        var u = db.Users.FirstOrDefault(x => x.LinkingKey == key);
        //        u.TelegramUserID = update.Message.From.Username;
        //        u.LinkingKey = null;
        //        u.Save(db);
        //        MergeUsers(db, u, usr);
        //        return new CommandResponse("Account linked.  Welcome " + u.Nick);
        //    }
        //    catch
        //    {
        //        return new CommandResponse("Unable to verify your account.");
        //    }
        //}



        //public static DB.Models.User MergeUsers(Instance db, DB.Models.User ircUser, DB.Models.User telegramUser)
        //{
        //    ircUser.TelegramUserID = telegramUser.TelegramUserID;
        //    ircUser.LinkingKey = null;
        //    ircUser.Points += telegramUser.Points;
        //    ircUser.Debt += telegramUser.Debt;
        //    ircUser.LastHeard = telegramUser.LastHeard;
        //    ircUser.LastState = telegramUser.LastState;
        //    telegramUser.RemoveFromDb(db);
        //    ircUser.Save(db);
        //    return ircUser;
        //}
    }
}
