using System;
using System.Reflection;

namespace Utils.Extensions
{
    public static class PropertyHelper
    {
        /// <summary>
        /// Get value of a property by string
        /// </summary>
        /// <param name="obj">Object that hold the property</param>
        /// <param name="name">Name of the property</param>
        /// <returns>Object</returns>
        public static Object GetPropValue(this Object obj, string name)
        {
            if (name == null)
                throw new ArgumentNullException("The name cold not be null");

            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }
    }
}
