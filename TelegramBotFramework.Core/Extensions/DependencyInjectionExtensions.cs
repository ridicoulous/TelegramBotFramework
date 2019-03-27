using Microsoft.Extensions.DependencyInjection;

namespace TelegramBotFramework.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services, string telegramBotKey, int adminId, string alias = "MainConfig")
        {
            services.AddSingleton(serviceProvider =>
            {
                var instance = new TelegramBotWrapper("582936396:AAEgKrDTUfRGO8v7BtEqAjeqOKHYkuyAUig", 166938818, alias);
                return instance;
            });
        }
    }
}
