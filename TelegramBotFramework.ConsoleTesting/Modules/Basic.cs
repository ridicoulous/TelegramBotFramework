﻿//using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Telegram.Bot;
using TelegramBotFramework.Core;
using TelegramBotFramework.Core.Objects;
//using Menu = ModuleFramework.Menu;

namespace CSChatBot.Modules
{
    [TelegramBotModule(Author = "msg_kurt", Name = "Google", Version = "1.0")]
    class Basic : TelegramBotModuleBase
    {
        private static readonly Regex Reg = new Regex("(<h3 class=\"r\">)(.*?)(</h3>)", RegexOptions.IgnoreCase);
        public Basic(TelegramBotWrapper bot):base(bot)
        {

        }

        [ChatCommand(Triggers = new[] { "google", "g", "lmgtfy" }, HideFromInline = true, Parameters = new[] { "<your search>" })]
        public  CommandResponse Google(CommandEventArgs args)
        {
            if (String.IsNullOrEmpty(args.Parameters)) return new CommandResponse("");
            var data = new WebClient().DownloadString($"https://www.google.com/search?q={args.Parameters}");
            var matches = Reg.Matches(data);
            try
            {
                var searchResults = new Dictionary<string, string>();
                foreach (Match result in matches)
                {
                    if (result.Value.Contains("a class=\"sla\"")) continue;
                    if (result.Value.Contains("<a href=\"/search?q=")) continue;
                    var start = result.Value.Replace("<h3 class=\"r\"><a href=\"/url?q=", "");
                    var url = start.Substring(0, start.IndexOf("&amp;sa=U"));
                    var title = start.Substring(start.IndexOf(">") + 1);
                    title = title.Replace("<b>", "").Replace("</b>", "").Replace("</a></h3>", "");
                    title = System.Web.HttpUtility.HtmlDecode(title);
                    searchResults.Add(title, url);
                    if (searchResults.Count >= 5) break;
                }
                var menu = new Menu();
                foreach (var result in searchResults)
                    menu.Buttons.Add(new InlineButton(result.Key, url: result.Value));
                return new CommandResponse("Here are the results for " + args.Parameters, menu: menu);
            }
            catch (Exception e)
            {
                return new CommandResponse("Sorry, I wasn't able to pull the results.  Send this to Para: " + e.Message);
            }
        }
    }
}
