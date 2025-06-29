using Newtonsoft.Json;

namespace BackendSystem.Common
{
    public static class JsonSafe
    {
        public static bool TryDeserialize<T>(string json, out T? result,out string? errorMessage)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                result = default;
                errorMessage = "Input JSON string is null or empty.";
                return false;
            }

            try
            {
                errorMessage = null;
                result = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                result = default;
                return false;
            }

        }
        public static bool TryDeserialize<T>(string json, out T? result)
        {
            return TryDeserialize<T>(json, out result, out _);
        }

    }
}

