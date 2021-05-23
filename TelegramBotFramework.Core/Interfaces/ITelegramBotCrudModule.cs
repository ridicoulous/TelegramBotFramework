using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Core.Interfaces
{
    public interface ITelegramBotCrudModule
    {
        Type LoadEntityTypeByName(string name, DbContext db);
        object LoadEntityValuebyId(object id, string primaryKeyName, Type entityType, DbContext db);
        IEnumerable<Type> GetEditableEntites();
        bool IsCurrentUserSubmitsEntityFieldValue(int userId);
        CommandResponse SubmitValue(int userId, string userInput);
        void Clear(int userId);

    }
}
