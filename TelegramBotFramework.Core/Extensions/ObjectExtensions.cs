using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotFramework.Core.Extensions
{
   public static class ObjectExtensions
    {
        public static string Normalize(this decimal? value)
        {
            if (!value.HasValue)
            {
                return null;
            }
            return (value.Value / 1.000000000000000000000000000000000m).ToString(CultureInfo.InvariantCulture);
        }
        public static Dictionary<string, object> AsDictionary(this object source,
          BindingFlags bindingAttr = BindingFlags.FlattenHierarchy |
          BindingFlags.Instance |
          BindingFlags.NonPublic |
          BindingFlags.Public |
          BindingFlags.Static)
        {
            try
            {
                var result = new Dictionary<string, object>();
                var props = source.GetType().GetProperties(bindingAttr);
                foreach (var p in props)
                {
                    if (p.IsDefined(typeof(JsonIgnoreAttribute)))
                        continue;
                    string key = p.Name;
                    if (p.IsDefined(typeof(JsonPropertyAttribute)))
                    {
                        key = p.GetCustomAttribute<JsonPropertyAttribute>().PropertyName ?? p.Name;
                    }
                    object value = p.GetValue(source, null);

                    if (value == null)
                    {
                        continue;
                    }

                    if (value is bool)
                    {
                        value = value.ToString().ToLowerInvariant();
                    }
                    if (value is decimal || value is decimal?)
                    {
                        value = (value as decimal?).Normalize();

                    }
                    if (p.IsDefined(typeof(Newtonsoft.Json.JsonConverterAttribute)))
                    {
                        var att = p.GetCustomAttribute<Newtonsoft.Json.JsonConverterAttribute>();
                        var t = Activator.CreateInstance(att.ConverterType);
                        value = JsonConvert.SerializeObject(value, t as JsonConverter);
                    }
                    if (!result.ContainsKey(key) && !String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value.ToString()))
                    {
                        result.Add(key, value);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
