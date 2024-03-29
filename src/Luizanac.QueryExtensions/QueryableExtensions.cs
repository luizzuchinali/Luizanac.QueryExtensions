using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
using Luizanac.QueryExtensions.Enums;
using Luizanac.QueryExtensions.Filter;
using Luizanac.QueryExtensions.Extensions;

namespace Luizanac.QueryExtensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filter by asc or desc passing an string
        /// </summary>
        /// <param name="source">The source IQueryable</param>
        /// <param name="sort">Asc,prop / desc,prop</param>
        /// <param name="caseType">You application case type</param>
        /// <typeparam name="TSource">Type of data</typeparam>
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string sort,
            ECaseType caseType = ECaseType.PascalCase)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(sort) || string.IsNullOrWhiteSpace(sort))
                return (IOrderedQueryable<TSource>)source;

            var sorter = sort.Split(',');

            var properties = sorter[0].GetProperties('.', caseType);

            return sorter[1].Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                ? source.ExecuteOrderBy("OrderBy", properties)
                : source.ExecuteOrderBy("OrderByDescending", properties);
        }

        private static IOrderedQueryable<TSource> ExecuteOrderBy<TSource>(
            this IQueryable<TSource> query, string methodName, string[] properties)
        {
            var entityType = typeof(TSource);

            var method = methodName.GetMethodInfo(typeof(Queryable), 2);

            var selector = entityType.GetSelector(properties);

            var genericMethod = method
                .MakeGenericMethod(entityType, entityType.GetPropertyType(properties));

            return (IOrderedQueryable<TSource>)genericMethod
                .Invoke(genericMethod, new object[] { query, selector });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource">Entity type</typeparam>
        /// <param name="query">The source queryable</param>
        /// <param name="filters">Filters string with right format</param>
        /// <returns>The source queryable with filters aplied</returns>
        public static IQueryable<TSource> Filter<TSource>(this IQueryable<TSource> query, string filters)
        {
            if (string.IsNullOrEmpty(filters) || string.IsNullOrWhiteSpace(filters)) return query;

            foreach (var filter in filters.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrWhiteSpace(filter))
                    continue;

                var term = new ParsedFilter(filter);
                var parameter = Expression.Parameter(typeof(TSource), "x");

                var queryExpression = term.Names.GetQueryExpression(parameter, term);

                var lambdaExpression = Expression.Lambda<Func<TSource, bool>>(queryExpression, parameter);

                query = query.Where(lambdaExpression);
            }

            return query;
        }

        private static Expression GetQueryExpression(this string[] names, ParameterExpression parameter,
            ParsedFilter parsedFilter) =>
            names.Select(x =>
                {
                    var property = GetMemberProperty(x, parameter);
                    var propertyType = ((PropertyInfo)property.Member).PropertyType;
                    var converter = TypeDescriptor.GetConverter(propertyType);

                    var operatorExpressions =
                        parsedFilter.Values.GetOperatorExpressions(converter, propertyType, property, parsedFilter);

                    return operatorExpressions.Aggregate((a, c) => Expression.OrElse(a, c));
                })
                .Aggregate(Expression.OrElse);

        private static MemberExpression GetMemberProperty(string strProperty, ParameterExpression parameter)
        {
            MemberExpression property = null;
            if (strProperty.Contains("."))
            {
                var propNames = strProperty.Split(".");
                foreach (var propName in propNames)
                {
                    if (property == null)
                        property = Expression.PropertyOrField(parameter, propName);
                    else
                        property = Expression.PropertyOrField(property, propName);
                }
            }
            else
                property = Expression.PropertyOrField(parameter, strProperty);

            return property;
        }

        private static IEnumerable<Expression> GetOperatorExpressions(this string[] values, TypeConverter converter,
            Type propertyType, MemberExpression property, ParsedFilter parsedFilter) => values.Select(x =>
        {
            var filterValue = converter.ConvertFromInvariantString(x);
            var constantValue = Expression.Constant(filterValue);
            var valueExpression = Expression.Convert(constantValue, propertyType);
            return parsedFilter.ParsedOperator.GetOperatorExpression(property, valueExpression);
        });

        private static Expression GetOperatorExpression(this EFilterOperator @operator, MemberExpression property,
            UnaryExpression valueExpression)
        {
            switch (@operator)
            {
                case EFilterOperator.GreaterThan:
                    return Expression.GreaterThan(property, valueExpression);
                case EFilterOperator.GreaterThanOrEqualTo:
                    return Expression.GreaterThanOrEqual(property, valueExpression);
                case EFilterOperator.LessThan:
                    return Expression.LessThan(property, valueExpression);
                case EFilterOperator.LessThanOrEqualTo:
                    return Expression.LessThanOrEqual(property, valueExpression);
                case EFilterOperator.Equals:
                    return Expression.Equal(property, valueExpression);
                case EFilterOperator.NotEquals:
                    return Expression.NotEqual(property, valueExpression);
                case EFilterOperator.StartsWith:
                    return GetMethodExpression("StartsWith", typeof(string), property, valueExpression);
                case EFilterOperator.NotStartsWith:
                    return Expression.Not(GetMethodExpression("StartsWith", typeof(string), property, valueExpression));
                case EFilterOperator.Contains:
                    return GetMethodExpression("Contains", typeof(string), property, valueExpression);
                case EFilterOperator.NotContains:
                    return Expression.Not(GetMethodExpression("Contains", typeof(string), property, valueExpression));
                default:
                    return Expression.Equal(property, valueExpression);
            }
        }

        private static Expression GetMethodExpression(string methodName, Type methodType, MemberExpression property,
            UnaryExpression valueExpression)
        {
            var method = methodName.GetMethodInfo(methodType, 1, false);
            return Expression.Call(property, method, valueExpression);
        }

        /// <summary>
        /// Paginates an <see cref="IQueryable{T}"/>
        /// </summary>
        /// <param name="query">The IQueriable</param>
        /// <param name="page">Currrent page</param>
        /// <param name="size">Number of data to get</param>
        /// <typeparam name="T">The entity type</typeparam>
        /// <returns><see cref="IQueryable{T}"/> paginated</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int size)
        {
            page = page <= 0 ? 1 : page;
            query = query.Skip((page - 1) * size).Take(size);
            return query;
        }

        // public static HttpContext SetPaginationHeader<T>(this HttpContext httpContext, string route, Pagination<T> pagination){
        //     httpContext.Response.Headers.Add ("X-Total-Count", pagination.TotalPages.ToString());
        //     httpContext.Response.Headers.Add ("Link",
        //         $"<{route}&page={pagination.CurrentPage}>; rel=\"first\", <{route}&page={pagination.NextPage}>; rel=\"next\", <{route}&page={pagination.TotalDataCount}>; rel=\"last\""
        //     );
        //     return httpContext;
        // }
    }
}