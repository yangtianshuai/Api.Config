namespace Api.Config
{
    /// <summary>
    /// Services
    /// </summary>
    public class Services
    {
        /// <summary>
        /// Type
        /// </summary>
        public Service Type { get; set; }
        /// <summary>
        /// InstanceType
        /// </summary>
        public Service InstanceType { get; set; }
    }
    /// <summary>
    /// Service
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Package
        /// </summary>
        public string Package { get; set; }
        /// <summary>
        /// Class
        /// </summary>
        public string Class { get; set; }
    }
}
