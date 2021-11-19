using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.DefaultModules;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;

namespace TelegramBotFramework.Example.SimpleBotExample.Modules
{
    [TelegramBotModule(Author = "ridicoulous", IsModuleActive = true, Name = "Echo", Version = "1.0")]
    public class SimpleEchoModule : TelegramBotModuleBase<SimpleTelegramBot>
    {
        public SimpleEchoModule(SimpleTelegramBot wrapper) : base(wrapper)
        {
        }
        [ChatCommand(Triggers = new[] { "long" }, HelpText = "Long message batch")]
        public virtual CommandResponse Long(CommandEventArgs args)
        {
            return new CommandResponse(new string('t',4128));
        }
        [ChatCommand(Triggers = new[] { "hello" }, HelpText = "Hello world")]
        public virtual CommandResponse Hello(CommandEventArgs args)
        {
            return new CommandResponse($"How are you, `{args.SourceUser.UserName ?? $"stranger"} with id {args.SourceUser.UserId}` ```{JsonConvert.SerializeObject(args.Message.From)}```", parseMode: ParseMode.MarkdownV2);
        }

        [ChatCommand(Triggers = new[] { "buttons" }, HelpText = "Hello buttons, user single value submit")]
        public virtual CommandResponse Buttons(CommandEventArgs args)
        {
            var buttonsMenu = new Menu(1, Enumerable.Range(0, 5).Select(c => new InlineButton($"Button {c}", "buttonpress", c.ToString())).ToList());
            return new CommandResponse($"Press the button below:", menu: buttonsMenu, parseMode: ParseMode.MarkdownV2);
        }

        [CallbackCommand(Trigger = "buttonpress", BotAdminOnly = true)]
        public CommandResponse CancelOrder(CallbackEventArgs args)
        {
            var p = args.Parameters;
            return new CommandResponse($"You pressed `{p}` button", parseMode: ParseMode.MarkdownV2);
        }
    }
    [TelegramBotModule(Author = "ridicoulous", IsModuleActive = true, Name = "CrudBotModuleTest", Version = "1.0")]
    public class SimpleCrudModule : BaseCrudModule<SimpleTelegramBot>
    {
        public SimpleCrudModule(SimpleTelegramBot wrapper) : base(wrapper)
        {
        }

        protected override void OnEntityUpdated(Type entityType, object entityValue)
        {
            BotWrapper.SendMessageToAll($"{entityType.FullName} was updated {Emoji.Dice}");
        }
    }
}