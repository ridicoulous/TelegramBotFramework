using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Example.SimpleBotExample.Modules
{
    public class UserSurveyExample
    {
        [Survey(new string[] { "Biden", "Trump" },Order =0)]
        public string President { get; set; }
        [Survey(QuestionText = "Enter probability of win", Order = 1)]
        public decimal Probability { get; set; }       
        [Survey(new string[] { "true", "false" },QuestionText ="Are you shure?", Order = 2)]
        public bool IsActive { get; set; }       
    }
}
