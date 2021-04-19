using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotFramework.Core.Interfaces
{
    /// <summary>
    /// marker interface for entities editing through bot
    /// </summary>
    public interface IEditableEntity<TId>:IEditableEntity
    {
        new TId Id { get; set; }       
    }
    public interface IEditableEntity
    {       
        object Id { get; set; }
        string ReadableEntityNameForEditing { get; set; }
    }

}
