using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using TelegramBotFramework.Core.Interfaces;
using System.Net.Http;

namespace TelegramBotFramework.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services, ITelegramBotWrapper instance)
        {
            services.AddSingleton(instance);
        }
        public static void AddTelegramBot(this IServiceCollection services, string telegramBotKey, int adminId, HttpClient httpClient=null, string alias = "TelegramBotFramework", bool needNewUserApproove = false, string paymentToken = null, string dir = "", string webHookUrl = null, bool shouldUseInMemoryDb = false)
        {
            services.AddSingleton(serviceProvider =>
            {
                var instance = new TelegramBotWrapper(telegramBotKey, adminId, httpClient, serviceProvider, alias,needNewUserApproove,paymentToken,dir,webHookUrl,shouldUseInMemoryDb);
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
