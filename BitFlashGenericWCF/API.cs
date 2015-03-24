using System.Collections.Generic;

namespace BitFlashGenericWCF
{
    public static class API
    {
        private static readonly List<string> APIKeys = new List<string>(10000); 
        public static bool Validate(string apiKey)
        {
            return APIKeys.Contains(apiKey);
        }

        public static void Initialize()
        {
            APIKeys.Add("6c793695171e793d7d0080ad7700a2bc50256912cef2492c201e8ecc54b24ab5");
        }
    }
}
