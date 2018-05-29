using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    /// <summary>
    /// Extensions on IEnumerable types
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Helps to shape a list of data by providing only the fields asked by user
        /// </summary>
        /// <typeparam name="TSource">the type of object contained in IEnumerable</typeparam>
        /// <param name="source">IEnumerable list</param>
        /// <param name="fields">fields asked by user</param>
        /// <returns></returns>
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var expandoObjectList = new List<ExpandoObject>();

            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyInfoList.AddRange(propertyInfos);

            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    var property = field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"property {property} was not found on {typeof(TSource)}");

                    }
                    propertyInfoList.Add(propertyInfo);
                }


                foreach (TSource sourceObject in source)
                {
                    var dataShapedObj = new ExpandoObject();

                    foreach (var propertyInfo in propertyInfoList)
                    {
                        var propertyValue = propertyInfo.GetValue(sourceObject);

                        ((IDictionary<string, object>)dataShapedObj).Add(propertyInfo.Name, propertyValue);
                    }
                    expandoObjectList.Add(dataShapedObj);
                }
            }

            return expandoObjectList;
        }
    }
}
