using System;
using Newtonsoft.Json;

namespace Game.Sim
{
    /// <summary>
    /// Shared Newtonsoft settings for Def files. Unknown fields are errors, so a typo in JSON
    /// fails at load instead of silently falling back to a default value.
    /// </summary>
    public static class DefJson
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error,
        };

        public static T Deserialize<T>(string json, string source)
        {
            T result;
            try
            {
                result = JsonConvert.DeserializeObject<T>(json, Settings);
            }
            catch (JsonException e)
            {
                throw new InvalidOperationException($"Failed to parse {source}: {e.Message}", e);
            }

            if (result == null)
                throw new InvalidOperationException($"{source} is empty.");
            return result;
        }
    }
}
