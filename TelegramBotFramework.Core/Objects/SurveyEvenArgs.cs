using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.Objects
{
    public class SurveyEventArgs<T> : EventArgs  where T : IBotSurvey
    {
        public SurveyEventArgs(long userId, T survay)
        {
            UserId = userId;
            FilledSurvey = survay;
        }
        public long UserId { get; set; }
        public T FilledSurvey { get; set; }
    }
}
