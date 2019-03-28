using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
namespace TelegramBotFramework.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services, string telegramBotKey, int adminId, string alias = "MainConfig")
        {
            services.AddSingleton(serviceProvider =>
            {
                var instance = new TelegramBotWrapper(telegramBotKey, adminId, serviceProvider, alias);
                return instance;
            });
        }

        public static void UseTelegramBot(this IApplicationBuilder app)
        {
             app.ApplicationServices.GetServices<TelegramBotWrapper>();
        }
    }
}
