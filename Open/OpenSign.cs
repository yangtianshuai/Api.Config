namespace Api.Config
{
    public class OpenSign
    {        
        public string AppId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 随机数
        /// </summary>
        public string Nonce { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
    }
}
