using System;
using System.Collections.Generic;
using System.Text;
using TelegramBotFramework.Core.Objects;

namespace TelegramBotFramework.Core.Interfaces
{   
    public interface IBotSurvey
    {
       
    }
    public class UsersSurveys
    {
        private Dictionary<long, object> _dict = new Dictionary<long, object>();
        public object this[long i]
        {
            get { return _dict[i]; }            
        }
        public void Add<T>(long key, T value) where T : class
        {
            _dict.Add(key, value);
        }
        public bool ContainsKey(long key)
        {
            return _dict.ContainsKey(key);
        }
        public T GetValue<T>(long key) where T : class, new()
        {
            return _dict[key] as T;
        }
        public void Remove(long key)
        {
            _dict.Remove(key);
        }

       
    }
}
