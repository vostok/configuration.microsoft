using System;
using System.Collections.Generic;
using System.Text;

namespace Vostok.Configuration.Microsoft.Tests.Helpers
{
    internal static class RandomStringGenerator
    {
        [ThreadStatic]
        private static Random random;

        public static string CreateString(int length)
        {
            var rnd = GetRandom();
            var sb = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                sb.Append('a' + rnd.Next(26));
            }

            return sb.ToString();
        }

        public static List<string> CreateStrings(int amount, int length)
        {
            var result = new List<string>();

            for (var i = 0; i < amount; i++)
            {
                result.Add(CreateString(length));
            }

            return result;
        }

        private static Random GetRandom() =>
            random ?? (random = new Random());
    }
}