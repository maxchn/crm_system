using System;
using System.Text;

namespace CrmSystem.Server.Utils
{
    public static class LinkGenerator
    {
        private static readonly char[] AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._~@".ToCharArray();
        private static Random _random = new Random();
        private static StringBuilder _builder = new StringBuilder();

        public static string GenerateShortLink(string domain, short length = 10)
        {
            _builder.Clear();
            _builder.Append(domain);

            for (int i = 0; i < length; i++)
            {
                _builder.Append(AllowedChars[_random.Next(0, AllowedChars.Length)]);
            }

            return _builder.ToString();
        }

        public static string GenerateLongLink(short iterationCount = 4)
        {
            _builder.Clear();
            
            for (int i = 0; i < iterationCount; i++)
            {
                _builder.Append(Guid.NewGuid());
            }

            return _builder.ToString().Replace("-", "");
        }
    }
}