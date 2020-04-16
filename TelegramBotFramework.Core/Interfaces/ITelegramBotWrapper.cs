using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface ITelegramBotWrapper
    {
        void Run();
        bool AnswerHandling { get; set; } 
        Dictionary<long, Queue<SurveyAttribute>> UsersWaitingAnswers { get; set; }
        UsersSurveys CurrentUserUpdatingObjects { get; set; }
        bool IsSurveyInitiated { get; set; }
        TelegramBotClient Bot { get; set; }
        void SendMessageToAll(string message, bool onlyAdmins = false, bool onlydev=true, bool isSilent=false);
        void SendMessage(string message, long userId, bool isSilent);
        Task SendMessageAsync(string message, long userId, bool isSilent);


    }
}
