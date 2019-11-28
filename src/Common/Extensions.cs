using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Common
{
    public static class Extensions
    {
        public static object DeserializeFromStream(this Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }

    }
}
