using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Example.SimpleBotExample.Modules
{
    public class UserSurveyExample
    {
        [Survey(new string[] { "Baiden", "Trump" })]
        public string President { get; set; }
        [Survey(QuestionText = "Enter probability of win")]
        public decimal Probability { get; set; }       
        [Survey(new string[] { "true", "false" },QuestionText ="Are you shure?")]
        public bool IsActive { get; set; }
       

    }
}
