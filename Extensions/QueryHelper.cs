using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Luizanac.Utils.Extensions.Interfaces;
using Luizanac.Utils.Extensions.Models;
using Microsoft.EntityFrameworkCore;

namespace Luizanac.Utils.Extensions
{
    public static class QueryHelper
    {
        /// <summary>
        /// Filter by asc or desc passing an string
        /// </summary>
        /// <param name="source">The source IQueryable</param>
        /// <param name="sort">Asc, Desc</param>
        /// <typeparam name="TSource">Type of data</typeparam>
        public static IQueryable<TSource> OrderByString<TSource>(this IQueryable<TSource> source, string sort)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(sort) || string.IsNullOrWhiteSpace(sort)) throw new ArgumentNullException(nameof(sort));

            var sorter = sort.Split(',');

            source = sorter[1].ToLower().Equals("asc")
                ? source.OrderBy(sorter[0])
                : source.OrderByDescending(sorter[0]);

            return source;
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource>(
            this IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            var method = GetOrderMethod("OrderBy");

            var selector = GetSelector(entityType, propertyName);
            var genericMethod = method
                .MakeGenericMethod(entityType, GetPropertyType(entityType, propertyName));

            return (IOrderedQueryable<TSource>) genericMethod
                .Invoke(genericMethod, new object[] {query, selector});
        }

        public static IOrderedQueryable<TSource> OrderByDescending<TSource>(
            this IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            var method = GetOrderMethod("OrderByDescending");

            var selector = GetSelector(entityType, propertyName);
            var genericMethod = method
                .MakeGenericMethod(entityType, GetPropertyType(entityType, propertyName));

            return (IOrderedQueryable<TSource>) genericMethod
                .Invoke(genericMethod, new object[] {query, selector});
        }

        private static MethodInfo GetOrderMethod(string methodName)
        {
            var enumarableType = typeof(Queryable);
            var method = enumarableType.GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                .Where(m =>
                {
                    var parameters = m.GetParameters().ToList();
                    return parameters.Count == 2;
                }).Single();

            return method;
        }

        private static Type GetPropertyType(Type entityType, string propertyName)
        {
            var navigationProperty = propertyName.Split('.');
            if (navigationProperty.Length > 1)
                return entityType.GetProperty(navigationProperty[0])?.PropertyType.GetProperty(navigationProperty[1])
                    ?.PropertyType;

            return entityType.GetProperty(navigationProperty[0])?.PropertyType;
        }

        private static LambdaExpression GetSelector(Type entityType, string propertyName)
        {
            var properties = propertyName.Split('.');
            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.PropertyOrField(arg, properties[0]);

            if (properties.Length > 1)
                property = Expression.PropertyOrField(property, properties[1]);

            return Expression.Lambda(property, new ParameterExpression[] {arg});
        }

        /// <summary>
        /// Paginates an IQueryable
        /// </summary>
        /// <param name="query">The IQueriable</param>
        /// <param name="page">Currrent page</param>
        /// <param name="size">Number of data to get</param>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <returns></returns>
        public static async Task<IPagination<IList<T>>> Paginate<T>(this IQueryable<T> query, int page, int size)
        {
            var entries = query.Skip((page - 1) * size).Take(size);
            var count = await query.CountAsync();
            var totalPages = (int) Math.Ceiling(count / (float) size);

            var firstPage = 1;
            var lastPage = totalPages;
            var prevPage = page > firstPage ? page - 1 : firstPage;
            var nextPage = page < lastPage ? page + 1 : lastPage;
            return new Pagination<IList<T>>(await entries.ToListAsync(), totalPages, page, size, prevPage, nextPage,
                count);
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