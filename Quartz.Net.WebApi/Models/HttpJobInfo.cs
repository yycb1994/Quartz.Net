using RestSharp;

namespace Quartz.Net.WebApi.Models
{
    /// <summary>
    /// http 作业执行配置类
    /// </summary>
    public class HttpJobInfo
    {
        /// <summary>
        /// api请求地址
        /// </summary>
        public required string Url { get; set; }
        /// <summary>
        /// 请求方式
        /// </summary>
        public Method Request { get; set; }
        /// <summary>
        /// RestRequest 的配置键
        /// </summary>
        public Dictionary<string, string>? Headers { get; set; }
    }
}
