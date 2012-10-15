﻿namespace Client.Common
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonLowercaseSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new LowercaseContractResolver()
        };

        public static string SerializeObject(object o, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(o, Formatting.None, Settings);
        }

        public static T DeserializeObject<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        public class LowercaseContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return Utils.LowerFirstLetter(propertyName);
            }
        }
    }
}
