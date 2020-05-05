using System;
using System.Collections.Generic;
using System.Linq;

namespace RenameTool
{
    public static class Extensions
    {
        public static IEnumerable<string> TrimSplit(this string text, params char[] separator)
        {
            return text.Split(separator).Select(s => s.Trim());
        }

        /// <summary>
        /// 首字母小写写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToLower(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return input;
            string str = input.First().ToString().ToLower() + input.Substring(1);
            return str;
        }
    }
}
