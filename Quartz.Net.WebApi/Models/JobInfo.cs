namespace Quartz.Net.WebApi.Models
{
    /// <summary>
    /// 作业信息实体类
    /// </summary>
    public class JobInfo
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public required string JobName { get; set; }

        /// <summary>
        /// 任务组名称
        /// </summary>
        public required string GroupName { get; set; }

        /// <summary>
        /// Cron 表达式
        /// </summary>
        public required string CronExpression { get; set; }

        /// <summary>
        /// 上一次执行时间
        /// </summary>
        public DateTime LastExecutionTime { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public JobStatus Status { get; set; }

        /// <summary>
        /// 任务说明
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime JobCreateTime { get; set; }

        /// <summary>
        /// Http作业内容
        /// </summary>
        public required HttpJobInfo HttpJob { get; set; }
    }

    public enum JobStatus
    {
        暂停,
        正常运行,
        已结束
    }
}
