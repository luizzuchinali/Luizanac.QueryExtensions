using System;
using System.Collections.Generic;
using Luizanac.MongoDB.QueryExtensions.Extensions;
using Luizanac.QueryExtensions.Abstractions.Enums;
using Luizanac.QueryExtensions.Abstractions.Extensions;

namespace Luizanac.QueryExtensions.Abstractions.Sort
{
    /// <summary>
    /// Parse a sort string to a list of <see cref="ParsedSort"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class SortParser<T>
    {
        public IList<ParsedSort> ParsedSorts { get; } = new List<ParsedSort>();

        public SortParser(string sort, ECaseType caseType)
        {
            _ = sort ?? throw new ArgumentNullException(nameof(sort), "sort can't be null");
            var sorts = sort.Split('|');
            foreach (var sortPart in sorts)
            {
                var sorter = sortPart.Split(',');
                var properties = sorter[0].GetProperties('.', caseType);
                var lambda = typeof(T).GetLambdaExpression(properties);
                ParsedSorts.Add(new ParsedSort(sorter[1], lambda));
            }
        }
    }
}