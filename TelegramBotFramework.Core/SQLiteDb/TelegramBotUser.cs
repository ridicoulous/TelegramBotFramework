using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Core.SQLiteDb
{
    public class TelegramBotUser:IEditableEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// The users display name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The users Telegram Id
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// The users @username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// The first time we saw this user
        /// </summary>
        public System.DateTime FirstSeen { get; set; }
        /// <summary>
        /// The last time we saw this user
        /// </summary>
        public System.DateTime LastHeard { get; set; }
        /// <summary>
        /// The number of points this user has
        /// </summary>
        public long Points { get; set; } = 0;
        /// <summary>
        /// Users location, which they can set.  Can be used by various plugins
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// The users debt, which can be used by various plugins
        /// </summary>
        public int Debt { get; set; } = 0;
        /// <summary>
        /// The users last known state (talking, joining a group, leaving a group)
        /// </summary>
        public string LastState { get; set; }
        /// <summary>
        /// The users personal greeting.
        /// </summary>
        public string Greeting { get; set; }
        /// <summary>
        /// Is the user grounded from the bot?
        /// </summary>
        public bool Grounded { get; set; } = false;
        /// <summary>
        /// Who grounded this user?
        /// </summary>
        public string GroundedBy { get; set; }
        /// <summary>
        /// Is the user a bot admin?
        /// </summary>
        public bool IsBotAdmin { get; set; } = false;

        /// <summary>
        /// Is the user activated?
        /// </summary>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// Not in use at this time.  If bot is expanded to other platforms, will be used to link accounts
        /// </summary>
        public string LinkingKey { get; set; }
        /// <summary>
        /// Answer to "who is [user]"
        /// </summary>
        public string Description { get; set; }
        public new string ReadableEntityNameForEditing { get => "Useriq"; set { } }
    }
}
