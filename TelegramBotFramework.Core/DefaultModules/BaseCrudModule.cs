using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TelegramBotFramework.Core.Extensions;
using TelegramBotFramework.Core.Helpers;
using TelegramBotFramework.Core.Interfaces;
using TelegramBotFramework.Core.Objects;
using TelegramBotFramework.Core.SQLiteDb;
using Telegram.Bot.Extensions;
using Telegram.Bot;
using System.Threading;

namespace TelegramBotFramework.Core.DefaultModules
{

    public abstract class BaseCrudModule<TBotWrapper> : TelegramBotModuleBase<TBotWrapper>, ITelegramBotCrudModule
        where TBotWrapper : ITelegramBotWrapper

    {
        private readonly ConcurrentDictionary<int, Type> _currentUpdatingEntryType = new();
        private readonly ConcurrentDictionary<int, object> _currentUpdatingEntryValue = new();
        private readonly ConcurrentDictionary<int, string> _currentUpdatingFieldName = new();
        private readonly ConcurrentDictionary<int, string> _currentUpdatingPrimaryKeyName = new();

        public BaseCrudModule(TBotWrapper wrapper) : base(wrapper)
        {

        }

        [ChatCommand(Triggers = new[] { "crud" }, BotAdminOnly = true, HelpText = "Entities updating")]
        public CommandResponse GetAllEntitesForCrud(CommandEventArgs args)
        {
            var entitesToEdit = GetEditableEntites();
            return new CommandResponse("Choose entity set for edit:", menu: GenerateMenuForEntity(entitesToEdit));
        }
        [CallbackCommand(Trigger = BotCrudActions.EditEntity, BotAdminOnly = true)]
        public CommandResponse OnEntityTypeChoosed(CallbackEventArgs args)
        {
            using (var db = BotWrapper.Db)
            {
                var typeOfEntitySetToEdit = LoadEntityTypeByName(args.Parameters, db);

                if (typeOfEntitySetToEdit == null || !typeOfEntitySetToEdit.IsAssignableTo(typeof(IEditableEntity)))
                {
                    return new CommandResponse("Something went wrong");
                }

                AddEditingEntityType(args.SourceUser.Id, typeOfEntitySetToEdit);
                var firstValueToGetPrimaryKeyName = db.GetFirstOrDefault(typeOfEntitySetToEdit);
                if (firstValueToGetPrimaryKeyName != null)
                {
                    var pk = firstValueToGetPrimaryKeyName as IEditableEntity;
                    if (pk != null)
                    {
                        AddPkName(args.SourceUser.Id, pk.PrimaryKeyName);
                    }
                }
                var entitesToEdit = db.Set(_currentUpdatingEntryType[args.SourceUser.Id]).ToList();
                var menu = new Menu();
                menu.Columns = 1;
                foreach (var e in entitesToEdit)
                {
                    var id = GetPropertyValue(typeOfEntitySetToEdit, e, _currentUpdatingPrimaryKeyName[args.SourceUser.Id]);
                    if (id == null)
                    {
                        continue;
                    }
                    var name = e as IEditableEntity;
                   
                    menu.Buttons.Add(new InlineButton($"{name.EntityReadableName}", BotCrudActions.ChooseEntityForEditById, id.ToString()));
                }
                return new CommandResponse("Choose particular entity:", menu: menu);
            }
        }
        [CallbackCommand(Trigger = BotCrudActions.ChooseEntityForEditById, BotAdminOnly = true)]
        public CommandResponse OnEntityEntryChoosed(CallbackEventArgs args)
        {
            using (var db = BotWrapper.Db)
            {
                try
                {                    
                    var entityValue = LoadEntityValuebyId(id: args.Parameters,
                               primaryKeyName: _currentUpdatingPrimaryKeyName[args.SourceUser.Id],
                               entityType: _currentUpdatingEntryType[args.SourceUser.Id],
                               db: db);

                    AddCurrentUpdatingValue(args.SourceUser.Id, entityValue);
                    var menu = new Menu();
                    menu.Columns = 1;
                    var pkName = _currentUpdatingPrimaryKeyName[args.SourceUser.Id];
                    foreach (var e in entityValue.AsDictionary(fieldsToIgnore:pkName))
                    {
                        menu.Buttons.Add(new InlineButton($"{e.Key}:\t{e.Value}", BotCrudActions.FieldForEditChoosed, e.Key));
                    }
                    return new CommandResponse("Choose entity field for edit:", menu: menu);
                }
                catch (Exception ex)
                {
                    return new CommandResponse($"Error occured: {ex.Message}");
                }
                
            }
        }
        [CallbackCommand(Trigger = BotCrudActions.FieldForEditChoosed, BotAdminOnly = true)]
        public CommandResponse OnFieldChoosed(CallbackEventArgs args)
        {
            try
            {
                AddFieldForEditName(args.SourceUser.Id, args.Parameters);
                var entryValue = _currentUpdatingEntryValue[args.SourceUser.Id] as IEditableEntity;
                return new CommandResponse($"Write `{args.Parameters}` field of `{entryValue.EntityReadableName}` value:", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                return new CommandResponse($"Error occured: {ex.Message}");
            }
        }

        private object GetPropertyValue(Type objectType, object src, string propName)
        {
            return objectType.GetProperty(propName).GetValue(src, null);
        }
        private void AddPkName(int userId, string pkName)
        {
            if (_currentUpdatingPrimaryKeyName.ContainsKey(userId))
            {
                _currentUpdatingPrimaryKeyName[userId] = pkName;
            }
            else
            {
                _currentUpdatingPrimaryKeyName.TryAdd(userId, pkName);
            }
        }
        private void AddEditingEntityType(int userId, Type entityType)
        {
            if (_currentUpdatingEntryType.ContainsKey(userId))
            {
                _currentUpdatingEntryType[userId] = entityType;
            }
            else
            {
                _currentUpdatingEntryType.TryAdd(userId, entityType);
            }
        }
        private void AddFieldForEditName(int userId, string fieldName)
        {
            if (_currentUpdatingFieldName.ContainsKey(userId))
            {
                _currentUpdatingFieldName[userId] = fieldName;
            }
            else
            {
                _currentUpdatingFieldName.TryAdd(userId, fieldName);
            }
        }
        private void AddCurrentUpdatingValue(int userId, object value)
        {
            if (_currentUpdatingEntryValue.ContainsKey(userId))
            {
                _currentUpdatingEntryValue[userId] = value;
            }
            else
            {
                _currentUpdatingEntryValue.TryAdd(userId, value);
            }
        }
        public Type LoadEntityTypeByName(string entityName, DbContext db)
        {
            var type = db.Model.FindEntityType(entityName);
            if (type != null)
                return type.ClrType;
            else
            {
                return null;
            }
        }

        public object LoadEntityValuebyId(object id, string primaryKeyName, Type entityType, DbContext db)
        {
            return db.GetEntityByTypeAndId(id, primaryKeyName, entityType);
        }

        private Menu GenerateMenuForEntity(IEnumerable<Type> entities, string action = BotCrudActions.EditEntity)
        {
            var menu = new Menu()
            {
                Columns = 1,
                Buttons = entities.Select(f => new InlineButton($"Edit: {f.Name}", action, f.FullName)).ToList()
            };
            return menu;
        }

        public IEnumerable<Type> GetEditableEntites()
        {
            using (var context = BotWrapper.Db)
            {
                var types = context.Model.GetEntityTypes();
                List<Type> entities = new List<Type>();
                foreach (var t in types)
                {
                    if (t.ClrType.IsAssignableTo(typeof(IEditableEntity)))
                    {
                        entities.Add(t.ClrType);
                    }
                }
                return entities;
            }
        }

        public bool IsCurrentUserSubmitsEntityFieldValue(int userId)
        {
            return _currentUpdatingEntryType.ContainsKey(userId)
                && _currentUpdatingEntryValue.ContainsKey(userId)
                && _currentUpdatingFieldName.ContainsKey(userId)
                && _currentUpdatingPrimaryKeyName.ContainsKey(userId);
        }

        public CommandResponse SubmitValue(int userId, string userInput)
        {
            try
            {
                PropertyInfo propertyInfo = _currentUpdatingEntryType[userId].GetProperty(_currentUpdatingFieldName[userId]);

                propertyInfo.SetValue(_currentUpdatingEntryValue[userId],
                    Convert.ChangeType(userInput, propertyInfo.PropertyType, CultureInfo.GetCultureInfo("en-US")), null);
                using (var db = BotWrapper.Db)
                {
                    db.Update(_currentUpdatingEntryValue[userId]);
                    db.SaveChanges();

                    OnEntityUpdated(_currentUpdatingEntryType[userId], _currentUpdatingEntryValue[userId]);
                }
                var entryValue = _currentUpdatingEntryValue[userId] as IEditableEntity;

                return new CommandResponse($"Value `{userInput}` for `{entryValue.EntityReadableName}.{_currentUpdatingFieldName[userId]}` was saved", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            catch (Exception ex)
            {
                return new CommandResponse($"Value `{userInput}` for `{_currentUpdatingFieldName[userId]}` was not saved: {ex.ToString()}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
            finally
            {
                Clear(userId);
            }
        }
        protected abstract void OnEntityUpdated(Type entityType, object entityValue);
        public void Clear(int userId)
        {
            _currentUpdatingEntryType.Remove(userId, out _);
            _currentUpdatingEntryValue.TryRemove(userId, out _);
            _currentUpdatingFieldName.TryRemove(userId, out _);
            _currentUpdatingPrimaryKeyName.TryRemove(userId, out _);
        }
    }
}
