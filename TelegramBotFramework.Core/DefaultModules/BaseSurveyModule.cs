using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.DefaultModules;

namespace TelegramBotFramework.Core.DefaultModules
{
    
    public abstract class BaseSurveyModule<TSurvey, TBotWrapper> : TelegramBotModuleBase<TBotWrapper> where TBotWrapper : ITelegramBotWrapper where TSurvey : class, new()
    {
        public BaseSurveyModule(TBotWrapper wrapper) : base(wrapper)
        {
         
            wrapper.OnSurveyComplete += Wrapper_OnSurveyComplete;
        }

        private void Wrapper_OnSurveyComplete(long userid)
        {
            this.SubmitSurvey(BotWrapper.CurrentUserUpdatingObjects.GetValue<TSurvey>(userid));     
        }
    
        public virtual CommandResponse CreateSurvay(CommandEventArgs args)
        {
            return BotWrapper.InitServey<TSurvey>(args.SourceUser.UserId);
        }
        public abstract void SubmitSurvey(TSurvey survey);

    }
}
