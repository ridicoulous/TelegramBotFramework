using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Extensions;
using TelegramBotFramework.Core.Helpers;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;

namespace TelegramBotFramework.Core.DefaultModules
{

    [TelegramBotModule(Author = "ridicoulous", Name = "CrudEntityModule", Version = "1.0")]
    public abstract class CrudEntityModule<TBotWrapper> : TelegramBotModuleBase<TBotWrapper>
        where TBotWrapper : ITelegramBotWrapper
        
    {
        public CrudEntityModule(TBotWrapper wrapper) : base(wrapper)
        {

        }

        [ChatCommand(Triggers = new[] { "crud" }, DevOnly = true, DontSearchInline = true)]
        public CommandResponse GetAllEntitesForCrud(CommandEventArgs args)
        {
            var entitesToEdit = GetEditableEntites();
            return new CommandResponse("Choose entity for edit:", menu: GenerateMenuForEntity(entitesToEdit));
        }

        private Menu GenerateMenuForEntity(IEnumerable<IEditableEntity> entities, string action = BotCrudActions.EditEntity)
        {
            var menu = new Menu()
            {
                Columns = 1,
                Buttons = entities.Select(f => new InlineButton($"Edit: {f.ReadableEntityNameForEditing}", action, f.ReadableEntityNameForEditing)).ToList()
            };
            return menu;
        }
        private Menu GenerateMenuForEntityFields(IEditableEntity entry)
        {
            using (var db = BotWrapper.Db)
            {
                var entity = db.Model.FindEntityType(entry.ReadableEntityNameForEditing);
                var t = entity.FindPrimaryKey();
                db.Set<TelegramBotUser>();
                db.Update(entry);
                db.UpdateEntity(entity.ClrType, entry);
                //db.Model.GetRelationalModel().Tables.ToList()[0].Columns.ToList()[0].va
            }
            //var type =typeof(TDbContext).GetProperty(entity.ReadableEntityNameForEditing);



            var menu = new Menu();
            return menu;
        }
        private IEnumerable<IEditableEntity> GetEditableEntites()
        {
            using(var context = BotWrapper.Db)
            {
                var types = context.Model.GetEntityTypes();
                List<IEditableEntity> entities = new List<IEditableEntity>();
                foreach (var t in types)
                {
                    if (t.ClrType.IsAssignableTo(typeof(IEditableEntity)))
                    {
                        entities.Add(new EditableEntity(t.Name));
                    }
                }
                return entities;
            }
         
        }
    }
}
