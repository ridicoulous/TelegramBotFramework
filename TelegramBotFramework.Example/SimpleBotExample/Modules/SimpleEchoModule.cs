using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Example.SimpleBotExample.Modules
{
    [TelegramBotModule(Author = "ridicoulous", IsModuleActive = true, Name = "Echo", Version = "1.0")]
    public class SimpleEchoModule : TelegramBotModuleBase<SimpleTelegramBot>
    {
        public SimpleEchoModule(SimpleTelegramBot wrapper) : base(wrapper)
        {
        }

        [ChatCommand(Triggers = new[] { "hello" }, HelpText = "Hello world")]
        public virtual CommandResponse Hello(CommandEventArgs args)
        {            
            return new CommandResponse($"How are you, `{args.SourceUser.UserName ?? $"stranger"} with id {args.SourceUser.UserId}`", parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "buttons" }, HelpText = "Hello buttons, user single value submit")]
        public virtual CommandResponse Buttons(CommandEventArgs args)
        {
            var buttonsMenu = new Menu(1,Enumerable.Range(0,5).Select(c=> new InlineButton($"Button {c}", "buttonpress", c.ToString())).ToList());
            return new CommandResponse($"Press the button below:", menu:buttonsMenu, parseMode: ParseMode.Default);
        }

        [CallbackCommand(Trigger = "buttonpress", BotAdminOnly = true)]
        public CommandResponse CancelOrder(CallbackEventArgs args)
        {
            var p = args.Parameters;
            return new CommandResponse($"You pressed `{p}` button", parseMode: ParseMode.Default);
        }
    }
}
