using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.DefaultModules
{
    public class TestServey : IBotSurvey
    {
        [Survey( new string[] {"42","43"})]
        public string Name { get; set; }
        [Survey(QuestionText ="А тут булево значение")]
        public bool BooleanValue { get; set; }
        [Survey]
        public decimal DecimalValue { get; set; }

        // public event Action<TestServey> OnFillig;

    }
  
    [TelegramBotModule(Author = "ridicoulous", Name = "Base", Version = "1.0")]
    public class BaseModule : TelegramBotModuleBase<TelegramBotWrapper>
    {
        //public TestModule(TelegramBotWrapper wrapper) : base(wrapper)
        //{

        //}    
        public BaseModule(TelegramBotWrapper wrapper) : base(wrapper)
        {

        }
        

        [ChatCommand(Triggers = new[] { "source" }, HelpText = "Gets the source code for this bot")]
        public CommandResponse GetSource(CommandEventArgs args)
        {
            return new CommandResponse("https://github.com/ridicoulous/TelegramBotFramework\n" +
                "Donates are greatly appreciated:\n" +
                "`3A1pFjyRu4eeGrZTMXWNp2LyEZbeUDLENN`\n" +
                "`0x6fea7665684584884124c1867d7ec31b56c43373`\n" +
                "Feel free to open the issues at GitHub", parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "modules" }, HelpText = "Show a list of modules currently loaded")]
        public CommandResponse GetModules(CommandEventArgs args)
        {
            //var sb = new StringBuilder();
            var menu = new Menu
            {
                Columns = 2,
                Buttons = BotWrapper.Modules.Select(x => new InlineButton(x.Key.Name, "i", x.Key.Name)).ToList()
            };
            //foreach (var m in Loader.Modules)
            //    sb.AppendLine($"{m.Key.Name}, by {m.Key.Author} (version {m.Key.Version})");
            return new CommandResponse("Currently loaded modules: ", menu: menu, level: ResponseLevel.Private);
        }

        [CallbackCommand(Trigger = "i", HelpText = "Gets information on a module")]
        public CommandResponse GetCommands(CallbackEventArgs args)
        {
            var sb = new StringBuilder();
            var m =
                 BotWrapper.Modules.FirstOrDefault(
                    x => String.Equals(x.Key.Name, args.Parameters, StringComparison.CurrentCultureIgnoreCase));
            if (m.Key == null)
                return new CommandResponse($"{args.Parameters} module not found.");
            sb.AppendLine($"*{m.Key.Name}*, by _{m.Key.Author}_ (version {m.Key.Version})\n");
            var menu = new Menu { Columns = 2 };
            foreach (var method in m.Value.GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
            {
                var att = method.GetCustomAttributes<ChatCommand>().First();
                menu.Buttons.Add(new InlineButton(att.Triggers[0], "c", att.Triggers[0]));
            }

            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown, menu: menu);
        }

        [CallbackCommand(Trigger = "c", HelpText = "Gets information on a command")]
        public CommandResponse GetCommandInfo(CallbackEventArgs args)
        {
            var sb = new StringBuilder();
            var c =
                 BotWrapper.Commands.FirstOrDefault(
                    x => String.Equals(x.Key.Triggers[0], args.Parameters, StringComparison.CurrentCultureIgnoreCase)).Key;
            if (c == null)
                return new CommandResponse($"{args.Parameters} command not found.");
            sb.AppendLine($"*{c.Triggers[0]}*: {c.HelpText}");
            if (c.Parameters.Length > 0)
                sb.AppendLine("*Parameters*");
            foreach (var p in c.Parameters)
                sb.AppendLine($"\t{p}");
            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "commands" }, HelpText = "commands <module name> - show all commands in the module", Parameters = new[] { "<module name>" })]
        public CommandResponse GetCommands(CommandEventArgs args)
        {
            var sb = new StringBuilder();
            var module =
                BotWrapper.Modules.FirstOrDefault(
                    x => String.Equals(x.Key.Name, args.Parameters, StringComparison.CurrentCultureIgnoreCase));
            if (module.Key == null)
                return new CommandResponse($"{args.Parameters} module not found.");

            foreach (var method in module.Value.GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
            {
                var att = method.GetCustomAttributes<ChatCommand>().First();
                sb.AppendLine($"*{att.Triggers[0]}*: {att.HelpText ?? method.Name}");
            }

            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown);
        }

        [ChatCommand(Triggers = new[] { "menu" }, HelpText = "Menue builded from all awailable")]
        public CommandResponse GetCommandList(CommandEventArgs args)
        {
            var sb = new StringBuilder();
            //var module =
            //    BotWrapper.Modules.FirstOrDefault();
            //if (module.Key == null)
            // return new CommandResponse($"{args.Parameters} module not found.");
            foreach (var module in BotWrapper.Modules)
            {
                sb.AppendLine($"*Module {module.Key.Name}:*");
                foreach (var method in module.Value.GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
                {
                    var att = method.GetCustomAttributes<ChatCommand>().First();
                    if (!att.DontSearchInline)
                        sb.AppendLine($"/{att.Triggers[0]} : {att.HelpText ?? method.Name}");
                }
            }

            return new CommandResponse(sb.ToString(), parseMode: ParseMode.Markdown);
        }


    }
}
