using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface IBaseSurveyModule<TSurvey> : IBaseSurveyModule where TSurvey : class, new()
    {
        CommandResponse InitServey<T>(long userId) where T : class, new(); 
        void SubmitSurvey(TSurvey survey);
    }
    public interface IBaseSurveyModule
    {
        CommandResponse GetAnswer(Message message);
        bool HandleResponse(Message message);  
        CommandResponse SendQuestion(long userId, List<SurveyAttribute> questions = null);    
    }
}
