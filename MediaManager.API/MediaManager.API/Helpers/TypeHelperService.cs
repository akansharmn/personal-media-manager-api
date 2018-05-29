using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediaManager.API.Services;

namespace MediaManager.API.Helpers
{
    /// <summary>
    /// A helper class to determine if a property is present on a type
    /// </summary>
    public class TypeHelperService : ITypeHelperService
    {
        /// <summary>
        /// Method to determine of a property is present in a class
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="fields">fields separated by comma to be checked</param>
        /// <returns></returns>
        public bool TypeHasProperties<T>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
                return true;

            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var property = field.Trim();
                var propertyInfo = typeof(T).GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
