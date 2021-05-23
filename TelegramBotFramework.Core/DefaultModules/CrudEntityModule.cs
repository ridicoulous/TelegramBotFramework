using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private Type _currentUpdatingType;
        private object _currentUpdatingEntry;
        private string _updatingField;
        


        public CrudEntityModule(TBotWrapper wrapper) : base(wrapper)
        {

        }

        [ChatCommand(Triggers = new[] { "crud" }, DevOnly = true, DontSearchInline = true)]
        public CommandResponse GetAllEntitesForCrud(CommandEventArgs args)
        {
            var entitesToEdit = GetEditableEntites();
            return new CommandResponse("Choose entity for edit:", menu: GenerateMenuForEntity(entitesToEdit));
           
        }
        [ChatCommand(Triggers = new[] { "test" }, DevOnly = false, DontSearchInline = true)]
        public CommandResponse Test(CommandEventArgs args)
        {
            var entry = typeof(TelegramBotUser);
            ParameterExpression p = Expression.Parameter(entry);
            Expression property = Expression.Property(p, "Id");
            Expression c = Expression.Constant(1);
            Expression body = Expression.Equal(property, c);
            Expression exp = Expression.Lambda(body, new ParameterExpression[] { p });

            MethodInfo singleMethod = typeof(Queryable).GetMethods()
                .Single(m => m.Name == "Single" && m.GetParameters().Count() == 2)
                .MakeGenericMethod(entry);

            using (var db = BotWrapper.Db)
            {
                object result = singleMethod.Invoke(null, new object[] { db.Query(entry), exp });
            }
            return new CommandResponse("Choose entity for edit:");

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
            ParameterExpression p = Expression.Parameter(entry);
            Expression property = Expression.Property(p, "Id");
            Expression c = Expression.Constant(1);
            Expression body = Expression.Equal(property, c);
            Expression exp = Expression.Lambda(body, new ParameterExpression[] { p });

            MethodInfo singleMethod = typeof(Queryable).GetMethods()
                .Single(m => m.Name == "Single" && m.GetParameters().Count() == 2)
                .MakeGenericMethod(entry);

            //  DbSet dbSet = context.Set(domainObject.GetType());


            using (var db = BotWrapper.Db)
            {
                
                object result = singleMethod.Invoke(null, new object[] { db.Query(entry), exp });

                var entity = db.Model.FindEntityType(entry);
                var t = entity.FindPrimaryKey();
            
                db.Set<TelegramBotUser>();
                db.Update(entry);
                //db.UpdateEntity(entity.ClrType, entry);
                //db.Model.GetRelationalModel().Tables.ToList()[0].Columns.ToList()[0].va
            }
            //var type =typeof(TDbContext).GetProperty(entity.ReadableEntityNameForEditing);



            var menu = new Menu();
            return menu;
        }
        private IEnumerable<Type> GetEditableEntites()
        {
            using(var context = BotWrapper.Db)
            {
                var types = context.Model.GetEntityTypes();
                List<Type> entities = new List<Type>();
                foreach (var t in types)
                {
                    if (t.ClrType.IsAssignableTo(typeof(IEditableEntity)))
                    {
                        entities.Add(t.ClrType);
                    }
                }
                return entities;
            }
         
        }
    }
}
