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
        public static void UsingDbContext<TDbContext>(this Func<TDbContext> getContextAsDisposable, Action<TDbContext> contextAction)
          where TDbContext : DbContext
        {
            using (var context = getContextAsDisposable())
            {
                contextAction(context);
                context.SaveChanges();
            }
        }
        public static TReturn UsingDbContext<TDbContext,TReturn>(this Func<TDbContext> getContextAsDisposable, Func<TDbContext, TReturn> contextAction)
         where TDbContext : DbContext
        {
            using (var context = getContextAsDisposable())
            {
               var result = contextAction(context);
                context.SaveChanges();
                return result;
            }
        }
        public static void UsingDbContextEntity<TDbContext, TDbSet>(this Func<TDbContext> getContextAsDisposable, Action<DbSet<TDbSet>> contextAction) where TDbSet: class
        where TDbContext : DbContext
        {
            using (var context = getContextAsDisposable())
            {
                var set = context.Set<TDbSet>();
                contextAction(set);
                context.SaveChanges();
                 //contextAction(context);
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
