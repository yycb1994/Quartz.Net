using Newtonsoft.Json;
using Quartz.Impl;
using Quartz.Net.WebApi.Job;
using Quartz.Net.WebApi.Models;

namespace Quartz.Net.WebApi.Utils
{
    /// <summary>
    /// 定时任务管理器
    /// </summary>
    public class QuartzHelper
    {
        private IScheduler scheduler;
        private List<JobInfo> jobInfos;

        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jobs.json");

        /// <summary>
        /// 构造函数，初始化定时任务管理器
        /// </summary>
        public QuartzHelper()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();
            LoadJobInfosApi().Wait();

        }
        /// <summary>
        /// 保存作业信息到本地 JSON 文件
        /// </summary>
        private void SaveJobInfos()
        {
            string json = JsonConvert.SerializeObject(jobInfos);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// 加载本地 JSON 文件中的作业信息
        /// </summary>
        private async Task LoadJobInfosApi()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                jobInfos = JsonConvert.DeserializeObject<List<JobInfo>>(json) ?? new List<JobInfo>();
                foreach (var jobInfo in jobInfos)
                {
                   
                    // 创建委托的唯一键
                    var delegateKey = Guid.NewGuid().ToString();
                    // 将委托存储在静态字典中
                    HttpJob.Delegates[delegateKey] = jobInfo.HttpJob;

                    // 创建并调度作业
                    IJobDetail job = JobBuilder.Create<HttpJob>()
                        .WithIdentity(jobInfo.JobName, jobInfo.GroupName).UsingJobData("delegateKey", delegateKey) // 将委托的键添加到JobDataMap
                        .Build();

                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(jobInfo.JobName, jobInfo.GroupName)
                        .WithCronSchedule(jobInfo.CronExpression)
                        //.StartNow()
                        .Build();

                    await scheduler.ScheduleJob(job, trigger);

                    // 根据任务状态恢复或暂停任务
                    if (jobInfo.Status == JobStatus.正常运行)
                    {
                        await ResumeJob(jobInfo.JobName, jobInfo.GroupName);
                    }
                    else
                    {
                        await PauseJob(jobInfo.JobName, jobInfo.GroupName);
                    }
                }
            }
            else
            {
                jobInfos = new List<JobInfo>();
            }
        }



        #region 执行普通任务时使用，传委托时可以参考此方法
        ///// <summary>
        ///// 新建任务并立即执行
        ///// </summary>
        //[Obsolete("执行普通任务时使用，可以传委托使用")]
        //public async Task AddJob(string jobName, string groupName, string cronExpression, Func<bool> func, string description = "")
        //{
        //    if (jobInfos.Any(c => c.JobName == jobName && c.GroupName == groupName))
        //    {
        //        return;
        //    }

        //    // 创建委托的唯一键
        //    var delegateKey = Guid.NewGuid().ToString();
        //    // 将委托存储在静态字典中
        //   // MyJobClass.Delegates[delegateKey] = func;

        //    // 创建作业信息并保存到列表  需要将func 加入到jobInfo 中做作业持久化!!!!
        //    var jobInfo = new JobInfo { JobName = jobName, GroupName = groupName, CronExpression = cronExpression, Status = JobStatus.正常运行, Description = description, JobCreateTime = DateTime.Now };
        //    jobInfos.Add(jobInfo);
        //    SaveJobInfos();

        //    // 创建Quartz作业和触发器
        //    IJobDetail job = JobBuilder.Create<MyJobClass>()
        //        .WithIdentity(jobName, groupName)
        //        .UsingJobData("delegateKey", delegateKey) // 将委托的键添加到JobDataMap
        //        .Build();

        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithIdentity(jobName + "Trigger", groupName)
        //        .StartNow()
        //        .WithCronSchedule(cronExpression).WithDescription(description)
        //        .Build();

        //    await scheduler.ScheduleJob(job, trigger);

        //}

        #endregion

        /// <summary>
        /// 新建任务并立即执行
        /// </summary>       
      
        public async Task AddJobApi(string jobName, string groupName, string cronExpression, HttpJobInfo httpJobInfo, string description = "")
        {
            if (jobInfos.Any(c => c.JobName == jobName && c.GroupName == groupName))
            {
                return;
            }

            // 创建委托的唯一键
            var delegateKey = Guid.NewGuid().ToString();
            // 将委托存储在静态字典中
            HttpJob.Delegates[delegateKey] = httpJobInfo;

            // 创建作业信息并保存到列表  需要将func 加入到jobInfo 中做作业持久化!!!!
            var jobInfo = new JobInfo { JobName = jobName, GroupName = groupName, CronExpression = cronExpression, HttpJob = httpJobInfo, Status = JobStatus.正常运行, Description = description, JobCreateTime = DateTime.Now };
            jobInfos.Add(jobInfo);
            SaveJobInfos();

            // 创建Quartz作业和触发器
            IJobDetail job = JobBuilder.Create<HttpJob>()
                .WithIdentity(jobName, groupName)
                .UsingJobData("delegateKey", delegateKey) // 将委托的键添加到JobDataMap
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(jobName + "Trigger", groupName)
                .StartNow()
                .WithCronSchedule(cronExpression).WithDescription(description)
                .Build();

            await scheduler.ScheduleJob(job, trigger);

        }


        /// <summary>
        /// 暂停任务
        /// </summary>
        public async Task PauseJob(string jobName, string groupName)
        {
            await scheduler.PauseJob(new JobKey(jobName, groupName));
            var job = jobInfos.FirstOrDefault(j => j.JobName == jobName && j.GroupName == groupName);
            if (job != null)
            {
                job.Status = JobStatus.暂停;
                SaveJobInfos();
            }
        }

        /// <summary>
        /// 开启任务
        /// </summary>
        public async Task ResumeJob(string jobName, string groupName)
        {
            await scheduler.ResumeJob(new JobKey(jobName, groupName));
            var job = jobInfos.FirstOrDefault(j => j.JobName == jobName && j.GroupName == groupName);
            if (job != null)
            {
                job.Status = JobStatus.正常运行;
                SaveJobInfos();
            }
        }

        /// <summary>
        /// 立即执行任务
        /// </summary>
        public async Task TriggerJob(string jobName, string groupName)
        {
            await scheduler.TriggerJob(new JobKey(jobName, groupName));
            var job = jobInfos.FirstOrDefault(j => j.JobName == jobName && j.GroupName == groupName);
            if (job != null)
            {
                job.LastExecutionTime = DateTime.Now;
                SaveJobInfos();
            }
        }


        /// <summary>
        /// 修改任务
        /// </summary>
        public async Task ModifyJob(string jobName, string groupName, string cronExpression, HttpJobInfo httpJobInfo, string description = "")
        {
            await DeleteJob(jobName, groupName);
            await AddJobApi(jobName, groupName, cronExpression, httpJobInfo, description);
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        public async Task DeleteJob(string jobName, string groupName)
        {
            await scheduler.DeleteJob(new JobKey(jobName, groupName));
            jobInfos.RemoveAll(j => j.JobName == jobName && j.GroupName == groupName);
            SaveJobInfos();
        }

        /// <summary>
        /// 获取当前所有任务列表
        /// </summary>
        public List<JobInfo> GetAllJobs()
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                jobInfos = JsonConvert.DeserializeObject<List<JobInfo>>(json) ?? new List<JobInfo>();
                return jobInfos;
            }
            else
                return null;
            
        }


    }
}
