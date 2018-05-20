using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> videoPropertyMapping = new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
            {"Title", new PropertyMappingValue(new List<string>(){ "Title"}) },
            {"Url", new PropertyMappingValue(new List<string>(){ "Url"}) },
            {"Domain", new PropertyMappingValue(new List<string>(){ "Domain"}) }

        };

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMappingDictionary = GetPropertyMapping<TSource, TDestination>();

            var orderByFields = fields.Split(',');

            foreach(var orderByField in orderByFields)
            {
                var trimmedField = orderByField.Trim();

                var index = trimmedField.IndexOf(" ");
                var property =index == -1?trimmedField: trimmedField.Remove(index);
                if(!propertyMappingDictionary.ContainsKey(property))
                {
                    return false;
                }

            }
            return true;
        }

        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<VideoForCreationDTO, Video>(videoPropertyMapping));
        }

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            if(matchingMapping.Count() == 1)
            {
                return matchingMapping.First().mappingDictionary;
            }

            throw new Exception("no matching property dictionary found");
        }
    }
}
