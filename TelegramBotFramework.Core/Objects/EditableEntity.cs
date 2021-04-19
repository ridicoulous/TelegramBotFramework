using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Interfaces;

namespace TelegramBotFramework.Core.Objects
{
    public class EditableEntity<TId> : EditableEntity,IEditableEntity
    {
        public EditableEntity(TId id, string readableEntityNameForEditing):base(readableEntityNameForEditing)
        {
            Id = id;
           // EntityFieldsAndValues = entityFieldsAndValues ?? throw new ArgumentNullException(nameof(entityFieldsAndValues));
        }

        public TId Id { get; set; }
        
        object IEditableEntity.Id { get => Id; set => Id= (TId)value; }
    }
    public class EditableEntity : IEditableEntity
    {
        public EditableEntity(string readableEntityNameForEditing)
        {
            ReadableEntityNameForEditing = readableEntityNameForEditing ?? throw new ArgumentNullException(nameof(readableEntityNameForEditing));
        }

        public string ReadableEntityNameForEditing { get; set; }
        public object Id { get; set; }
    }
}
