using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NJsonSchema;

namespace MediaManager.API
{
    public class YoutubeParser : IParser
    {
        public VideoForCreationDTO video { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DomainName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public   bool IsMatch(VideoForCreationDTO video)
        {
            if (video.Domain == "youtube")
                return true;
            else
                return false;
        }

        public bool IsValid(string properties)
        {
            var obj = JsonConvert.DeserializeObject<YoutubeProperties>(properties);
            if (obj == null)
                return false;
            return true;
        }

        public IParser Create()
        {
            return new YoutubeParser();
        }
        public VideoForCreationDTO Update(string propertyname, string value, VideoForCreationDTO video)
        {
            var obj = JObject.Parse(video.Properties);
            if (obj.Property(propertyname) != null)
            {
                obj.Property(propertyname).Value = value;
            }

            return video;
        }

        public bool Update(string propertyname, string value)
        {
            throw new NotImplementedException();
        }

        public bool IsMatch(string domain)
        {
            throw new NotImplementedException();
        }
    }
}
