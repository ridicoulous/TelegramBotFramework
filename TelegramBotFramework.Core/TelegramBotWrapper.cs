using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotFramework.Core.DefaultModules;
using TelegramBotFramework.Core.Helpers;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Logging;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;

namespace TelegramBotFramework.Core
{
    public class TelegramBotWrapper : TelegramBotWrapperWithDb<TelegramBotDefaultSqLiteDbContext>
    {
        public TelegramBotWrapper(ITelegramBotOptions options) : base(options,  new DefaultDbContextFactory(options.Alias,options.InMemoryDb))
        {
         
        }
    }

    public class TelegramBotWrapperWithUserDb<TDbContext> : TelegramBotWrapperWithDb<TDbContext> where TDbContext : DbContext, ITelegramBotDbContext
    {
        public TelegramBotWrapperWithUserDb(ITelegramBotOptions options,IDbContextFactory<TDbContext> factory) : base(options,factory)
        {
        }
    }

    public abstract class TelegramBotWrapperWithDb<TDbContext> : ITelegramBotWrapper<TDbContext>, ITelegramBotWrapper
         where TDbContext : DbContext, ITelegramBotDbContext
    {
        public static string RootDirectory { get; set; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public UsersSurveys CurrentUserUpdatingObjects { get; set; } = new UsersSurveys();
        public delegate CommandResponse ChatCommandMethod(CommandEventArgs args);
        public delegate CommandResponse ChatServeyMethod(Message args);
        public Dictionary<long, Queue<SurveyAttribute>> UsersWaitingAnswers { get; set; } = new Dictionary<long, Queue<SurveyAttribute>>();

        public Dictionary<ChatSurvey, ChatServeyMethod> SurveyAnswersHandlers = new Dictionary<ChatSurvey, ChatServeyMethod>();

        public readonly List<string> Questions = new List<string>();
        public delegate CommandResponse CallbackCommandMethod(CallbackEventArgs args);
        public delegate void OnException(Exception unhandled);
        public event OnException ExceptionHappened;
        public List<InvoiceDto> UserInvoices = new List<InvoiceDto>();
        public event Action<InvoiceDto> OnPaymentReceived;
        public Dictionary<ChatCommand, ChatCommandMethod> Commands = new Dictionary<ChatCommand, ChatCommandMethod>();
        public Dictionary<CallbackCommand, CallbackCommandMethod> CallbackCommands = new Dictionary<CallbackCommand, CallbackCommandMethod>();
        public Dictionary<TelegramBotModule, Type> Modules = new Dictionary<TelegramBotModule, Type>();
        public TelegramBotLogger Log;
        /// <summary>
        /// Should be disposed!
        /// </summary>
        public TDbContext Db => DbContextFactory.CreateDbContext();
        private readonly IDbContextFactory<TDbContext> DbContextFactory;

        public TelegramBotSetting LoadedSetting;
        public ModuleMessenger Messenger = new ModuleMessenger();
        public TelegramBotClient Bot { get; private set; }
        internal static User Me = null;
        public bool IsSurveyInitiated { get; set; }
        public ConcurrentDictionary<long, KeyValuePair<Type, IEditableEntity>> UserEditingEntity { get; set; }

        DbContext ITelegramBotWrapper.Db => Db;

        public virtual void SeedBotAdmins(params int[] admins)
        {
            try
            {
                foreach (var u in admins)
                {
                    Db.Users.Add(new TelegramBotUser() { IsBotAdmin = true, UserId = u, FirstSeen = DateTime.UtcNow });
                }
                Db.Users.Add(new TelegramBotUser() { IsBotAdmin = true, UserId = LoadedSetting.TelegramDefaultAdminUserId, FirstSeen = DateTime.UtcNow });
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Write($"Saving admin error: {ex}", LogLevel.Error, null, "error.log");
            }
        }
        protected readonly ITelegramBotOptions Options;
        /// <summary>
        /// Constructor. You may inject IServiceProvider to freely use you registered services in your modules
        /// </summary>
        /// <param name="key"></param>
        /// <param name="adminId"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="alias"></param>       
    
        public TelegramBotWrapperWithDb(ITelegramBotOptions options, IDbContextFactory<TDbContext> contextFactory)
        {
            DbContextFactory = contextFactory;
           
            Options = options;          
            if (!String.IsNullOrEmpty(options.Directory))
            {
                RootDirectory = options.Directory;
            }

            if (!Directory.Exists(Path.Combine(RootDirectory)))
            {
                Directory.CreateDirectory(Path.Combine(RootDirectory));
            }
            Log = new TelegramBotLogger(Path.Combine(RootDirectory, "Logs-" + Options.Alias));
            var setting = new TelegramBotSetting() { Alias = options.Alias, TelegramDefaultAdminUserId = Options.AdminId, TelegramBotAPIKey = Options.Key };
            LoadedSetting = setting;

            try
            {                
               
                using(Db)
                {
                    Db.Database.EnsureCreated();

                    Db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"Db creating data error: {ex.ToString()}", LogLevel.Error, null, "error.log");
            }

            Console.OutputEncoding = Encoding.UTF8;
            Messenger.MessageSent += MessengerOnMessageSent;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            //var telegramBotModuleDir = Path.Combine(RootDirectory, "AddonModules-" + alias);

            //WatchForNewModules(telegramBotModuleDir);
        }
      
        private void WatchForNewModules(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Path = path;
            //watcher.NotifyFilter = NotifyFilters.LastWrite;
            //watcher.Filter = "*.dll";
            //watcher.Changed += new FileSystemEventHandler(OnChanged);
            //watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Log.WriteLine("Scanning Addon TelegramBotModules directory for custom TelegramBotModules...", overrideColor: ConsoleColor.Cyan);
            LoadModules();
        }

        public void LoadModules()
        {
            //Clear the list
            Commands.Clear();
            //load base methods first
            GetMethodsFromAssembly(Assembly.GetExecutingAssembly());
            Log.WriteLine("Scanning Addon TelegramBotModules directory for custom TelegramBotModules...", overrideColor: ConsoleColor.Cyan);
            var telegramBotModuleDir = Path.Combine(RootDirectory, "AddonModules-" + LoadedSetting.Alias);
            Directory.CreateDirectory(telegramBotModuleDir);

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //now load TelegramBotModules from directory
            foreach (var file in Directory.GetFiles(telegramBotModuleDir, "*.dll"))
            {
                GetMethodsFromAssembly(Assembly.LoadFrom(file));
            }
            foreach (var assembly in assemblies)
            {
                GetMethodsFromAssembly(assembly);
            }
        }

        private void GetMethodsFromAssembly(Assembly assembly)
        {
            try
            {
                foreach (var botModule in assembly.GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract
                   && myType.IsDefined(typeof(TelegramBotModule))))
                {

                    var moduleAttributes = botModule.GetCustomAttributes<TelegramBotModule>().FirstOrDefault();
                    Log.WriteLine($"Loading {botModule.GetType().Name} ({moduleAttributes.Name}) module");
                    var currentBot = this.GetType();
                    object instance = null;
                    if (moduleAttributes.Name == "CrudSimple")
                    {

                    }
                    var constructs = botModule.GetConstructors();
                    foreach (var c in constructs)
                    {
                        var paramss = c.GetParameters();
                        if (paramss.Length == 1)
                        {
                            if (paramss[0].ParameterType == currentBot)
                            {
                                Log.WriteLine($"Finded constructor, invoking it for loading {moduleAttributes.Name} at {this.GetType().FullName}");

                                instance = c.Invoke(new object[] { this });
                            }
                        }
                    }
                    if (instance == null)
                    {
                        var constructor = botModule.GetConstructor(new[] { typeof(TelegramBotWrapper) });

                        if (constructor == null)
                        {
                            Log.WriteLine($"Can not create instance of {moduleAttributes.Name}");
                            continue;
                        }
                        Log.WriteLine($"Finded constructor {constructor.Name}, invoking it for loading {moduleAttributes.Name} at {this.GetType().FullName}");
                        instance = constructor.Invoke(new object[] { this });
                    }
                    if (instance == null)
                    {
                        Log.WriteLine($"{moduleAttributes.Name}not loaded cause can not instantiate by finding contructor");

                        continue;
                    }
                    if (Modules.ContainsKey(moduleAttributes))
                    {
                        Log.WriteLine($"{moduleAttributes.Name} has been already loaded. Rename it, if it is no dublicate");
                        continue;
                    }
                    Modules.Add(moduleAttributes, botModule);

                    foreach (var method in instance.GetType().GetMethods().Where(x => x.IsDefined(typeof(ChatSurvey))))
                    {
                        var att = method.GetCustomAttributes<ChatSurvey>().FirstOrDefault();

                        if (SurveyAnswersHandlers.ContainsKey(att))
                        {
                            Log.WriteLine($"ChatSurvey {method.Name}\n\t  already added", overrideColor: ConsoleColor.Cyan);
                            continue;
                        }
                        SurveyAnswersHandlers.Add(att, (ChatServeyMethod)Delegate.CreateDelegate(typeof(ChatServeyMethod), instance, method));
                        Log.WriteLine($"Loaded SurveyAnswersHandler {method.Name}\n", overrideColor: ConsoleColor.Green);

                    }
                    foreach (var method in instance.GetType().GetMethods().Where(x => x.IsDefined(typeof(ChatCommand))))
                    {
                        var att = method.GetCustomAttributes<ChatCommand>().First();
                        if (Commands.ContainsKey(att))
                        {
                            Log.WriteLine($"ChatCommand {method.Name}\n\t  with Trigger(s): {att.Triggers.Aggregate((a, b) => a + ", " + b)} not loaded, possible dublicate", overrideColor: ConsoleColor.Cyan);
                            continue;
                        }
                        Commands.Add(att, (ChatCommandMethod)Delegate.CreateDelegate(typeof(ChatCommandMethod), instance, method));
                        Log.WriteLine($"Loaded ChatCommand {method.Name}\n\t Trigger(s): {att.Triggers.Aggregate((a, b) => a + ", " + b)}", overrideColor: ConsoleColor.Green);

                    }
                    foreach (var m in botModule.GetMethods().Where(x => x.IsDefined(typeof(CallbackCommand))))
                    {
                        var att = m.GetCustomAttributes<CallbackCommand>().First();
                        if (CallbackCommands.ContainsKey(att))
                        {
                            Log.WriteLine($"Not loaded CallbackCommand {m.Name}\n\t Trigger: {att.Trigger}, possible dublicate", overrideColor: ConsoleColor.Cyan);
                            continue;
                        }
                        CallbackCommands.Add(att, (CallbackCommandMethod)Delegate.CreateDelegate(typeof(CallbackCommandMethod), instance, m));
                        Log.WriteLine($"Loaded CallbackCommand {m.Name}\n\t Trigger: {att.Trigger}", overrideColor: ConsoleColor.Green);
                    }
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(e.ToString(), LogLevel.Error, fileName: "error.log");
            }
        }
        private void MessengerOnMessageSent(object sender, EventArgs e)
        {
            var args = (e as MessageSentEventArgs);
            Send(args);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Log.WriteLine((e.ExceptionObject as Exception).Message, LogLevel.Error);
            if (ex != null)
                ExceptionHappened(ex);
        }

        public void Run()
        {
            Bot = new TelegramBotClient(LoadedSetting.TelegramBotAPIKey);
            try
            {
                //Load in the modules              
                LoadModules();
                if (String.IsNullOrEmpty(Options.WebHookUrl))
                {
                    Bot.DeleteWebhookAsync();
                    Bot.OnUpdate += BotOnUpdateReceived;
                    Bot.OnInlineQuery += BotOnOnInlineQuery;
                    Bot.OnCallbackQuery += BotOnOnCallbackQuery;
                    Bot.OnReceiveError += Bot_OnReceiveError;
                    Bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;
                    Bot.StartReceiving();
                }
                else
                {
                    var uri = new Uri(Options.WebHookUrl);
                    Bot.SetWebhookAsync(Options.WebHookUrl);
                }
                Me = Bot.GetMeAsync().Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteLine("502 bad gateway, restarting in 2 seconds", LogLevel.Error, fileName: "telegram.log");
                Log.WriteLine(ex.ToString(), LogLevel.Error, fileName: "telegram.log");
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }
            Log.WriteLine("Connected to Telegram and listening..." + Me.FirstName + Me.LastName);
        }

        private void Bot_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            Log.WriteLine(e.Exception.ToString(), LogLevel.Error, null, "error.log");
            SendMessageToAll(e.Exception.Message);

        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            Log.WriteLine(e.ApiRequestException.ToString(), LogLevel.Error, null, "error.log");
            SendMessageToAll(e.ApiRequestException.Message);
        }

        private void BotOnOnCallbackQuery(object sender, CallbackQuery query)
        {
            var trigger = query.Data.Split('|')[0];
            var args = query.Data.Replace(trigger + "|", "");
            var user = UserHelper.GetTelegramUser(Db, LoadedSetting.TelegramDefaultAdminUserId, cbQuery: query);


            //extract the trigger


            if (user.Grounded) return;
            Log.WriteLine(query.From.FirstName, LogLevel.Info, ConsoleColor.Cyan, "telegram.log");
            Log.WriteLine(query.Data, LogLevel.Info, ConsoleColor.White, "telegram.log");
            foreach (var callback in CallbackCommands)
            {
                if (String.Equals(callback.Key.Trigger, trigger, StringComparison.InvariantCultureIgnoreCase))
                {
                    var eArgs = new CallbackEventArgs()
                    {
                        SourceUser = user,                     
                        Parameters = args,
                        Target = query.Message.Chat.Id.ToString(),
                        Messenger = Messenger,
                        Bot = Bot,
                        Query = query
                    };
                    var response = callback.Value.Invoke(eArgs);
                    if (!String.IsNullOrWhiteSpace(response?.Text))
                    {
                        Send(response, query.Message, true);
                    }
                }
            }
            if (UsersWaitingAnswers.ContainsKey(query.Message.Chat.Id) && CurrentUserUpdatingObjects != null && CurrentUserUpdatingObjects.ContainsKey(query.Message.Chat.Id))
            {
                query.Message.Text = args;
                // query.Message.Type = MessageType.Text;
                var h = SurveyAnswersHandlers.FirstOrDefault(c => c.Key.Name == CurrentUserUpdatingObjects[query.Message.Chat.Id].GetType().Name);

                var customAnswerHandler = h.Value == null ? SurveyAnswersHandlers.FirstOrDefault() : h;
                var response = customAnswerHandler.Value.Invoke(query.Message);
                Send(response, query.Message);
                return;
            }
        }
        private void BotOnOnCallbackQuery(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {

            var query = callbackQueryEventArgs.CallbackQuery;
            var trigger = query.Data.Split('|')[0];
            var args = query.Data.Replace(trigger + "|", "");
            var user = UserHelper.GetTelegramUser(Db, LoadedSetting.TelegramDefaultAdminUserId, cbQuery: query);


            //extract the trigger


            if (user.Grounded) return;
            Log.WriteLine(query.From.FirstName, LogLevel.Info, ConsoleColor.Cyan, "telegram.log");
            Log.WriteLine(query.Data, LogLevel.Info, ConsoleColor.White, "telegram.log");
            foreach (var callback in CallbackCommands)
            {
                if (String.Equals(callback.Key.Trigger, trigger, StringComparison.InvariantCultureIgnoreCase))
                {
                    var eArgs = new CallbackEventArgs()
                    {
                        SourceUser = user,
                        Parameters = args,
                        Target = query.Message.Chat.Id.ToString(),
                        Messenger = Messenger,
                        Bot = Bot,
                        Query = query
                    };
                    var response = callback.Value.Invoke(eArgs);
                    if (!String.IsNullOrWhiteSpace(response?.Text))
                    {
                        Send(response, query.Message, true);
                    }
                }
            }
            if (UsersWaitingAnswers.ContainsKey(query.Message.Chat.Id) && CurrentUserUpdatingObjects != null && CurrentUserUpdatingObjects.ContainsKey(query.Message.Chat.Id))
            {
                query.Message.Text = args;
                // query.Message.Type = MessageType.Text;
                var h = SurveyAnswersHandlers.FirstOrDefault(c => c.Key.Name == CurrentUserUpdatingObjects[query.Message.Chat.Id].GetType().Name);

                var customAnswerHandler = h.Value == null ? SurveyAnswersHandlers.FirstOrDefault() : h;
                var response = customAnswerHandler.Value.Invoke(query.Message);
                Send(response, query.Message);
                return;
            }
        }
        private void BotOnOnInlineQuery(object sender, InlineQuery query)
        {
            try
            {

                new Thread(() => HandleQuery(query)).Start();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                var message = e.GetType() + " - " + e.Message;
                if (e is FileNotFoundException)
                    message += " file: " + ((FileNotFoundException)e).FileName;
                //else if (e is DirectoryNotFoundException)
                //    message += " file: " + ((DirectoryNotFoundException)e).;
                message += Environment.NewLine + e.StackTrace;
                Log.WriteLine($"Error in message handling: {message}", LogLevel.Error, fileName: "telegram.log");
            }
        }
        private void BotOnOnInlineQuery(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            try
            {
                var query = inlineQueryEventArgs.InlineQuery;

                new Thread(() => HandleQuery(query)).Start();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                var message = e.GetType() + " - " + e.Message;
                if (e is FileNotFoundException)
                    message += " file: " + ((FileNotFoundException)e).FileName;
                //else if (e is DirectoryNotFoundException)
                //    message += " file: " + ((DirectoryNotFoundException)e).;
                message += Environment.NewLine + e.StackTrace;
                Log.WriteLine($"Error in message handling: {message}", LogLevel.Error, fileName: "telegram.log");
            }
        }

        private async void HandleQuery(InlineQuery query)
        {
            var user = UserHelper.GetTelegramUser(Db, LoadedSetting.TelegramDefaultAdminUserId, null, query);
            if (user.Grounded)
            {
                await Bot.AnswerInlineQueryAsync(query.Id, new InlineQueryResultBase[]
                {
                    new InlineQueryResultArticle("0", "Nope!", new InputTextMessageContent("I did bad things, and now I'm grounded from the bot."))
                    {
                        Description = "You are grounded...",
                    }
                }, 0, true);
                return;
            }
            Log.WriteLine("INLINE QUERY", LogLevel.Info, ConsoleColor.Cyan, "telegram.log");
            Log.WriteLine(user.Name + ": " + query.Query, LogLevel.Info, ConsoleColor.White, "telegram.log");
            var com = GetParameters("/" + query.Query);
            var choices =
                Commands.Where(x => x.Key.DevOnly != true && x.Key.BotAdminOnly != true && x.Key.GroupAdminOnly != true & !x.Key.HideFromInline & !x.Key.DontSearchInline &&
                x.Key.Triggers.Any(t => t.ToLower().Contains(com[0].ToLower())) & !x.Key.DontSearchInline).ToList();
            choices.AddRange(Commands.Where(x => x.Key.DontSearchInline && x.Key.Triggers.Any(t => String.Equals(t, com[0], StringComparison.InvariantCultureIgnoreCase))));
            if (LoadedSetting.TelegramDefaultAdminUserId == user.UserId)
                choices.AddRange(Commands.Where(x => (x.Key.DevOnly || x.Key.BotAdminOnly) && x.Key.AllowInlineAdmin && x.Key.Triggers.Any(t => t == com[0])));
            if (user.IsBotAdmin)
                choices.AddRange(Commands.Where(x => x.Key.BotAdminOnly && x.Key.AllowInlineAdmin && x.Key.Triggers.Any(t => t == com[0])));

            var results = new List<InlineQueryResultBase>();
            foreach (var c in choices)
            {
                var response = c.Value.Invoke(new CommandEventArgs
                {
                    SourceUser = user,                    
                    Parameters = com[1],
                    Target = "",
                    Messenger = Messenger,
                    Bot = Bot,
                    Message = null
                });
                var title = c.Key.Triggers[0];
                var description = c.Key.HelpText;
                if (query.Query.Split(' ').Length > 1 || c.Key.DontSearchInline || c.Key.Triggers.Any(x => String.Equals(x, com[0], StringComparison.InvariantCultureIgnoreCase)))
                {
                    description = response.ImageDescription ?? description;
                    title = response.ImageTitle ?? title;
                }

                results.Add(new InlineQueryResultArticle(Commands.ToList().IndexOf(c).ToString(), title, new InputTextMessageContent(response.Text)
                {
                    DisableWebPagePreview = false,
                    ParseMode = response.ParseMode
                })
                {
                    Description = description,
                    Title = title,
                    ThumbUrl = response.ImageUrl,
                    Url = response.ImageUrl,
                    HideUrl = true
                });

            }
            var menu = results.ToArray();
            try
            {
                await Bot.AnswerInlineQueryAsync(query.Id, menu, 0, true);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.InnerExceptions[0].Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public virtual void OnWebHookUpdate(Update update)
        {
            try
            {
                if (!((update.Message?.Date ?? update.CallbackQuery?.Message?.Date) > DateTime.UtcNow.AddSeconds(-15)))
                {
                    return;
                }
                if (update.Type == UpdateType.CallbackQuery)
                {
                    BotOnOnCallbackQuery("webhook", update.CallbackQuery);
                    return;
                }
                if (update.Type == UpdateType.InlineQuery)
                {
                    BotOnOnInlineQuery("webhook", update.InlineQuery);
                    return;
                }

                if (update.Type == UpdateType.PreCheckoutQuery || update.Message?.SuccessfulPayment != null)
                {
                    new Thread(() => HandlePreCheckout(update)).Start();
                    return;
                }
                if (update.Type == UpdateType.InlineQuery) return;


                new Thread(() => Handle(update)).Start();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                var message = e.GetType() + " - " + e.Message;
                if (e is FileNotFoundException)
                    message += " file: " + ((FileNotFoundException)e).FileName;
                //else if (e is DirectoryNotFoundException)
                //    message += " file: " + ((DirectoryNotFoundException)e).;
                message += Environment.NewLine + e.StackTrace;
                Log.WriteLine($"Error in message handling: {message}", LogLevel.Error, fileName: "telegram.log");
            }
        }
        private void BotOnUpdateReceived(object sender, UpdateEventArgs updateEventArgs)
        {
            try
            {
                var update = updateEventArgs.Update;

                if (update.Type == UpdateType.PreCheckoutQuery || update.Message?.SuccessfulPayment != null)
                {
                    new Thread(() => HandlePreCheckout(update)).Start();
                    return;
                }
                if (update.Type == UpdateType.InlineQuery) return;
                //if (update.Type == UpdateType.CallbackQuery) return;

                if (!(update.Message?.Date > DateTime.UtcNow.AddSeconds(-15)))
                {
                    //Log.WriteLine("Ignoring message due to old age: " + update.Message.Date);
                    return;
                }
                new Thread(() => Handle(update)).Start();
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;
                var message = e.GetType() + " - " + e.Message;
                if (e is FileNotFoundException)
                    message += " file: " + ((FileNotFoundException)e).FileName;
                //else if (e is DirectoryNotFoundException)
                //    message += " file: " + ((DirectoryNotFoundException)e).;
                message += Environment.NewLine + e.StackTrace;
                Log.WriteLine($"Error in message handling: {message}", LogLevel.Error, fileName: "telegram.log");
            }
        }
        internal void HandlePreCheckout(Update update)
        {
            if (update.PreCheckoutQuery != null)
            {
                var invoice = UserInvoices.FirstOrDefault(c => c.PayloadId == update.PreCheckoutQuery.InvoicePayload);
                if (invoice != null)
                {
                    invoice.TelegramId = update.PreCheckoutQuery.Id;
                    Bot.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id);
                }
            }
            if (update?.Message?.SuccessfulPayment != null)
            {
                var payedInvoice = UserInvoices.FirstOrDefault(c => c.PayloadId == update.Message.SuccessfulPayment.InvoicePayload);
                if (payedInvoice != null)
                {
                    payedInvoice.PaymentProviderId = update.Message.SuccessfulPayment.ProviderPaymentChargeId;
                    SendMessageToAll($"Payment received:\n{JsonConvert.SerializeObject(payedInvoice)}");

                    Send(new MessageSentEventArgs()
                    {
                        Target = payedInvoice.UserId.ToString(),
                        Response = new CommandResponse($"Your payment was received.\n" +
                        $"`Id: {payedInvoice.PaymentProviderId}`\n" +
                        $"`Summ: {update.Message.SuccessfulPayment.TotalAmount / 100} {payedInvoice.Currency}`", parseMode: ParseMode.Markdown)
                    });

                    OnPaymentReceived?.Invoke(payedInvoice);
                    Thread.Sleep(400);
                    UserInvoices.RemoveAll(c => c.PayloadId == update.Message.SuccessfulPayment.InvoicePayload);
                }
            }
        }
        public void SendInvoice(InvoiceDto invoice)
        {
            if (String.IsNullOrEmpty(Options.PaymentToken))
            {
                SendMessageToAll("Need to provide payment token");
                return;
            }
            UserInvoices.Add(invoice);
            Bot.SendInvoiceAsync((int)invoice.UserId, invoice.Title, invoice.Description, invoice.PayloadId, Options.PaymentToken, invoice.PayloadId, invoice.Currency, invoice.Goods);
        }
        internal void Handle(Update update)
        {
            try
            {

                if (update.Type == UpdateType.CallbackQuery && String.IsNullOrEmpty(update.Message.Text))
                {
                    var trigger = update.CallbackQuery.Data.Split('|')[0];
                    var args = update.CallbackQuery.Data.Replace(trigger + "|", "");

                    update.Message.Text = args;
                }
                var msg = (update.Message.From.Username ?? update.Message.From.FirstName) + ": " + update.Message.Text;
                var chat = update.Message.Chat.Title;
                if (String.IsNullOrWhiteSpace(chat))
                    chat = "Private Message";

                var user = UserHelper.GetTelegramUser(Db, LoadedSetting.TelegramDefaultAdminUserId, update);

                if (user.Grounded) return;
                TelegramBotGroup group;
                if (update.Message.Chat.Type != ChatType.Private)
                {
                    group = GroupHelper.GetGroup(Db, update);
                }
                Log.WriteLine(chat, LogLevel.Info, ConsoleColor.Cyan, "telegram.log");
                Log.WriteLine(msg, LogLevel.Info, ConsoleColor.White, "telegram.log");

                if (Options.ShouldApprooveNewUsers)
                {
                    if (!user.IsBotAdmin)
                    {
                        Bot.SendTextMessageAsync(update.Message.Chat, "You must be approved to use this bot, write to admin");
                        return;
                    }
                }
                if (UsersWaitingAnswers.ContainsKey(update.Message.Chat.Id) && UsersWaitingAnswers[update.Message.Chat.Id].Count > 0)
                {
                    if (!SurveyAnswersHandlers.Any())
                    {
                        Send(new MessageSentEventArgs() { Target = LoadedSetting.TelegramDefaultAdminUserId.ToString(), Response = new CommandResponse($"Here is any answer handlers for \n{JsonConvert.SerializeObject(UsersWaitingAnswers[update.Message.Chat.Id])}") });
                        return;
                    }
                    if (SurveyAnswersHandlers.Any(c => c.Key.Name == CurrentUserUpdatingObjects[update.Message.Chat.Id].GetType().Name))
                    {
                        if (update.Type == UpdateType.CallbackQuery && String.IsNullOrEmpty(update.Message.Text))
                        {
                            var trigger = update.CallbackQuery.Data.Split('|')[0];
                            var args = update.CallbackQuery.Data.Replace(trigger + "|", "");
                            update.Message.Text = args;
                        }
                        var customAnswerHandler = SurveyAnswersHandlers.FirstOrDefault(c => c.Key.Name == CurrentUserUpdatingObjects[update.Message.Chat.Id].GetType().Name);
                        var response = customAnswerHandler.Value.Invoke(update.Message);
                        Send(response, update.Message);
                    }
                    else
                    {
                        var customAnswerHandler = SurveyAnswersHandlers.FirstOrDefault();
                        var response = customAnswerHandler.Value.Invoke(update.Message);
                        Send(response, update.Message);
                    }
                    return;
                }
                if (update.Message.Text.StartsWith("!") || update.Message.Text.StartsWith("/"))
                {
                    var args = GetParameters(update.Message.Text);
                    foreach (var command in Commands)
                    {
                        if (command.Key.Triggers.Contains(args[0].ToLower()))
                        {
                            //check for access
                            var att = command.Key;
                            if (att.DevOnly &&
                                update.Message.From.Id != LoadedSetting.TelegramDefaultAdminUserId)
                            {
                                Send(new CommandResponse("You are not the developer!"), update);
                                return;
                            }
                            if (att.BotAdminOnly & !user.IsBotAdmin & LoadedSetting.TelegramDefaultAdminUserId != update.Message.From.Id)
                            {
                                Send(new CommandResponse("You are not a bot admin!"), update);
                                return;
                            }
                            if (att.GroupAdminOnly)
                            {
                                if (update.Message.Chat.Type == ChatType.Private)
                                {
                                    Send(new CommandResponse("You need to run this in a group"), update);
                                    return;
                                }
                                //is the user an admin of the group?
                                var status =
                                    Bot.GetChatMemberAsync(update.Message.Chat.Id, update.Message.From.Id)
                                        .Result.Status;
                                if (status != ChatMemberStatus.Administrator && status != ChatMemberStatus.Creator)
                                {
                                    Send(new CommandResponse("You are not a group admin!"), update);
                                    return;
                                }
                            }
                            if (att.InGroupOnly && update.Message.Chat.Type == ChatType.Private)
                            {
                                Send(new CommandResponse("You need to run this in a group"), update);
                                return;
                            }
                            if (att.InPrivateOnly)
                            {
                                Send(new CommandResponse("You need to run this in private"), update);
                                return;
                            }
                            var eArgs = new CommandEventArgs
                            {
                                SourceUser = user,
                              
                                Parameters = args[1],
                                Target = update.Message.Chat.Id.ToString(),
                                Messenger = Messenger,
                                Bot = Bot,
                                Message = update.Message
                            };
                            var response = command.Value.Invoke(eArgs);
                            if (!String.IsNullOrWhiteSpace(response.Text))
                                Send(response, update);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("Exception happend at handling update:\n" + ex.ToString(), LogLevel.Error, ConsoleColor.Cyan, "error.log");
            }

        }


        private static string[] GetParameters(string input)
        {
            if (String.IsNullOrEmpty(input)) return new[] { "", "" };
            var result = input.Contains(" ") ? new[] { input.Substring(1, input.IndexOf(" ")).Trim(), input.Substring(input.IndexOf(" ") + 1) } : new[] { input.Substring(1).Trim(), null };
            result[0] = result[0].Replace("@" + Me.Username, "");
            return result;
        }

        public void Send(CommandResponse response, Update update, bool edit = false)
        {
            Send(response, update.Message, edit);
        }

        public void Send(CommandResponse response, Message update, bool edit = false)
        {
            var text = response.Text;
            Log.WriteLine("Replying: " + text, overrideColor: ConsoleColor.Yellow);
            try
            {
                if (text.StartsWith("/me"))
                {
                    text = text.Replace("/me", "*") + "*";
                }
                var targetId = response.Level == ResponseLevel.Public ? update.Chat.Id : update.From.Id;
                if (edit && targetId == update.Chat.Id)
                {
                    Bot.EditMessageTextAsync(targetId, update.MessageId, text,
                        replyMarkup: CreateMarkupFromMenu(response.Menu),
                        parseMode: response.ParseMode);
                }
                else
                {
                    Bot.SendTextMessageAsync(targetId, text, replyMarkup: CreateMarkupFromMenu(response.Menu),
                        parseMode: response.ParseMode);
                }
                //Bot.SendTextMessage(update.Message.Chat.Id, text);
                return;
            }
            catch
            {

            }
        }

        public async Task SendAsync(MessageSentEventArgs args)
        {
            Log.WriteLine("Replying: " + args.Response.Text, overrideColor: ConsoleColor.Yellow);
            var text = args.Response.Text;
            try
            {
                if (text.StartsWith("/me"))
                {
                    text = text.Replace("/me", "*") + "*";
                }
                if (text.StartsWith("/"))
                {
                    text = text.Substring(1);
                }

                if (long.TryParse(args.Target, out var targetId))
                {
                    await Bot.SendTextMessageAsync(targetId, text, replyMarkup: CreateMarkupFromMenu(args.Response.Menu), parseMode: args.Response.ParseMode, disableNotification: args.IsSilent);
                }

                return;
            }

            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString(), LogLevel.Error, null, "error.log");
            }
        }
        public void Send(MessageSentEventArgs args)
        {
            Log.WriteLine("Replying: " + args.Response.Text, overrideColor: ConsoleColor.Yellow);
            var text = args.Response.Text;
            try
            {
                if (text.StartsWith("/me"))
                {
                    text = text.Replace("/me", "*") + "*";
                }
                if (text.StartsWith("/"))
                {
                    text = text.Substring(1);
                }

                if (long.TryParse(args.Target, out var targetId))
                {
                    var r = Bot.SendTextMessageAsync(targetId, text, replyMarkup: CreateMarkupFromMenu(args.Response.Menu), parseMode: args.Response.ParseMode, disableNotification: args.IsSilent).Result;
                }
                return;
            }

            catch (Exception ex)
            {

                Log.WriteLine(ex.ToString(), LogLevel.Error, null, "error.log");
            }
        }
        public InlineKeyboardMarkup CreateMarkupFromMenu(Menu menu)
        {
            if (menu == null) return null;
            var col = menu.Columns - 1;
            //this is gonna be fun...
            var final = new List<InlineKeyboardButton[]>();
            for (var i = 0; i < menu.Buttons.Count; i++)
            {
                var row = new List<InlineKeyboardButton>();
                do
                {
                    var cur = menu.Buttons[i];
                    row.Add(new InlineKeyboardButton
                    {
                        Text = cur.Text,
                        CallbackData = $"{cur.Trigger}|{cur.ExtraData}",
                        Url = cur.Url
                    });
                    i++;
                    if (i == menu.Buttons.Count) break;
                } while (i % (col + 1) != 0);
                i--;
                final.Add(row.ToArray());
                if (i == menu.Buttons.Count) break;
            }
            return new InlineKeyboardMarkup(final.ToArray());
        }
        public void SendMessageToAll(string message, bool onlyAdmins = false, bool onlydev = true, bool isSilent = false)
        {
            lock (this)
            {
                try
                {
                    using(var db = Db)
                    {
                        var users = db.Users.AsNoTracking().AsEnumerable();
                        if (onlyAdmins)
                            users = users.Where(c => c.IsBotAdmin);
                        if (onlydev)
                            users = users.Where(c => c.UserId == LoadedSetting.TelegramDefaultAdminUserId);
                        foreach (var user in users.ToList())
                        {
                            Send(new MessageSentEventArgs(isSilent) { Response = new CommandResponse(message, ResponseLevel.Private, parseMode: ParseMode.Markdown), Target = user.UserId.ToString() });
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    Send(new MessageSentEventArgs() { Response = new CommandResponse(message, ResponseLevel.Private, parseMode: ParseMode.Markdown), Target = LoadedSetting.TelegramDefaultAdminUserId.ToString() });
                    Send(new MessageSentEventArgs() { Response = new CommandResponse($"Failed with `{ex.ToString()}`", ResponseLevel.Private, parseMode: ParseMode.Markdown), Target = LoadedSetting.TelegramDefaultAdminUserId.ToString() });
                }
            }
        }

        public void SendMessage(string message, long userId, bool isSilent)
        {
            Bot.SendTextMessageAsync(userId, message, disableNotification: isSilent);
        }

        public async Task SendMessageAsync(string message, long userId, bool isSilent)
        {
            await Bot.SendTextMessageAsync(userId, message, disableNotification: isSilent);
        }
    }
}
