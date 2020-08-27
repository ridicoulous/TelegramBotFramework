using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.DefaultModules;

namespace TelegramBotFramework.Core.DefaultModules
{

    public abstract class BaseSurveyModule<TSurvey, TBotWrapper> :
        TelegramBotModuleBase<TBotWrapper>, IBaseSurveyModule<TSurvey> where TBotWrapper : ITelegramBotWrapper where TSurvey : class, new()
    {
        public BaseSurveyModule(TBotWrapper wrapper) : base(wrapper)
        {

        }
        public abstract void SubmitSurvey(long userId, TSurvey survey);

        public virtual CommandResponse AbortSurvey(long userId, string reason="command was catched")
        {
            BotWrapper.UsersWaitingAnswers.Remove(userId);
            BotWrapper.CurrentUserUpdatingObjects.Remove(userId);
            return new CommandResponse($"Survey has been aborted cause {reason}");
        }
        public virtual CommandResponse InitServey<T>(long userId, T instanse = null) where T : class, new()
        {
            BotWrapper.IsSurveyInitiated = true;
            if (BotWrapper.CurrentUserUpdatingObjects.ContainsKey(userId))
            {
                BotWrapper.CurrentUserUpdatingObjects.Remove(userId);
            }
            BotWrapper.CurrentUserUpdatingObjects.Add(userId, instanse ?? new T());

            var questions = typeof(T).GetProperties().Where(p => p.IsDefined(typeof(SurveyAttribute)));
            List<SurveyAttribute> attributes = new List<SurveyAttribute>();
            foreach (var t in questions.OrderBy(c => c.Name))
            {
                var survey = t.GetCustomAttributes<SurveyAttribute>().First();
                string allowedAnswers = "Enter";
                if (survey.Choises != null && survey.Choises.Any())
                {
                    allowedAnswers = $"Choose";
                }
                survey.QuestionText = String.IsNullOrEmpty(survey.QuestionText) ? $"{allowedAnswers} value of `{t.Name}`:" : survey.QuestionText;
                survey.UpdatingPropertyName = t.Name;
                attributes.Add(survey);
            }
            return SendQuestion(userId, attributes.ToList());
        }
        public virtual CommandResponse SendQuestion(long userId, List<SurveyAttribute> questions = null)
        {
            if (!BotWrapper.UsersWaitingAnswers.ContainsKey(userId))
            {
                var queue = new Queue<SurveyAttribute>();
                foreach (var q in questions)
                {
                    queue.Enqueue(q);
                }
                BotWrapper.UsersWaitingAnswers.Add(userId, queue);
            }
            else
            {
                if (BotWrapper.UsersWaitingAnswers[userId].Count == 0)
                {             
                    SubmitSurvey(userId, BotWrapper.CurrentUserUpdatingObjects.GetValue<TSurvey>(userId));
                    BotWrapper.UsersWaitingAnswers.Remove(userId);
                    return new CommandResponse("Thank you, your answers was saved");
                }
            }         
            Menu menu = null;
            if (BotWrapper.UsersWaitingAnswers[userId].Peek().Choises.Any())
            {
                menu = CreateButtonsWithCallback("choose", BotWrapper.UsersWaitingAnswers[userId].Peek().Choises);
            }

            return new CommandResponse($"{BotWrapper.UsersWaitingAnswers[userId].Peek().QuestionText}", menu: menu, parseMode: ParseMode.Markdown);
        }
        public virtual bool HandleResponse(Message message)
        {
            try
            {                
                var question = BotWrapper.UsersWaitingAnswers[message.Chat.Id].Peek();
                if (question.Choises.Any() && !question.Choises.Contains(message.Text.Trim()))
                {
                    BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Catched error at handling ansver: `Submitted {message.Text} is not allowed value`", ParseMode.Markdown).Wait();
                    return false;
                }
                PropertyInfo propertyInfo = BotWrapper.CurrentUserUpdatingObjects[message.Chat.Id].GetType().GetProperty(question.UpdatingPropertyName);
                if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(double) || propertyInfo.PropertyType == typeof(float))
                {
                    message.Text = message.Text.Replace(",", ".");
                }
                propertyInfo.SetValue(BotWrapper.CurrentUserUpdatingObjects[message.Chat.Id],
                    Convert.ChangeType(message.Text, propertyInfo.PropertyType, CultureInfo.GetCultureInfo("en-US")), null);
                var t = propertyInfo.GetValue(BotWrapper.CurrentUserUpdatingObjects[message.Chat.Id]);

                if (LastAnswerMessageId.ContainsKey(message.Chat.Id))
                {
                    LastAnswerMessageId[message.Chat.Id] = message.MessageId;
                }
                else
                {
                    LastAnswerMessageId.TryAdd(message.Chat.Id, message.MessageId);
                }
                return true;
            }
            catch (Exception ex)
            {
                BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Catched error at handling ansver: `{ex.Message}`", ParseMode.Markdown).Wait();
                if (LastAnswerMessageId.ContainsKey(message.Chat.Id))
                {
                    LastAnswerMessageId[message.Chat.Id] = message.MessageId;
                }
                else
                {
                    LastAnswerMessageId.TryAdd(message.Chat.Id, message.MessageId);
                }
                return false;
            }
        }
        private ConcurrentDictionary<long, int> LastAnswerMessageId = new ConcurrentDictionary<long, int>();
        private ConcurrentDictionary<long, int> LastQuestionMessageId = new ConcurrentDictionary<long, int>();

        [ChatSurvey(Name = "DefaultAnswerHandler")]
        public virtual CommandResponse GetAnswer(Message message)
        {           
            BotWrapper.Bot.SendChatActionAsync(message.Chat, ChatAction.Typing);
            Thread.Sleep(300);
            //if (message.Chat.Id != message.From.Id && message.From.IsBot)
            //{
            //    return AbortSurvey(message.Chat.Id, $"looks like message ({message.Text}) was recieved from bot: {message.From.Id}!={message.Chat.Id }");
            //}
            if (message.Text.StartsWith('/'))
            {
                return AbortSurvey(message.Chat.Id);
            }
            var question = BotWrapper.UsersWaitingAnswers[message.Chat.Id].Peek();
            if (HandleResponse(message))
            {
                BotWrapper.UsersWaitingAnswers[message.Chat.Id].Dequeue();        
                BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Answer for {question.QuestionText} accepted", ParseMode.Markdown).Wait();
            }
            else
            {                
                BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Answer for {question.QuestionText} was not accepted, try again, please", ParseMode.Markdown).Wait();
            }
            try
            {
                if(LastAnswerMessageId.ContainsKey(message.Chat.Id))
                    BotWrapper.Bot.DeleteMessageAsync(message.Chat, LastAnswerMessageId[message.Chat.Id]).Wait();
            }
            catch { }
            return SendQuestion(message.Chat.Id);
        }
    }
}
