using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    /// <summary>
    /// Extensions on object class
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// shapes an object according to fields asked by user
        /// </summary>
        /// <typeparam name="TSource">type of object in the IEnumerable list</typeparam>
        /// <param name="source">the IEnumerable list</param>
        /// <param name="fields">the fileds asked by user</param>
        /// <returns></returns>
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var shapedObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                foreach (var property in propertyInfos)
                {
                    var propertyValue = property.GetValue(source);
                    ((IDictionary<string, object>)shapedObject).Add(property.Name, propertyValue);
                }
            }
            else
            {
                var filedList = fields.Split(',');
                foreach (var field in filedList)
                {
                    var property = typeof(TSource).GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                    ((IDictionary<string, object>)shapedObject).Add(property.Name, property.GetValue(source));
                }
            }

            return shapedObject;
        }
    }
}
