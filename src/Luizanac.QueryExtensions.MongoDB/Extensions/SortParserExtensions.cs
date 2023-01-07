using MongoDB.Driver;
using System.Collections.Generic;
using Luizanac.QueryExtensions.Abstractions.Sort;

namespace Luizanac.MongoDB.QueryExtensions.Extensions
{
    public static class SortParserExtensions
    {
        /// <summary>
        /// Create an <see cref="IEnumerable{SortDefinition}"/> of <see cref="SortDefinition{T}"/> from a <see cref="SortParser{T}"/>
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SortDefinition}"/> of <see cref="SortDefinition{T}"/></returns>
        public static IEnumerable<SortDefinition<T>> GetSortDefinitions<T>(this SortParser<T> sortParser)
        {
            var builder = new SortDefinitionBuilder<T>();
            var sortDefinitions = new List<SortDefinition<T>>(sortParser.ParsedSorts.Count);

            foreach (var parsedSort in sortParser.ParsedSorts)
            {
                var expressionField = new ExpressionFieldDefinition<T>(parsedSort.LambdaExpression);
                sortDefinitions.Add(parsedSort.SortType == ESortType.Asc
                    ? builder.Ascending(expressionField)
                    : builder.Descending(expressionField));
            }

            return sortDefinitions;
        }
    }
}