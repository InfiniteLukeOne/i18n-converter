using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace I18nConverter
{
    // ReSharper disable once InconsistentNaming
    public class I18nJsonConverter : System.Text.Json.Serialization.JsonConverter<Dictionary<string, string>>
    {
        public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionary = new Dictionary<string, string>();

            var path = new Stack<string>();
            
            Read(ref reader, dictionary, path);

            return dictionary;
        }

        private static void Read(ref Utf8JsonReader reader, Dictionary<string, string> dictionary, Stack<string> path)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return;
                }

                // Get the key.
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                string propertyName = reader.GetString()!;

                path.Push(propertyName);

                reader.Read();
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    Read(ref reader, dictionary, path);
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    var value = reader.GetString()!;
                    dictionary.Add(string.Join('.', path.Reverse()), value);
                }
                else
                {
                    throw new JsonException();
                }

                path.Pop();
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            WriteObject(writer, value);
        }

        private static void WriteObject(Utf8JsonWriter writer, IEnumerable<KeyValuePair<string,string>> value)
        {
            writer.WriteStartObject();
            
            var groups = value
                .Select(i => (Key: i.Key.Split('.', 2), i.Value))
#pragma warning disable 8604
                .GroupBy(i => i.Key[0], item => new KeyValuePair<string,string>(item.Key.Skip(1).FirstOrDefault(), item.Value))
#pragma warning restore 8604
                .OrderBy(g => g.Key);
            foreach (var group in groups)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (group.Count() == 1 && group.Single().Key == null)
                {
                    writer.WriteString(group.Key, group.Single().Value);
                }
                else
                {
                    writer.WritePropertyName(group.Key);
                    WriteObject(writer, group);
                }
            }
            
            writer.WriteEndObject();
        }
    }
}