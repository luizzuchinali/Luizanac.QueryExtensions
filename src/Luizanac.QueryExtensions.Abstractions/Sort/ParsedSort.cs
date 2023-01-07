using System;
using System.Linq.Expressions;

namespace Luizanac.QueryExtensions.Abstractions.Sort
{
    public class ParsedSort
    {
        private const string Asc = "asc";
        private const string Desc = "desc";
        public ESortType SortType { get; }
        public LambdaExpression LambdaExpression { get; }

        public ParsedSort(string sortType, LambdaExpression lambdaExpression)
        {
            if (!(sortType.Contains(Asc) || sortType.Contains(Desc)))
                throw new ArgumentException("sortType can't be different of asc or desc", nameof(sortType));

            SortType = sortType == Asc ? ESortType.Asc : ESortType.Desc;
            LambdaExpression = lambdaExpression;
        }
    }
}