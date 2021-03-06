﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotFramework.Core.Interfaces
{
    /// <summary>
    /// Marker interface for take editable entities from db context
    /// </summary>
    public interface IEditableEntity
    {
        string PrimaryKeyName { get; }
        string EntityReadableName { get; }
    }

}
