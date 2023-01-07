using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using Luizanac.QueryExtensions.Filter;
using Luizanac.QueryExtensions.Abstractions.Enums;
using Luizanac.QueryExtensions.Abstractions.Extensions;

namespace Luizanac.MongoDB.QueryExtensions.Filter
{
    //TODO: move to abstraction package and create a mongodb especialization
    public class FilterParser<T> where T : class
    {
        protected readonly IEnumerable<ParsedFilter> ParsedFilters;
        protected readonly ECaseType CaseType;
        protected readonly FilterDefinitionBuilder<T> FilterBuilder;

        public FilterParser(
            string filters,
            FilterDefinitionBuilder<T> filterBuilder,
            ECaseType caseType = ECaseType.PascalCase)
        {
            if (string.IsNullOrEmpty(filters) || string.IsNullOrWhiteSpace(filters))
                throw new ArgumentException("invalid argument", nameof(filters));

            ParsedFilters = filters.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(filter => !string.IsNullOrWhiteSpace(filter))
                .Select(filter => new ParsedFilter(filter));

            CaseType = caseType;
            FilterBuilder = filterBuilder;
        }

        public IEnumerable<FilterDefinition<T>> GetFilterDefinitions() => ParsedFilters.Select(HandleTerm);

        private FilterDefinition<T> HandleTerm(ParsedFilter parsedFilter)
        {
            var filterDefinitions = new List<FilterDefinition<T>>();
            foreach (var name in parsedFilter.Names)
            {
                var propertyName = name.ConvertCase(CaseType);
                if (!HasProperty(propertyName))
                    continue;

                var valueDefinitions =
                    parsedFilter.Values.Select(x => GetFilterDefinitionFromOperator(parsedFilter.ParsedOperator, propertyName, x));
                filterDefinitions.Add(FilterBuilder.Or(valueDefinitions));
            }

            return FilterBuilder.Or(filterDefinitions);
        }

        protected bool HasProperty(string property)
        {
            var type = typeof(T);
            if (!property.Contains('.')) return type.GetProperty(property) != null;

            var props = property.Split('.');
            foreach (var prop in props)
            {
                var propertyInfo = type.GetProperty(prop);
                if (propertyInfo == null)
                    return false;

                type = propertyInfo.PropertyType;
            }

            return true;
        }

        protected virtual FilterDefinition<T> GetFilterDefinitionFromOperator(
            EFilterOperator @operator,
            string property,
            string value)
        {
            switch (@operator)
            {
                case EFilterOperator.GreaterThan:
                    return FilterBuilder.Gt(property, value);
                case EFilterOperator.GreaterThanOrEqualTo:
                    return FilterBuilder.Gte(property, value);
                case EFilterOperator.LessThan:
                    return FilterBuilder.Lt(property, value);
                case EFilterOperator.LessThanOrEqualTo:
                    return FilterBuilder.Lte(property, value);
                case EFilterOperator.Equals:
                    return FilterBuilder.Eq(property, value);
                case EFilterOperator.NotEquals:
                    return FilterBuilder.Ne(property, value);
                case EFilterOperator.StartsWith:
                    return FilterBuilder.Regex(property, new BsonRegularExpression($"^{value}", "i"));
                case EFilterOperator.NotStartsWith:
                    return FilterBuilder.Regex(property, new BsonRegularExpression($"^(?!{value})", "i"));
                case EFilterOperator.Contains:
                    return FilterBuilder.Regex(property, new BsonRegularExpression($"{value}", "i"));
                case EFilterOperator.NotContains:
                    return FilterBuilder.Regex(property, new BsonRegularExpression($"^((?!{value}).)*$", "i"));
                case EFilterOperator.Regex:
                    return FilterBuilder.Regex(property, new BsonRegularExpression(value, "i"));
                default:
                    return FilterDefinition<T>.Empty;
            }
        }
    }
}