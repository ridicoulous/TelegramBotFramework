using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotFramework.Core.SQLiteDb.Extensions
{
    public static class TelegramBotDbExtensions
    {
        #region Users

        public static void Save(this TelegramBotUser u, TelegramBotDbContext db)
        {
            if (u.ID == null || !ExistsInDb(u, db))
            {
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Users.Add(u);
                        db.SaveChanges();
                        u.ID = db.Users.FirstOrDefault(c => c.UserId == u.UserId).ID;
                    }
                    catch { }
                }
               
                //db.Database.ExecuteSqlCommand(
                //    "insert into users (Name, UserId, UserName, FirstSeen, LastHeard, Points, Location, Debt, LastState, Greeting, Grounded, GroundedBy, IsBotAdmin, LinkingKey, Description) VALUES (@Name, @UserId, @UserName, @FirstSeen, @LastHeard, @Points, @Location, @Debt, @LastState, @Greeting, @Grounded, @GroundedBy, @IsBotAdmin, @LinkingKey, @Description)",
                //    u);
                //u.ID =
                //    db.Database.Query<int>(
                //        $"SELECT ID FROM Users WHERE UserId = @UserId", u)
                //        .First();
            }
            else
            {
                //using(var tx = db.Database.BeginTransaction())
                //{
                //    try
                //    {
                //        db.Users.Update(u);
                //        db.SaveChanges();
                //        tx.Commit();
                //    }
                //    catch { }
                //}
                
                //db.ExecuteNonQuery(
                //    "UPDATE users SET Name = @Name, UserId = @UserId, UserName = @UserName, FirstSeen = @FirstSeen, LastHeard = @LastHeard, Points = @Points, Location = @Location, Debt = @Debt, LastState = @LastState, Greeting = @Greeting, Grounded = @Grounded, GroundedBy = @GroundedBy, IsBotAdmin = @IsBotAdmin, LinkingKey = @LinkingKey, Description = @Description WHERE ID = @ID",
                //    u);
            }
        }

        public static bool ExistsInDb(this TelegramBotUser user, TelegramBotDbContext db)
        {
            return db.Users.AsNoTracking().Any(i => i.ID == user.ID);
        }

        public static void RemoveFromDb(this TelegramBotUser user, TelegramBotDbContext db)
        {
            db.Users.Remove(user);
            db.SaveChanges();
        }

        /// <summary>
        /// Gets a user setting from the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="user">What user the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database TelegramBotDbContext</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static T GetSetting<T>(this TelegramBotUser user, string field, TelegramBotDbContext db, object def)
        {
            //if (db.Connection.State != ConnectionState.Open)
            //    db.Connection.Open();
            ////verify settings exist
            //var columns = new SQLiteCommand("PRAGMA table_info(users)", db.Connection).ExecuteReader();
            //var t = default(T);

            //while (columns.Read())
            //{
            //    if (String.Equals(columns[1].ToString(), field))
            //    {
            //        var result = new SqliteCommand($"select {field} from users where ID = {user.ID}", db.Connection).ExecuteScalar();
            //        if (t != null && t.GetType() == typeof(bool))
            //        {
            //            result = (result.ToString() == "1"); //make it a boolean value
            //        }
            //        return (T)result;
            //    }
            //}
            //var type = "BLOB";
            //if (t == null)
            //    type = "TEXT";
            //else if (t.GetType() == typeof(int))
            //    type = "INTEGER";
            //else if (t.GetType() == typeof(bool))
            //    type = "INTEGER";

            //new SqliteCommand($"ALTER TABLE users ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
            //    .ExecuteNonQuery();
            //return (T)def;
            throw new NotImplementedException();

        }

        /// <summary>
        /// Sets a user setting to the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="user">What user the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database TelegramBotDbContext</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static bool SetSetting<T>(this TelegramBotUser user, string field, TelegramBotDbContext db, object def, object value)
        {
            //try
            //{
            //    if (db.Connection.State != ConnectionState.Open)
            //        db.Connection.Open();
            //    //verify settings exist
            //    var columns = new SQLiteCommand("PRAGMA table_info(users)", db.Connection).ExecuteReader();
            //    var t = default(T);
            //    var type = "BLOB";
            //    if (t == null)
            //        type = "TEXT";
            //    else if (t.GetType() == typeof(int))
            //        type = "INTEGER";
            //    else if (t.GetType() == typeof(bool))
            //        type = "INTEGER";
            //    bool settingExists = false;
            //    while (columns.Read())
            //    {
            //        if (String.Equals(columns[1].ToString(), field))
            //        {
            //            settingExists = true;
            //        }
            //    }
            //    if (!settingExists)
            //    {
            //        new SQLiteCommand($"ALTER TABLE users ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
            //            .ExecuteNonQuery();
            //    }

            //    new SQLiteCommand($"UPDATE users set {field} = {(type == "INTEGER" ? (t != null && t.GetType() == typeof(bool)) ? (bool)value ? "1" : "0" : value : $"'{value}'")} where ID = {user.ID}", db.Connection).ExecuteNonQuery();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
            throw new NotImplementedException();

        }

        #endregion

        #region Groups

        public static void Save(this TelegramBotGroup u, TelegramBotDbContext db)
        {
            if (u.ID == null || !ExistsInDb(u, db))
            {
                //need to insert
                //db.ExecuteNonQuery(
                //    "insert into chatgroup (GroupId, Name, UserName, MemberCount) VALUES (@GroupId, @Name, @UserName, @MemberCount)",
                //    u);
                //u.ID =
                //    db.Connection.Query<int>(
                //        $"SELECT ID FROM chatgroup WHERE GroupId = @GroupId", u)
                //        .First();
                db.Groups.Add(u);
                db.SaveChanges();
                u.ID = db.Groups.FirstOrDefault(c => c.GroupId == u.GroupId).ID;
            }
            else
            {
                db.Groups.Update(u);
                db.SaveChanges();
                //db.ExecuteNonQuery(
                //    "UPDATE chatgroup SET GroupId = @GroupId, Name = @Name, UserName = @UserName, MemberCount = @MemberCount WHERE ID = @ID",
                //    u);
            }
        }

        public static bool ExistsInDb(this TelegramBotGroup group, TelegramBotDbContext db)
        {
            return db.Groups.Any(c => c.ID == group.ID);
        }

        public static void RemoveFromDb(this TelegramBotGroup group, TelegramBotDbContext db)
        {
            db.Groups.Remove(group);
            db.SaveChanges();
        }

        /// <summary>
        /// Gets a group setting from the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="group">What group the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database TelegramBotDbContext</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static T GetSetting<T>(this TelegramBotGroup group, string field, TelegramBotDbContext db, object def)
        {
            //if (db.Connection.State != ConnectionState.Open)
            //    db.Connection.Open();
            ////verify settings exist
            //var columns = new SQLiteCommand("PRAGMA table_info(chatgroup)", db.Connection).ExecuteReader();
            //var t = default(T);
            //while (columns.Read())
            //{
            //    if (String.Equals(columns[1].ToString(), field))
            //    {
            //        var result = new SQLiteCommand($"select {field} from chatgroup where ID = {group.ID}", db.Connection).ExecuteScalar();
            //        if (t != null && t.GetType() == typeof(bool))
            //        {
            //            result = (result.ToString() == "1"); //make it a boolean value
            //        }
            //        return (T)result;
            //    }
            //}
            //var type = "BLOB";
            //if (t == null)
            //    type = "TEXT";
            //else if (t.GetType() == typeof(int))
            //    type = "INTEGER";
            //else if (t.GetType() == typeof(bool))
            //    type = "INTEGER";
            //new SQLiteCommand($"ALTER TABLE chatgroup ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
            //    .ExecuteNonQuery();
            //return (T)def;
            throw new NotImplementedException();

        }

        /// <summary>
        /// Gets a group setting from the database
        /// </summary>
        /// <typeparam name="T">The type the setting should be (bool, int, string)</typeparam>
        /// <param name="group">What group the setting comes from</param>
        /// <param name="field">The name of the setting</param>
        /// <param name="db">The database TelegramBotDbContext</param>
        /// <param name="def">The default value for the field</param>
        /// <returns></returns>
        public static bool SetSetting<T>(this TelegramBotGroup group, string field, TelegramBotDbContext db, object def, object value)
        {
            //try
            //{
            //    if (db.Connection.State != ConnectionState.Open)
            //        db.Connection.Open();
            //    //verify settings exist
            //    var columns = new SQLiteCommand("PRAGMA table_info(chatgroup)", db.Connection).ExecuteReader();
            //    var t = default(T);
            //    var type = "BLOB";
            //    if (t == null)
            //        type = "TEXT";
            //    else if (t.GetType() == typeof(int))
            //        type = "INTEGER";
            //    else if (t.GetType() == typeof(bool))
            //        type = "INTEGER";
            //    bool settingExists = false;
            //    while (columns.Read())
            //    {
            //        if (String.Equals(columns[1].ToString(), field))
            //        {
            //            settingExists = true;
            //        }
            //    }
            //    if (!settingExists)
            //    {
            //        new SQLiteCommand($"ALTER TABLE chatgroup ADD COLUMN {field} {type} DEFAULT {(type == "INTEGER" ? def : $"'{def}'")};", db.Connection)
            //            .ExecuteNonQuery();
            //    }

            //    new SQLiteCommand($"UPDATE chatgroup set {field} = {(type == "INTEGER" ? (t != null && t.GetType() == typeof(bool)) ? (bool)value ? "1" : "0" : value : $"'{value}'")} where ID = {group.ID}", db.Connection).ExecuteNonQuery();

            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
            throw new NotImplementedException();
        }

        #endregion

        #region Settings

        public static void Save(this TelegramBotSetting set, TelegramBotDbContext db)
        {
            if (set.ID == null || !ExistsInDb(set, db))
            {
                //need to insert
                //db.ExecuteNonQuery(
                //    "insert into settings (Alias, TelegramBotAPIKey, TelegramDefaultAdminUserId) VALUES (@Alias, @TelegramBotAPIKey, @TelegramDefaultAdminUserId)",
                //    set);
                //set.ID = db.Connection.Query<int>("SELECT ID FROM Settings WHERE Alias = @Alias", set).First();
                db.Settings.Add(set);
                db.SaveChanges();
                set.ID = db.Settings.FirstOrDefault(c => c.Alias == set.Alias).ID;
            }
            else
            {
                //db.ExecuteNonQuery(
                //    "UPDATE settings SET Alias = @Alias, TelegramBotAPIKey = @TelegramBotAPIKey, TelegramDefaultAdminUserId = @TelegramDefaultAdminUserId WHERE ID = @ID",
                //    set);
                db.Settings.Update(set);
                db.SaveChanges();
            }
        }

        public static bool ExistsInDb(this TelegramBotSetting set, TelegramBotDbContext db)
        {
            return db.Settings.Any(c => c.ID == set.ID);
        }

        public static void RemoveFromDb(this TelegramBotSetting set, TelegramBotDbContext db)
        {
            db.Settings.Remove(set);
            db.SaveChanges();
        }

        /// <summary>
        /// Adds a field to the settings table, if needed
        /// </summary>
        /// <param name="set">the current settings loaded</param>
        /// <param name="db">TelegramBotDbContext of the database</param>
        /// <param name="field">Name of the field you need</param>
        /// <returns>Whether or not the field was missing / was added</returns>
        public static bool AddField(this TelegramBotSetting set, TelegramBotDbContext db, string field)
        {
            //if (db.Connection.State != ConnectionState.Open)
            //    db.Connection.Open();
            ////verify settings exist
            //var columns = new SQLiteCommand("PRAGMA table_info(settings)", db.Connection).ExecuteReader();
            //var settingExists = false;
            //while (columns.Read())
            //{
            //    if (String.Equals(columns[1].ToString(), field))
            //        settingExists = true;
            //}

            //if (!settingExists)
            //{
            //    new SQLiteCommand($"ALTER TABLE settings ADD COLUMN {field} TEXT DEFAULT '';", db.Connection)
            //        .ExecuteNonQuery();
            //    return true;
            //}

            //return false;
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the requested field from settings.
        /// </summary>
        /// <param name="set"></param>
        /// <param name="db"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetString(this TelegramBotSetting set, TelegramBotDbContext db, string field)
        {
            throw new NotImplementedException();

            //return
            //    db.Connection.Query<string>($"select {field} from settings where Alias = '{set.Alias}'")
            //        .FirstOrDefault();
        }

        public static void SetString(this TelegramBotSetting set, TelegramBotDbContext db, string field, string value)
        {
            throw new NotImplementedException();

            //new SQLiteCommand($"Update settings set {field} = '{value}' WHERE Alias = '{set.Alias}'", db.Connection)
            //    .ExecuteNonQuery();
        }

        #endregion

        public static string ExecuteQuery(this TelegramBotDbContext db, string commandText, object param = null)
        {
            // Ensure we have a connection
            //if (db.Connection == null)
            //{
            //    throw new NullReferenceException(
            //        "Please provide a connection");
            //}

            //// Ensure that the connection state is Open
            //if (db.Connection.State != ConnectionState.Open)
            //{
            //    db.Connection.Open();
            //}
            //var reader = db.Connection.ExecuteReader(commandText, param);
            //var response = "";
            //for (int i = 0; i < reader.FieldCount; i++)
            //{
            //    response += $"{reader.GetName(i)} - ";
            //}
            //response += "\n";
            //while (reader.Read())
            //{
            //    for (int i = 0; i < reader.FieldCount; i++)
            //    {
            //        response += $"{reader[i]} - ";
            //    }
            //    response += "\n";
            //}
            //// Use Dapper to execute the given query
            //return response;
            throw new NotImplementedException();

        }

        public static int ExecuteNonQuery(this TelegramBotDbContext db, string commandText, object param = null)
        {
            // Ensure we have a connection
            //if (db.Connection == null)
            //{
            //    throw new NullReferenceException(
            //        "Please provide a connection");
            //}

            //// Ensure that the connection state is Open
            //if (db.Connection.State != ConnectionState.Open)
            //{
            //    db.Connection.Open();
            //}

            //// Use Dapper to execute the given query
            //return db.Connection.Execute(commandText, param);
            throw new NotImplementedException();

        }

        public static string ToString(this Telegram.Bot.Types.User user)
        {
            return (user.FirstName + " " + user.LastName).Trim();
        }

        #region Helpers
        public static TelegramBotUser GetTarget(this Message message, string args, TelegramBotUser sourceUser, TelegramBotDbContext db)
        {          
            if (message == null) return sourceUser;
            if (message?.ReplyToMessage != null)
            {
                var m = message.ReplyToMessage;
                var userid = m.ForwardFrom?.Id ?? m.From.Id;
                return db.Users.AsNoTracking().FirstOrDefault(x => x.UserId == userid) ?? sourceUser;
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
                result = db.Users.AsNoTracking().FirstOrDefault(
                    x => x.UserName.ToUpper() == username.ToUpper());
            else if (id != 0)
                result = db.Users.AsNoTracking().FirstOrDefault(x => x.UserId == id);
            else
                result = db.Users.AsNoTracking().ToList().FirstOrDefault(
                        x =>
                            String.Equals(x.UserId.ToString(), args, StringComparison.InvariantCultureIgnoreCase) ||
                            String.Equals(x.UserName, args.Replace("@", ""), StringComparison.InvariantCultureIgnoreCase));
            return result ?? sourceUser;
        }
        #endregion
    }
}
