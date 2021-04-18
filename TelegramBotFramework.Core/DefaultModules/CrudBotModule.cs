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

namespace TelegramBotFramework.Core.DefaultModules
{

    [TelegramBotModule(Author = "ridicoulous", Name = "Base", Version = "1.0")]
    public class CrudBotModule<TDbContext> : TelegramBotModuleBase<TelegramBotWrapperWithUserDb<TDbContext>> 
        where TDbContext : DbContext, ITelegramBotDbContext
    {
        public CrudBotModule(TelegramBotWrapperWithUserDb<TDbContext> wrapper) : base(wrapper)
        {

        }

        [ChatCommand(Triggers = new[] { "crud" }, DevOnly = true, DontSearchInline = true)]
        public CommandResponse GetAllEntitesForCrud(CommandEventArgs args)
        {
            var entitesToEdit = BotWrapper.DbContextFactory.UsingDbContext(GetEditableEntites);
            return new CommandResponse("Choose entity for edit:", menu: GenerateMenuForEntity(entitesToEdit));
        }

        private Menu GenerateMenuForEntity(IEnumerable<IEditableEntity> entities, string action=BotCrudActions.EditEntity)
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
            using(var db = BotWrapper.DbContextFactory())
            {
                var entity = db.Model.FindEntityType(entry.ReadableEntityNameForEditing);
                var t = entity.FindPrimaryKey();
                
                //db.Model.GetRelationalModel().Tables.ToList()[0].Columns.ToList()[0].va
            }
            //var type =typeof(TDbContext).GetProperty(entity.ReadableEntityNameForEditing);
            
            
           
            var menu = new Menu();
            return menu;
        }
        private IEnumerable<IEditableEntity> GetEditableEntites(TDbContext context)
        {
            var types = context.Model.GetEntityTypes();
            List<IEditableEntity> entities = new List<IEditableEntity>();
            foreach (var t in types)
            {
                if (t.ClrType.IsAssignableFrom(typeof(IEditableEntity)))
                {
                    entities.Add(new EditableEntity(t.Name));
                }
            }
            return entities;
        }
    }
}
