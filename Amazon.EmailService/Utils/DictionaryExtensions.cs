using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;

namespace Amazon.EmailService.Utils
{
    public static class DictionaryExtensions
    {
        public static void Add<TKey>(this Dictionary<TKey, AttributeValue> dictionary, Type propertyType, TKey key, string value)
        {
            if (propertyType == typeof(string))
            {
                dictionary.Add(key, new AttributeValue { S = value });
            }
            else if (propertyType == typeof(DateTime))
            {
                var parsedDateTime = DateTime.Parse(value);
                dictionary.Add(key, new AttributeValue { S = parsedDateTime.ToString("o") });
            }
            else if (propertyType == typeof(bool))
            {
                dictionary.Add(key, new AttributeValue { BOOL = bool.Parse(value) });
            }
        }
    }

}
