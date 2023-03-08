using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Cache
{
    public class CacheUnit
    {
        public static ICacheUnit Current { get; set; }     

        public CacheUnit(ICacheUnit cacheUnit)
        {
            Current = cacheUnit;
        }
    }
}
