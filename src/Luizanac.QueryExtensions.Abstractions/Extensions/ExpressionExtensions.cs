using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Luizanac.MongoDB.QueryExtensions.Extensions
{
    //TODO:1- move to abstractions lib
    //TODO:2- change string to value type and pass by ref

    public static class ExpressionExtensions
    {
        /// <summary>
        ///	Create a lambda expression by array of properties
        /// </summary>
        /// <param name="entityType"><see cref="Type"/></param>
        /// <param name="properties"><see cref="IEnumerable{T}"/> of properties to access in lambda</param>
        /// <returns></returns>
        public static LambdaExpression GetLambdaExpression(this Type entityType, IEnumerable<string> properties)
        {
            var arg = Expression.Parameter(entityType, "x");
            var property = properties.Aggregate<string, MemberExpression>(null,
                (current, strProperty) =>
                    current == null
                        ? Expression.PropertyOrField(arg, strProperty)
                        : Expression.PropertyOrField(current, strProperty));

            return Expression.Lambda(property, arg);
        }
    }
}