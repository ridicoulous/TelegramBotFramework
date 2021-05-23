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

    [TelegramBotModule(Author = "ridicoulous", Name = "Base", Version = "1.0")]
    public class CrudBotModule<TBotWrapper, TDbContext> : TelegramBotModuleBase<TBotWrapper>
        where TBotWrapper : TelegramBotWrapperWithUserDb<TDbContext>
        where TDbContext : DbContext, ITelegramBotDbContext
    {
        public CrudBotModule(TBotWrapper wrapper) : base(wrapper)
        {

        }

        [ChatCommand(Triggers = new[] { "crud" }, DevOnly = true, DontSearchInline = true)]
        public CommandResponse GetAllEntitesForCrud(CommandEventArgs args)
        {
            var entitesToEdit = BotWrapper.Db.UsingDbContext(GetEditableEntites);
            return new CommandResponse("Choose entity for edit:", menu: GenerateMenuForEntity(entitesToEdit));
        }

        private Menu GenerateMenuForEntity(IEnumerable<Type> entities, string action = BotCrudActions.EditEntity)
        {
            var menu = new Menu()
            {
                Columns = 1,
                Buttons = entities.Select(f => new InlineButton($"Edit: {f.Name}", action, f.Name)).ToList()
            };
            return menu;
        }
        private Menu GenerateMenuForEntityFields(Type entry)
        {
            using (var db = BotWrapper.Db)
            {
                var entity = db.Model.FindEntityType(entry);
                var t = entity.FindPrimaryKey();
                db.Set<TelegramBotUser>();
                db.Update(entry);
               // db.UpdateEntity(entity.ClrType, entry);
                //db.Model.GetRelationalModel().Tables.ToList()[0].Columns.ToList()[0].va
            }
            //var type =typeof(TDbContext).GetProperty(entity.ReadableEntityNameForEditing);



            var menu = new Menu();
            return menu;
        }
        private IEnumerable<Type> GetEditableEntites(TDbContext context)
        {
            var types = context.Model.GetEntityTypes();
            List<Type> entities = new List<Type>();
            foreach (var t in types)
            {
                if (t.ClrType.IsAssignableFrom(typeof(IEditableEntity)))
                {
                    entities.Add(t.ClrType);
                }
            }
            return entities;
        }
    }
}
