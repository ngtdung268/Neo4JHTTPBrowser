using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo4JHTTPBrowser.Helpers
{
    internal class SingleOrDictListPropertyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonToken = JToken.Load(reader);
            if (jsonToken.Type == JTokenType.Array)
            {
                var result = new List<object>();
                var children = jsonToken.Children().ToList();

                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    if (child.Type == JTokenType.Object)
                    {
                        result.Add(child.ToObject<Dictionary<string, object>>());
                    }
                    else
                    {
                        result.Add(child.ToObject<object>());
                    }
                }

                return result;
            }
            else
            {
                throw new JsonSerializationException("Invalid JSON format");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is List<object> list)
            {
                writer.WriteStartArray();

                foreach (var item in list)
                {
                    if (item is IDictionary<string, object> dict)
                    {
                        serializer.Serialize(writer, dict);
                    }
                    else
                    {
                        serializer.Serialize(writer, item);
                    }
                }

                writer.WriteEndArray();
            }
            else
            {
                throw new JsonSerializationException("Invalid object type for serialization");
            }
        }
    }
}
