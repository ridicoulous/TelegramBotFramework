using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TelegramBotFramework.Core.Objects
{   
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property)]
    public class IgnoreFieldAttribute : Attribute
    {    

    }
}
