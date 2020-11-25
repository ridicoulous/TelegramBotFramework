using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core.DefaultModules;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Example.SimpleBotExample.Modules
{
    [TelegramBotModule(Author = "ridicoulous", Name = "Survey", Version = "1.0")]
    public class SimpleSurveyModule : BaseSurveyModule<UserSurveyExample, SimpleTelegramBot>
    {
        public SimpleSurveyModule(SimpleTelegramBot wrapper) : base(wrapper)
        {
        }

        [ChatCommand(Triggers = new[] { "survey" }, HelpText = "User survey")]
        public CommandResponse InitServey(CommandEventArgs args)
        {
            return base.InitServey<UserSurveyExample>(args.SourceUser.UserId);
        }
        [ChatSurvey(Name = nameof(UserSurveyExample))]
        public override void SubmitSurvey(long userId, UserSurveyExample survey)
        {
            BotWrapper.SendMessageToAll($"User {userId} was submitted survey: \n```{JsonConvert.SerializeObject(survey)}```");
        }
        [ChatSurvey(Name = nameof(UserSurveyExample))]
        public override CommandResponse GetAnswer(Message message)
        {
            return base.GetAnswer(message);
        }
    }
}
