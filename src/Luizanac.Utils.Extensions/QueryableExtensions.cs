using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Luizanac.Utils.Extensions.Abstractions.Enums;
using Luizanac.Utils.Extensions.Abstractions.Interfaces;
using Luizanac.Utils.Extensions.Abstractions.Models;
using Luizanac.Utils.Extensions.Models;
using Microsoft.EntityFrameworkCore;

namespace Luizanac.Utils.Extensions
{
	public static class QueryableExtensions
	{
		/// <summary>
		/// Filter by asc or desc passing an string
		/// </summary>
		/// <param name="source">The source IQueryable</param>
		/// <param name="sort">Asc,prop / desc,prop</param>
		/// <typeparam name="TSource">Type of data</typeparam>
		public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string sort, ECaseType caseType = ECaseType.PascalCase)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (string.IsNullOrEmpty(sort) || string.IsNullOrWhiteSpace(sort)) throw new ArgumentNullException(nameof(sort));

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
			foreach (var filter in filters.Split(',', StringSplitOptions.RemoveEmptyEntries))
			{
				if (string.IsNullOrWhiteSpace(filter))
					continue;

				var term = new Term(filter);
				var parameter = Expression.Parameter(typeof(TSource), "x");
				var property = Expression.PropertyOrField(parameter, term.Names.First());
				var propertyType = ((PropertyInfo)property.Member).PropertyType;
				var converter = TypeDescriptor.GetConverter(propertyType);

				var filterValue = converter.ConvertFromInvariantString(term.Values.First());
				var constantValue = Expression.Constant(filterValue);
				var valueExpression = Expression.Convert(constantValue, propertyType);
				var operatorExpression = GetOperatorExpression(term.OperatorParsed, property, valueExpression);
				var lambdaExpression = Expression.Lambda<Func<TSource, bool>>(operatorExpression, parameter);

				query = query.Where(lambdaExpression);
			}

			return query;
		}


		private static BinaryExpression GetOperatorExpression(this EFilterOperator @operator, Expression property, Expression constantExpression) => @operator switch
		{
			EFilterOperator.GreaterThan => Expression.GreaterThan(property, constantExpression),
			EFilterOperator.GreaterThanOrEqualTo => Expression.GreaterThanOrEqual(property, constantExpression),
			EFilterOperator.LessThan => Expression.LessThan(property, constantExpression),
			EFilterOperator.LessThanOrEqualTo => Expression.LessThanOrEqual(property, constantExpression),
			EFilterOperator.Equals => Expression.Equal(property, constantExpression),
			EFilterOperator.NotEquals => Expression.NotEqual(property, constantExpression),
			_ => null
		};


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