using System;
using System.Linq;
using System.Text;
using Luizanac.QueryExtensions.Enums;

namespace Luizanac.QueryExtensions.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string str)
        {
            var builder = new StringBuilder(str.Length);
            char? previous = null;
            foreach (var c in str)
            {
                builder.Append(previous == null || previous == '.' ? char.ToUpper(c) : c);
                previous = c;
            }

            return builder.ToString();
        }

        public static string ToCamelCase(this string str)
        {
            var builder = new StringBuilder(str.Length);
            char? previous = null;
            foreach (var c in str)
            {
                builder.Append(previous == null || previous == '.' ? char.ToLower(c) : c);
                previous = c;
            }

            return builder.ToString();
        }

        public static string ConvertCase(this string str, ECaseType caseType)
        {
            switch (caseType)
            {
                case ECaseType.CamelCase:
                    return str.ToCamelCase();
                case ECaseType.PascalCase:
                    return str.ToPascalCase();
                default:
                    return str;
            }
        }

        public static string[] GetProperties(this string str, char splitSeparator = '.',
            ECaseType caseType = ECaseType.PascalCase)
        {
            var properties = str.Split(new[] { splitSeparator }, StringSplitOptions.RemoveEmptyEntries);
            return properties.Select(x => x.ConvertCase(caseType)).ToArray();
        }
    }
}