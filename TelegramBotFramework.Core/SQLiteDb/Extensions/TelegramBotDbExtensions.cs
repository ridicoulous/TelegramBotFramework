using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.SQLiteDb.Extensions
{
    public static class TelegramBotDbExtensions
    {
        #region Users

        public static void Save(this TelegramBotUser u, ITelegramBotDbContext db)
        {
            if (u.Id == 0 || !ExistsInDb(u, db))
            {
                try
                {
                    db.TelegramBotUsers.Add(u);
                    db.SaveChanges();
                    u.Id = db.TelegramBotUsers.FirstOrDefault(c => c.UserId == u.UserId).Id;
                }
                catch { }
            }
            else
            {
                try
                {
                   // db.Users.Update(u);
                 //   db.SaveChanges();
                }
                catch(Exception e)
                {
                
                }
            }
        }

        public static bool ExistsInDb(this TelegramBotUser user, ITelegramBotDbContext db)
        {
            return db.TelegramBotUsers.AsNoTracking().Any(i => i.Id == user.Id);
        }

        public static void RemoveFromDb(this TelegramBotUser user, ITelegramBotDbContext db)
        {
            db.TelegramBotUsers.Remove(user);
            db.SaveChanges();
        }

  
        #endregion

        #region Groups

        public static void Save(this TelegramBotGroup u, ITelegramBotDbContext db)
        {
            if (u.ID == null || !ExistsInDb(u, db))
            {                
                db.TelegramBotGroups.Add(u);
                db.SaveChanges();
                u.ID = db.TelegramBotGroups.FirstOrDefault(c => c.GroupId == u.GroupId).ID;
            }
            else
            {
                db.TelegramBotGroups.Update(u);
                db.SaveChanges();               
            }
        }

        public static bool ExistsInDb(this TelegramBotGroup group, ITelegramBotDbContext db)
        {
            return db.TelegramBotGroups.Any(c => c.ID == group.ID);
        }

        public static void RemoveFromDb(this TelegramBotGroup group, ITelegramBotDbContext db)
        {
            db.TelegramBotGroups.Remove(group);
            db.SaveChanges();
        }

       

        #endregion

        #region Settings

        public static void Save(this TelegramBotSetting set, ITelegramBotDbContext db)
        {
            if (set.Id == 0 || !ExistsInDb(set, db))
            {             
                db.TelegramBotSettings.Add(set);
                db.SaveChanges();
                set.Id = db.TelegramBotSettings.FirstOrDefault(c => c.Alias == set.Alias).Id;
            }
            else
            {               
                db.TelegramBotSettings.Update(set);
                db.SaveChanges();
            }
        }

        public static bool ExistsInDb(this TelegramBotSetting set, ITelegramBotDbContext db)
        {
            return db.TelegramBotSettings.Any(c => c.Id == set.Id);
        }

        public static void RemoveFromDb(this TelegramBotSetting set, ITelegramBotDbContext db)
        {
            db.TelegramBotSettings.Remove(set);
            db.SaveChanges();
        }

       

        #endregion

       

     

        public static string ToString(this Telegram.Bot.Types.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim();
        }

        #region Helpers
        public static TelegramBotUser GetTarget(this Message message, string args, TelegramBotUser sourceUser, ITelegramBotDbContext db)
        {
            if (message == null) return sourceUser;
            if (message?.ReplyToMessage != null)
            {
                var m = message.ReplyToMessage;
                var userid = m.ForwardFrom?.Id ?? m.From.Id;
                return db.TelegramBotUsers.AsNoTracking().FirstOrDefault(x => x.UserId == userid) ?? sourceUser;
            }
            if (String.IsNullOrWhiteSpace(args))
            {
                return sourceUser;
            }
            //check for a user mention
            var mention = message?.Entities.FirstOrDefault(x => x.Type == MessageEntityType.Mention);
            var textmention = message?.Entities.FirstOrDefault(x => x.Type == MessageEntityType.TextMention);
            var id = 0;
            var username = "";
            if (mention != null)
                username = message.Text.Substring(mention.Offset + 1, mention.Length - 1);
            else if (textmention != null)
            {
                id = textmention.User.Id;
            }
            TelegramBotUser result = null;
            if (!String.IsNullOrEmpty(username))
                result = db.TelegramBotUsers.AsNoTracking().FirstOrDefault(
                    x => x.UserName.ToUpper() == username.ToUpper());
            else if (id != 0)
                result = db.TelegramBotUsers.AsNoTracking().FirstOrDefault(x => x.UserId == id);
            else
                result = db.TelegramBotUsers.AsNoTracking().ToList().FirstOrDefault(
                        x =>
                            String.Equals(x.UserId.ToString(), args, StringComparison.InvariantCultureIgnoreCase) ||
                            String.Equals(x.UserName, args.Replace("@", ""), StringComparison.InvariantCultureIgnoreCase));
            return result ?? sourceUser;
        }
        #endregion
    }
}
