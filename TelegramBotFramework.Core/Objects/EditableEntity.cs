using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.Objects
{
    public class EditableEntity<TId> : IEditableEntity<TId>
    {
        public EditableEntity(TId id, Dictionary<string, object> entityFieldsAndValues)
        {
            Id = id;
            EntityFieldsAndValues = entityFieldsAndValues ?? throw new ArgumentNullException(nameof(entityFieldsAndValues));
        }

        public TId Id { get; set; }
        public Dictionary<string, object> EntityFieldsAndValues { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public class EditableEntity : IEditableEntity
    {
        public EditableEntity(string readableEntityNameForEditing)
        {
            ReadableEntityNameForEditing = readableEntityNameForEditing ?? throw new ArgumentNullException(nameof(readableEntityNameForEditing));
        }

        public string ReadableEntityNameForEditing { get; set; }
    }
}
