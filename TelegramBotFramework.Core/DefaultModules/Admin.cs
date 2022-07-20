﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Helpers;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.DefaultModules
{
    [TelegramBotModule(Author = "parabola949", Name = "Admin", Version = "1.0")]
    public class Admin<TBot,TDb> : TelegramBotModuleBase<TBot> where TBot : ITelegramBotWrapper<TDb> where TDb : DbContext,ITelegramBotDbContext
    {
        public Admin(TBot bot) : base(bot)
        {

        }

        //[ChatCommand(Triggers = new[] { "ground", "finishhim!", "kthxbai" }, BotAdminOnly = true, HelpText = "Stops a user from using the bot")]
        //public static CommandResponse GroundUser(CommandEventArgs args)
        //{
        //    var target = args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
        //    if (target.UserId == args.SourceUser.UserId)
        //    {
        //        return new CommandResponse("Invalid target.");
        //    }
        //    if (target.Grounded)
        //    {
        //        return new CommandResponse($"{target.Name} is already grounded by {target.GroundedBy}");
        //    }
        //    target.Grounded = true;
        //    target.GroundedBy = args.SourceUser.Name;
        //    target.Save(args.DatabaseInstance);
        //    return new CommandResponse($"{target.Name} is grounded!");
        //}

        //[ChatCommand(Triggers = new[] { "unground", "izoknaow" }, BotAdminOnly = true, HelpText = "Allows user to use the bot again", Parameters = new[] { "<userid>", "<@username>", "as a reply" })]
        //public static CommandResponse UngroundUser(CommandEventArgs args)
        //{
        //    var target = args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
        //    if (target.UserId == args.SourceUser.UserId)
        //    {
        //        return new CommandResponse("Invalid target.");
        //    }
        //    if (!target.Grounded)
        //    {
        //        return new CommandResponse($"{target.Name} isn't grounded anyways...");
        //    }
        //    target.Grounded = false;
        //    target.GroundedBy = null;
        //    target.Save(args.DatabaseInstance);
        //    return new CommandResponse($"{target.Name} is ungrounded!");
        //}

        //[ChatCommand(Triggers = new[] { "sql" }, DevOnly = true, Parameters = new[] { "<sql command>" })]
        //public static CommandResponse RunSql(CommandEventArgs args)
        //{
        //    return new CommandResponse($"{args.DatabaseInstance.ExecuteNonQuery(args.Parameters)} records changed");
        //}

        //[ChatCommand(Triggers = new[] { "query" }, DevOnly = true, Parameters = new[] { "<select statement>" })]
        //public static CommandResponse RunQuery(CommandEventArgs args)
        //{
        //    return new CommandResponse(args.DatabaseInstance.ExecuteQuery(args.Parameters));
        //}

        //[ChatCommand(Triggers = new[] { "cleandb", }, DevOnly = true, HelpText = "Cleans all users with UserID (0)")]
        //public static CommandResponse CleanDatabase(CommandEventArgs args)
        //{
        //    var start = args.DatabaseInstance.Users.Count();
        //    args.DatabaseInstance.ExecuteNonQuery("DELETE FROM USERS WHERE UserId = 0");
        //    var end = args.DatabaseInstance.Users.Count();
        //    return new CommandResponse($"Database cleaned. Removed {start - end} users.");
        //}

        #region Chat Commands

        [ChatCommand(Triggers = new[] { "addbotadmin", "addadmin" }, DevOnly = true, DontSearchInline = true, Parameters = new[] { "<userid>", "<@username>", "as a reply" })]
        public CommandResponse AddBotAdmin(CommandEventArgs args)
        {
            using(var db = BotWrapper.Db)
            {
                var target =db.GetTarget(args);
                if (target != null && target.Id != args.SourceUser.Id)
                {
                    target.IsBotAdmin = true;
                    return new CommandResponse($"{target.Name} is now a bot admin.");
                }
                if (target != null && target.Id == args.SourceUser.Id)
                    return new CommandResponse("You can't add yourself!");
                else
                    return new CommandResponse("Unknown user or user is not cached!");
            }          
        }

        [ChatCommand(Triggers = new[] { "rembotadmin", "remadmin" }, DevOnly = true, DontSearchInline = true, Parameters = new[] { "<userid>", "<@username>", "as a reply" })]
        public CommandResponse RemoveBotAdmin(CommandEventArgs args)
        {
            var target = BotWrapper.Db.GetTarget(args);
            if (target != null && target.Id != args.SourceUser.Id)
            {
                target.IsBotAdmin = false;
                return new CommandResponse($"{target.Name} is no longer a bot admin.");
            }
            if (target != null && target.Id == args.SourceUser.Id)
                return new CommandResponse("You can't remove yourself!");
            else 
                return new CommandResponse("Unknown user or user is not cached!");
        }
        [ChatCommand(Triggers = new[] { "users" }, DevOnly = true, DontSearchInline = true)]
        public CommandResponse GetUsersList(CommandEventArgs args)
        {
            var sb = new StringBuilder();
            var users = BotWrapper.Db.TelegramBotUsers.AsNoTracking().ToList();
            foreach (var u in users)
            {
                sb.Append($"{u.UserId}: {u.Name} {u.UserName} {u.IsBotAdmin} {u.FirstSeen}\n");
            }
            return new CommandResponse(sb.ToString());
        }

        //[ChatCommand(Triggers = new[] { "cs" }, DevOnly = true, AllowInlineAdmin = true)]
        //public static CommandResponse RunCsCode(CommandEventArgs args)
        //{
        //    return new CommandResponse($"``` {args.Parameters} ```\n" + CompileCs(
        //        @"using System.Linq;
        //        using System;
        //        using System.Collections.Generic;
        //        using System.Diagnostics;
        //        using System.IO;
        //        using System.Net;
        //        using System.Threading;
        //        using Telegram.Bot.Types.Enums;
        //        using Telegram.Bot.Types;
        //        using Telegram.Bot;
        //        using System.Threading.Tasks;
        //        class Program {
        //            public static void Main(string[] args) {
        //                " + args.Parameters + @"
        //            }
        //        }").Result, parseMode: ParseMode.Markdown);
        //}

        //[ChatCommand(Triggers = new[] { "tg" }, DevOnly = true, AllowInlineAdmin = true)]
        //public static CommandResponse EmulateTG(CommandEventArgs args)
        //{
        //    var code = args.Parameters;
        //    var rgx = new Regex("(bot.).*(Async).*(\\))(?=;)");
        //    var add = "var r = $&.Result";
        //    code = rgx.Replace(code, add);
        //    return new CommandResponse($"``` {code} ```\n" + CompileCs(
        //        @"using System.Linq;
        //        using System;
        //        using System.Collections.Generic;
        //        using System.Diagnostics;
        //        using System.IO;
        //        using System.Net;
        //        using System.Threading;
        //        using Telegram.Bot.Types.Enums;
        //        using Telegram.Bot.Types;
        //        using Telegram.Bot;
        //        using System.Threading.Tasks;
        //        using Newtonsoft.Json;
        //        class Program {
        //            public static void Main(string[] args) {
        //                var bot = new TelegramBotClient(""" + Program.LoadedSetting.TelegramBotAPIKey + @""");
        //                try{" + code + @"}
        //                catch(AggregateException e){
        //                Console.WriteLine(e.InnerExceptions[0].Message);
        //                }
        //            }
        //        }").Result, parseMode: ParseMode.Markdown);
        //}

        //private static async Task<string> CompileCs(string code)
        //{
        //    try
        //    {
        //        var csc = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
        //        var parameters = new CompilerParameters(
        //            new[]
        //            {
        //                "mscorlib.dll", "System.Core.dll", "System.dll", "System.Data.dll", "Telegram.Bot.dll",
        //                "Newtonsoft.Json.dll", "System.Net.Http.dll"
        //            }, Path.Combine(Program.RootDirectory, "foo.exe"), true)
        //        { GenerateExecutable = true };
        //        CompilerResults results = csc.CompileAssemblyFromSource(parameters, code);
        //        var result = new StringBuilder();
        //        if (results.Errors.HasErrors)
        //        {
        //            results.Errors.Cast<CompilerError>().ToList().ForEach(error => result.AppendLine(error.ErrorText));
        //            return result.ToString();
        //        }
        //        //no errors, run it.
        //        var proc = new Process
        //        {
        //            StartInfo = new ProcessStartInfo
        //            {
        //                FileName = Path.Combine(Program.RootDirectory, "foo.exe"),
        //                UseShellExecute = false,
        //                RedirectStandardOutput = true,
        //                CreateNoWindow = true,
        //                WorkingDirectory = Program.RootDirectory
        //            }
        //        };

        //        proc.Start();

        //        while (!proc.StandardOutput.EndOfStream)
        //        {
        //            result.AppendLine(proc.StandardOutput.ReadLine());
        //            await Task.Delay(500);
        //        }
        //        return result.ToString();
        //    }
        //    catch (Exception e)
        //    {
        //        return $"{e.Message}\n{e.StackTrace}";
        //    }
        //}
        #endregion
    }
}
