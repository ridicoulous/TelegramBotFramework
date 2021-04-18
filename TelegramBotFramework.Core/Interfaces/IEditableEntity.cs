using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotFramework.Core.Interfaces
{
    /// <summary>
    /// marker interface for entities editing through bot
    /// </summary>
    public interface IEditableEntity<TId>
    {
        TId Id { get; set; }        
        Dictionary<string,object> EntityFieldsAndValues { get; set; }
    }
    public interface IEditableEntity
    {       
        string ReadableEntityNameForEditing { get; set; }
    }

}
