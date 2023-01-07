using System;
using System.Linq;
using System.Text.RegularExpressions;
using Luizanac.QueryExtensions.Filter;

namespace Luizanac.MongoDB.QueryExtensions.Filter
{
    public class ParsedFilter
    {
        public ParsedFilter(string filters)
        {
            if (string.IsNullOrWhiteSpace(filters)) throw new ArgumentNullException(nameof(filters));

            var filterSplits = filters.Split(Operators, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim()).ToArray();
            Names = Regex.Split(filterSplits[0], SplitPattern).Select(t => t.Trim()).ToArray();
            Values = filterSplits.Length > 1
                ? Regex.Split(filterSplits[1], SplitPattern).Select(t => t.Trim()).ToArray()
                : null;
            Operator = Array.Find(Operators, filters.Contains) ?? "==";
            ParsedOperator = GetOperatorParsed(Operator);
        }

        private const string SplitPattern = @"(?<!($|[^\\])(\\\\)*?\\)\|";

        private static readonly string[] Operators =
        {
            "!@=",
            "@=",
            "!_=",
            "_=",
            "!=",
            "==",
            ">=",
            "<=",
            ">",
            "<",
            "$="
        };

        public string[] Names { get; }

        public EFilterOperator ParsedOperator { get; }

        public string[] Values { get; }

        private string Operator { get; }

        private static EFilterOperator GetOperatorParsed(string @operator)
        {
            switch (@operator)
            {
                case "==":
                    return EFilterOperator.Equals;
                case "!=":
                    return EFilterOperator.NotEquals;
                case "<":
                    return EFilterOperator.LessThan;
                case ">":
                    return EFilterOperator.GreaterThan;
                case ">=":
                    return EFilterOperator.GreaterThanOrEqualTo;
                case "<=":
                    return EFilterOperator.LessThanOrEqualTo;
                case "@=":
                    return EFilterOperator.Contains;
                case "!@=":
                    return EFilterOperator.NotContains;
                case "_=":
                    return EFilterOperator.StartsWith;
                case "!_=":
                    return EFilterOperator.NotStartsWith;
                case "$=":
                    return EFilterOperator.Regex;
                default:
                    return EFilterOperator.Equals;
            }
        }
    }
}