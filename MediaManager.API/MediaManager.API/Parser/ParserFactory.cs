using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API
{
    public  class ParserFactory
    {
        List<IParser> parsers { get; set; }

        public ParserFactory(List<IParser> parsers)
        {
            this.parsers = parsers;
        }

        public IParser CreateParser(string domain)
        {
           var parser = parsers.FirstOrDefault(x => x.IsMatch(domain));
            return parser.Create();
        }        
    }
}
