using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Luizanac.QueryExtensions
{
    public static class ReflectionExtensions
    {
        public static Type GetPropertyType(this Type entityType, string[] properties)
        {
            if (properties.Length > 1)
                return entityType.GetProperty(properties[0])?.PropertyType.GetProperty(properties[1])
                    ?.PropertyType;

            return entityType.GetProperty(properties[0])?.PropertyType;
        }

        public static LambdaExpression GetSelector(this Type entityType, string[] properties)
        {
            var arg = Expression.Parameter(entityType, "x");
            MemberExpression property = null;
            foreach (var strProperty in properties)
            {
                if (property == null)
                    property = Expression.PropertyOrField(arg, strProperty);
                else
                    property = Expression.PropertyOrField(property, strProperty);
            }

            return Expression.Lambda(property, new ParameterExpression[] { arg });
        }

        public static MethodInfo GetMethodInfo(this string methodName, Type type, int parametersCount,
            bool genericMethodDefinition = true)
        {
            var method = type.GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition == genericMethodDefinition)
                .Where(m =>
                {
                    var parameters = m.GetParameters().ToList();
                    return parameters.Count == parametersCount;
                }).First();

            return method;
        }
    }
}