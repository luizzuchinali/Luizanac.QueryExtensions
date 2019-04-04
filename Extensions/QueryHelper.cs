using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utils.Extensions.Models;

namespace Utils.Extensions
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
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var sorter = sort.Split(',');

            source = sorter[1].ToLower().Equals("asc")
                ? source.OrderBy(x => x.GetPropValue(sorter[0]))
                : source.OrderByDescending(x => x.GetPropValue(sorter[0]));

            return source;
        }

        /// <summary>
        /// Paginates an IQueryable
        /// </summary>
        /// <param name="query">The IQueriable</param>
        /// <param name="page">Currrent page</param>
        /// <param name="size">Number of data to get</param>
        /// <typeparam name="T">Type of the data</typeparam>
        /// <returns></returns>
        public static async Task<Pagination<IList<T>>> Paginate<T>(this IQueryable<T> query, int page, int size)
        {
            var entries = query.Skip((page - 1) * size).Take(size);
            var count = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (float)size);

            var firstPage = 1;
            var lastPage = totalPages;
            var prevPage = page > firstPage ? page - 1 : firstPage;
            var nextPage = page < lastPage ? page + 1 : lastPage;
            return new Pagination<IList<T>>(await entries.ToListAsync(), totalPages, page, size, prevPage, nextPage, count);
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