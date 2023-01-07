using MongoDB.Driver;
using System.Collections.Generic;
using Luizanac.MongoDB.QueryExtensions.Extensions;
using Luizanac.MongoDB.QueryExtensions.Filter;
using Luizanac.QueryExtensions.Abstractions.Enums;
using Luizanac.QueryExtensions.Abstractions.Sort;
using static System.Console;

namespace Luizanac.MongoDB.QueryExtensions
{
    public class Query<T> where T : class
    {
        public IMongoCollection<T> Collection { get; }
        public FindOptions<T> Options { get; }

        private IEnumerable<FilterDefinition<T>> _filterDefinitions;
        protected ECaseType CaseType { get; }

        private FilterDefinitionBuilder<T> _filterBuilder;

        protected FilterDefinitionBuilder<T> FilterBuilder
        {
            get
            {
                _filterBuilder = _filterBuilder ?? new FilterDefinitionBuilder<T>();
                return _filterBuilder;
            }
        }

        private SortDefinitionBuilder<T> _sortBuilder;

        protected SortDefinitionBuilder<T> SortBuilder
        {
            get
            {
                _sortBuilder = _sortBuilder ?? new SortDefinitionBuilder<T>();
                return _sortBuilder;
            }
        }

        public Query(IMongoCollection<T> collection, ECaseType caseType = ECaseType.PascalCase)
        {
            Collection = collection;
            CaseType = caseType;
            Options = new FindOptions<T>();
        }

        public Query(IMongoCollection<T> collection, FindOptions<T> options, ECaseType caseType = ECaseType.PascalCase)
            : this(collection, caseType)
        {
            Options = options;
        }

        public FilterDefinition<T> GetFilterDefinition() =>
            _filterDefinitions != null
                ? new FilterDefinitionBuilder<T>().And(_filterDefinitions)
                : FilterDefinition<T>.Empty;

        /// <summary>
        /// Add sort capability to <see cref="Query{T}"/> instance
        /// </summary>
        /// <param name="sort">the sort string property,asc/desc</param>
        /// <returns>The current instance of <see cref="Query{T}"/></returns>
        public Query<T> OrderBy(string sort)
        {
            if (string.IsNullOrEmpty(sort) || string.IsNullOrWhiteSpace(sort)) return this;

            var sortDefinitions = new SortParser<T>(sort, CaseType).GetSortDefinitions();
            Options.Sort = SortBuilder.Combine(sortDefinitions);
            return this;
        }

        public Query<T> Filter(string filters)
        {
            if (string.IsNullOrEmpty(filters) || string.IsNullOrWhiteSpace(filters)) return this;

            var parser = new FilterParser<T>(filters, FilterBuilder, CaseType);
            _filterDefinitions = parser.GetFilterDefinitions();

            WriteLine(filters);
            return this;
        }
    }
}