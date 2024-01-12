namespace Api.Config.Cache
{
    public class CacheUnit
    {
        public static ICacheUnit Current { get; private set; }     

        internal CacheUnit(ICacheUnit cacheUnit)
        {
            Current = cacheUnit;
        }
    }
}
