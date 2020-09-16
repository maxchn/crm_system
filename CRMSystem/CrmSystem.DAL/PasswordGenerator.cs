using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CrmSystem.DAL
{
    public static class PasswordGenerator
    {
        private const int DefaultMinimum = 8;
        private const int DefaultMaximum = 12;
        private static readonly char[] AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#&*=+-@[]".ToCharArray();
        private static Random _random = new Random();
        private static StringBuilder _builder = new StringBuilder();

        public static string Generate()
        {
            bool isValid = false;

            while (!isValid)
            {
                _builder.Clear();
                for (int i = 0; i < _random.Next(DefaultMinimum, DefaultMaximum); i++)
                {
                    _builder.Append(AllowedChars[_random.Next(0, AllowedChars.Length)]);
                }

                isValid = IsValid(_builder.ToString());
            }

            return _builder.ToString();
        }

        private static bool IsValid(string value)
        {
            return Regex.IsMatch(value, "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\\$%\\^&\\*])(?=.{8,}).*$");
        }
    }
}