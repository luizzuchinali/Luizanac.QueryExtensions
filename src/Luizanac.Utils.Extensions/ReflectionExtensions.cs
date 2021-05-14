using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Luizanac.Utils.Extensions.Abstractions.Enums;

namespace Luizanac.Utils.Extensions
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
            var property = Expression.PropertyOrField(arg, properties[0]);

            if (properties.Length > 1)
                property = Expression.PropertyOrField(property, properties[1]);

            return Expression.Lambda(property, new ParameterExpression[] { arg });
        }

        public static MethodInfo GetMethodInfo(this string methodName, Type type, int parametersCount)
        {
            var method = type.GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                .Where(m =>
                {
                    var parameters = m.GetParameters().ToList();
                    return parameters.Count == parametersCount;
                }).Single();

            return method;
        }

        public static string[] GetProperties(this string str, char splitSeparator = '.', ECaseType caseType = ECaseType.PascalCase)
        {
            var properties = str.Split(splitSeparator, StringSplitOptions.RemoveEmptyEntries);
            return properties.Select(x => x.ConvertCase(caseType)).ToArray();
        }
    }
}