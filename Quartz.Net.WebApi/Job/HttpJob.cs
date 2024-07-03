using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Quartz.Net.WebApi.Models;
using RestSharp;
using Quartz.Net.WebApi.Utils;
using Newtonsoft.Json;
namespace Quartz.Net.WebApi.Job
{
    public class HttpJob : IJob
    {
        // 用于存储http作业执行配置字典
        public static readonly Dictionary<string, HttpJobInfo> Delegates = new();
        public async Task Execute(IJobExecutionContext context)
        {
            // 从JobDataMap获取委托的键
            var delegateKey = context.JobDetail.JobDataMap.GetString("delegateKey");

            if (delegateKey != null && Delegates.TryGetValue(delegateKey, out var func))
            {
                var requestBody = new RestRequest();
                if (func.Headers != null)
                {
                    foreach (var header in func.Headers)
                    {
                        requestBody.AddHeader(header.Key, header.Value);
                    }
                }


                // 执行委托
                var content = HttpHelper.HttpRequest(func.Url, func.Request, requestBody);
                // 根据委托执行结果进行操作
                JobLogHelper.AddJobLog(new JobLog() { JobName = context.JobDetail.Key.Name, GroupName = context.JobDetail.Key.Group, RunTime = DateTime.Now, RunResult = content });
                UpdateLastExecutionTime(context.JobDetail.Key.Name, context.JobDetail.Key.Group, DateTime.Now);
            }

            await Task.CompletedTask;

        }

        /// <summary>
        /// 更新作业最后一次执行时间
        /// </summary> 
        public void UpdateLastExecutionTime(string jobName, string groupName, DateTime lastExecutionTime)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jobs.json");
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var jobInfos = JsonConvert.DeserializeObject<List<JobInfo>>(json) ?? new List<JobInfo>();

                var job = jobInfos.FirstOrDefault(j => j.JobName == jobName && j.GroupName == groupName);
                if (job != null)
                {
                    job.LastExecutionTime = lastExecutionTime;
                    string result = JsonConvert.SerializeObject(jobInfos);
                    File.WriteAllText(filePath, result);
                }
            }

        }
    }
}
