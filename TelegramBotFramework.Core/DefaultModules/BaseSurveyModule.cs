using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.DefaultModules;

namespace TelegramBotFramework.Core.DefaultModules
{
    
    public abstract class BaseSurveyModule<TSurvey, TBotWrapper> : TelegramBotModuleBase<TBotWrapper> where TBotWrapper : ITelegramBotWrapper where TSurvey : class, new()
    {
        public BaseSurveyModule(TBotWrapper wrapper) : base(wrapper)
        {         
          
        }
        public virtual CommandResponse CreateSurvay(CommandEventArgs args)
        {
            return InitServey<TSurvey>(args.SourceUser.UserId);
        }
        public abstract void SubmitSurvey(TSurvey survey);

        public virtual CommandResponse InitServey<T>(long userId) where T : class, new()
        {
            BotWrapper.IsSurveyInitiated = true;
            if (BotWrapper.CurrentUserUpdatingObjects.ContainsKey(userId))
            {
                BotWrapper.CurrentUserUpdatingObjects.Remove(userId);
            }
            BotWrapper.CurrentUserUpdatingObjects.Add(userId, new T());

            var questions = typeof(T).GetProperties().Where(p => p.IsDefined(typeof(SurveyAttribute)));
            List<SurveyAttribute> attributes = new List<SurveyAttribute>();
            foreach (var t in questions)
            {
                var survey = t.GetCustomAttributes<SurveyAttribute>().First();
                string allowedAnswers = "";
                if (survey.Choises != null && survey.Choises.Any())
                {
                    allowedAnswers = $"*[Allowed values:{String.Join(",", survey.Choises)}]*\n";
                }
                survey.QuestionText = String.IsNullOrEmpty(survey.QuestionText) ? $"{allowedAnswers}Enter value of `{t.Name}`:" : survey.QuestionText;
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
                    BotWrapper.AnswerHandling = false;
                    //   OnSurveyComplete?.Invoke(userId);
                    SubmitSurvey(BotWrapper.CurrentUserUpdatingObjects.GetValue<TSurvey>(userId));
                    BotWrapper.UsersWaitingAnswers.Remove(userId);
                    return new CommandResponse("Thank you, your answers was saved");
                }
                //   return new CommandResponse("");
            }
            BotWrapper.AnswerHandling = true;
            return new CommandResponse($"{BotWrapper.UsersWaitingAnswers[userId].Peek().QuestionText}", parseMode: ParseMode.Markdown);
        }
        public virtual bool HandleResponse(Message message)
        {
            try
            {
                var question = BotWrapper.UsersWaitingAnswers[message.Chat.Id].Peek();
                if (question.Choises.Any() && !question.Choises.Contains(message.Text.Trim()))
                {
                    BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Catched error at handling ansver: `Submitted {message.Text} does not allowed value`", ParseMode.Markdown);
                    return false;
                }
                PropertyInfo propertyInfo = BotWrapper.CurrentUserUpdatingObjects[message.Chat.Id].GetType().GetProperty(question.UpdatingPropertyName);
                propertyInfo.SetValue(BotWrapper.CurrentUserUpdatingObjects[message.Chat.Id], Convert.ChangeType(message.Text, propertyInfo.PropertyType), null);
                return true;
            }
            catch (Exception ex)
            {
                BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Catched error at handling ansver: `{ex.Message}`", ParseMode.Markdown);
                return false;
            }
        }
        [ChatSurvey(Name ="DefaultAnswerHandler")]
        public virtual CommandResponse GetAnswer(Message message)
        {
            BotWrapper.Bot.SendChatActionAsync(message.Chat, ChatAction.Typing);
            var question = BotWrapper.UsersWaitingAnswers[message.Chat.Id].Peek();
            if (HandleResponse(message))
            {
                BotWrapper.UsersWaitingAnswers[message.Chat.Id].Dequeue();
                BotWrapper.Bot.DeleteMessageAsync(message.Chat, message.MessageId - 1).Wait();
                BotWrapper.Bot.DeleteMessageAsync(message.Chat, message.MessageId).Wait();
                BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Answer for {question.QuestionText} accepted", ParseMode.Markdown);
            }
            else
            {
                BotWrapper.Bot.DeleteMessageAsync(message.Chat, message.MessageId - 1).Wait();
                BotWrapper.Bot.DeleteMessageAsync(message.Chat, message.MessageId).Wait();
                BotWrapper.Bot.SendTextMessageAsync(message.Chat, $"Answer for {question.QuestionText} was not accepted, try again, please", ParseMode.Markdown);
            }
            return SendQuestion(message.Chat.Id);
        }
    }
}
