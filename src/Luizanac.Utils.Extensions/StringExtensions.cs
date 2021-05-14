using System;
using System.Linq;
using Luizanac.Utils.Extensions.Abstractions.Enums;

namespace Luizanac.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string str) => char.ToUpperInvariant(str[0]) + str.Substring(1);
        public static string ToCamelCase(this string str) => char.ToLowerInvariant(str[0]) + str.Substring(1);
        public static string ConvertCase(this string str, ECaseType caseType) => caseType switch
        {
            ECaseType.CamelCase => str.ToCamelCase(),
            ECaseType.PascalCase => str.ToPascalCase(),
            _ => str
        };
    }
}