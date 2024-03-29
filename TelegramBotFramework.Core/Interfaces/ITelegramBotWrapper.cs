﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface ITelegramBotWrapper<TDbContext> : ITelegramBotWrapper
        where TDbContext : DbContext, ITelegramBotDbContext
    {
        new TDbContext Db { get; }
    }
    public interface ITelegramBotWrapper
    {
        IEnumerable<ChatCommand> ChatCommands { get;  }
        //Dictionary<CallbackCommand, CallbackCommandMethod> CallbackCommands { get; }
      
        Dictionary<TelegramBotModule, object> Modules { get; }
        DbContext Db { get; }
        
        ConcurrentDictionary<long, KeyValuePair<Type, IEditableEntity>> UserEditingEntity { get; set; }
        void Run();
        void SeedBotAdmins(params int[] adminIds);
        Dictionary<long, Queue<SurveyAttribute>> UsersWaitingAnswers { get; set; }
        UsersSurveys CurrentUserUpdatingObjects { get; set; }
        bool IsSurveyInitiated { get; set; }
        TelegramBotClient Bot { get; }
        void SendMessageToAll(string message, bool onlyAdmins = false, bool onlydev = true, bool isSilent = false);
        void SendMessage(string message, long userId, bool isSilent);
        Task SendMessageAsync(string message, long userId, bool isSilent);

    }
}
