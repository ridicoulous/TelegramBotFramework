using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TelegramBotFramework.Core.Interfaces;
using System.Net.Http;
using System;
using Microsoft.EntityFrameworkCore;

namespace TelegramBotFramework.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services, ITelegramBotWrapper instance)
        {
            services.AddSingleton(instance);
        }
        public static void AddTelegramBot(this IServiceCollection services, ITelegramBotOptions options)
        {
            services.AddSingleton(serviceProvider =>
            {
                var instance = new TelegramBotWrapper(options);
                return instance;
            });
        }
        public static void AddTelegramBotWithDbContext<TDbContext>(this IServiceCollection services, ITelegramBotOptions options, Func<TDbContext> contextFactory) 
            where TDbContext : DbContext, ITelegramBotDbContext
        {
            services.AddSingleton(serviceProvider =>
            {
                var instance = new TelegramBotWrapperWithUserDb<TDbContext>(options,contextFactory); 
                return instance;
            });
        }

        public static void UseTelegramBot(this IApplicationBuilder app)
        {
            var bots = app.ApplicationServices.GetServices<ITelegramBotWrapper>();
            foreach (var b in bots)
            {
                b.Run();
            }
        }
    }
}
