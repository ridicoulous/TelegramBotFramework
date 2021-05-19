using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotFramework.Core.Extensions
{
    public static class DbContextExtensions
    {

        public static void UsingDbContext<TDbContext>(this TDbContext disposableDbContext, Action<TDbContext> contextAction)
          where TDbContext : DbContext
        {
            using (disposableDbContext)
            {
                contextAction(disposableDbContext);
                disposableDbContext.SaveChanges();
            }
        }
        public static TReturn UsingDbContext<TDbContext, TReturn>(this TDbContext disposableDbContext, Func<TDbContext, TReturn> contextAction)
         where TDbContext : DbContext
        {
            using (disposableDbContext)
            {
                var result = contextAction(disposableDbContext);
                disposableDbContext.SaveChanges();
                return result;
            }
        }
        public static void UsingDbContextEntity<TDbContext, TDbSet>(this TDbContext disposableDbContext, Action<DbSet<TDbSet>> contextAction, bool shouldSaveChanges=false) where TDbSet : class
        where TDbContext : DbContext
        {
            using (disposableDbContext)
            {
                var set = disposableDbContext.Set<TDbSet>();
                contextAction(set);
                if(shouldSaveChanges)       
                    disposableDbContext.SaveChanges();
            }
        }

        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set));
        static readonly MethodInfo UpdateMethod = typeof(DbContext).GetMethod(nameof(DbContext.Update));


        public static IQueryable Query(this DbContext context, Type entityType) =>
                (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);

        public static void UpdateEntity(this DbContext context, Type entityType, object value) =>
                UpdateMethod.MakeGenericMethod(entityType).Invoke(context, new[] { value });

    }
}
