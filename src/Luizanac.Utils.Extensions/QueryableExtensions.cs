using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luizanac.Utils.Extensions.Abstractions.Enums;
using Luizanac.Utils.Extensions.Abstractions.Interfaces;
using Luizanac.Utils.Extensions.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace Luizanac.Utils.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Filter by asc or desc passing an string
        /// </summary>
        /// <param name="source">The source IQueryable</param>
        /// <param name="sort">Asc, Desc</param>
        /// <typeparam name="TSource">Type of data</typeparam>
        public static IOrderedQueryable<TSource> OrderByString<TSource>(this IQueryable<TSource> source, string sort, ECaseType caseType = ECaseType.PascalCase)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(sort) || string.IsNullOrWhiteSpace(sort)) throw new ArgumentNullException(nameof(sort));

            var sorter = sort.Split(',');

            var properties = sorter[1].GetProperties('.', caseType);

            return sorter[0].Equals("asc", StringComparison.InvariantCultureIgnoreCase)
                ? source.ExecuteOrderBy("OrderBy", properties)
                : source.ExecuteOrderBy("OrderByDescending", properties);
        }

        private static IOrderedQueryable<TSource> ExecuteOrderBy<TSource>(
            this IQueryable<TSource> query, string methodName, string[] properties)
        {
            var entityType = typeof(TSource);

            var method = methodName.GetMethodInfo(typeof(IQueryable), 2);

            var selector = entityType.GetSelector(properties);

            var genericMethod = method
                .MakeGenericMethod(entityType, entityType.GetPropertyType(properties));

            return (IOrderedQueryable<TSource>)genericMethod
                .Invoke(genericMethod, new object[] { query, selector });
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
            page = page <= 0 ? 1 : page;

            var entries = query.Skip((page - 1) * size).Take(size);
            var count = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (float)size);

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