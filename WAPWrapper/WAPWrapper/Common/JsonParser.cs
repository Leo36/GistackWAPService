namespace WAPWrapper
{
    using System.Runtime.Serialization.Json;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public static class Parser
    {
         public static List<T> FromJSONString<T>(string json)
             where T : class
         {
            List<T> result = new List<T>();
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            Newtonsoft.Json.Linq.JToken value;

            if (jObject.TryGetValue("value", out value))
            {
                foreach (var a in jObject["value"])
                {
                    var deser = JsonConvert.DeserializeObject<T>(a.ToString());
                    result.Add(deser);

                } 
            }
            else
            {
                var deser = JsonConvert.DeserializeObject<T>(jObject.ToString(), settings);
                result.Add(deser);
            } 
            return result;
        }

        public static T FromJSON<T>(string json)
            where T : class
        {
            T output;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                output = serializer.ReadObject(mem) as T;

            return output;

        }

        public static string ToJSON<T>(T classInstance)
            where T : class
        {
            string jsonOutput = string.Empty;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                serializer.WriteObject(mem, classInstance);
                mem.Position = 0;

                using (System.IO.StreamReader reader = new System.IO.StreamReader(mem))
                    jsonOutput = reader.ReadToEnd();

            }

            return jsonOutput;
        }
    }
}

