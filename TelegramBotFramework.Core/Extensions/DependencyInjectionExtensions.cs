using Microsoft.Extensions.DependencyInjection;

namespace TelegramBotFramework.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services, string telegramBotKey, int adminId, string alias = "MainConfig")
        {
            services.AddSingleton(serviceProvider =>
            {
                var instance = new TelegramBotWrapper(telegramBotKey, adminId, alias);
                return instance;
            });
        }
    }
}
