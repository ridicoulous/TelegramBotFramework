using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
        public static void UsingDbContextEntity<TDbContext, TDbSet>(this TDbContext disposableDbContext, Action<DbSet<TDbSet>> contextAction, bool shouldSaveChanges = false) where TDbSet : class
        where TDbContext : DbContext
        {
            using (disposableDbContext)
            {
                var set = disposableDbContext.Set<TDbSet>();
                contextAction(set);
                if (shouldSaveChanges)
                    disposableDbContext.SaveChanges();
            }
        }

        public static IQueryable Query(this DbContext context, string entityName) =>
            context.Query(context.Model.FindEntityType(entityName).ClrType);

        static readonly MethodInfo SetMethod = typeof(DbContext).GetMethods().Single(m => m.Name == nameof(DbContext.Set) && m.GetParameters().Count() == 0);
        //static readonly MethodInfo UpdateMethod = typeof(DbContext).GetMethod(nameof(DbContext.Update));


        public static IQueryable Query(this DbContext context, Type entityType) =>
                (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(context, null);

        public static IQueryable<object> Set(this DbContext context, Type t)
        {
            return (IQueryable<object>)SetMethod
                  .MakeGenericMethod(t)
                  .Invoke(context, null);
        }
        //public static void UpdateEntity(this DbContext context, Type entityType, object value) =>
        //        UpdateMethod.MakeGenericMethod(entityType).Invoke(context, new[] { value });
        public static object GetEntityByTypeAndId(this DbContext db, object id, string idFieldName, Type entityType)
        {
            var pkType = db.Model.FindEntityType(entityType);
            if (pkType != null)
            {
                var idConverted = Convert.ChangeType(id, pkType.FindPrimaryKey().GetKeyType(), CultureInfo.GetCultureInfo("en-US"));
                ParameterExpression p = Expression.Parameter(entityType);

                Expression property = Expression.Property(p, idFieldName);
                Expression c = Expression.Constant(idConverted);
                Expression body = Expression.Equal(property, c);
                Expression exp = Expression.Lambda(body, new ParameterExpression[] { p });

                MethodInfo singleMethod = typeof(Queryable).GetMethods()
                    .Single(m => m.Name == "Single" && m.GetParameters().Count() == 2)
                    .MakeGenericMethod(entityType);
                return singleMethod.Invoke(null, new object[] { db.Query(entityType), exp });
            }
            else
            {
                return null;
            }
            
        }
        public static object GetFirstOrDefault(this DbContext db,  Type entityType)
        {
            
            return db.Set(entityType).FirstOrDefault();
        }
        public static List<object> GetAll(this DbContext db, Type entityType)
        {
            return db.Set(entityType).ToList();
        }

    }
}
