namespace Quartz.Net.WebApi.Models
{
    /// <summary>
    /// 作业执行日志
    /// </summary>
    public class JobLog
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 任务组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public DateTime RunTime { get; set; }
        /// <summary>
        /// 执行结果
        /// </summary>
        public string RunResult { get; set; }
    }
}
