using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelegramBotFramework.Core.Objects
{   
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class SurveyAttribute : Attribute
    {
        public string UpdatingPropertyName { get; set; }
        public string QuestionText { get;  set; }
        public List<string> Choises { get; set; } = new List<string>();
        public bool IsAnsvered { get; set; }
        public SurveyAttribute()
        {

        }
        public SurveyAttribute(string[] choises )
        {            
            Choises = choises.ToList();
        }
        public SurveyAttribute(string question)
        {
            QuestionText = question;    
        }
        public SurveyAttribute(string question, string[] choises)
        {
            QuestionText = question;
            Choises = choises.Any() ? choises.ToList() : new List<string>();
        }

    }
}
