using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace service.tests.Extensions
{
    public static class ObjectExtensions
    {
        public static object Convert(this object obj, Type type)
        {
            try
            {
                if (obj is string && !type.GetNestedTypes().Any(t => t == typeof(ValueType)))
                    return new JsonSerializer().Deserialize(new JsonTextReader(new StringReader((string)obj)), type);
            }
            catch(JsonReaderException)
            {
                var serializedResult = new StringBuilder();
                new JsonSerializer().Serialize(new StringWriter(serializedResult), obj);
                return new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(serializedResult.ToString())), type);
            }

            return obj;
        }
    }
}
