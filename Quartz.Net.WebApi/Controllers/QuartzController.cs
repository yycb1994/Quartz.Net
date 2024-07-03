
using Microsoft.AspNetCore.Mvc;
using Quartz.Net.WebApi.Models;
using Quartz.Net.WebApi.Utils;

namespace _quartzHelper.Net.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuartzController : ControllerBase
    {
        private readonly QuartzHelper _quartzHelper;
        public QuartzController(QuartzHelper quartzHelper)
        {
            _quartzHelper = quartzHelper;
        }

        [HttpGet]
        [Route("job/GetJobs")]
        public object GetJobs()
        {
            return Ok(new {code=200,data = _quartzHelper.GetAllJobs() });
        }

        [HttpGet]
        [Route("job/GetJobLog")]
        public object GetJobLog(string jobName, string groupName)
        {
            return Ok(new { code = 200, data = JobLogHelper.GetJobLog(jobName, groupName) });         
        }
        [HttpGet]
        [Route("job/GetJobLogs")]
        public object GetJobLogs()
        {
            return Ok(new { code = 200, data = JobLogHelper.GetAllLogs() });
        }


        [HttpPost]
        [Route("job/AddJob")]
        public async Task<object> Add(JobInfo jobInfo)
        {
            try
            {
                await _quartzHelper.AddJobApi(jobInfo.JobName, jobInfo.GroupName, jobInfo.CronExpression, jobInfo.HttpJob, jobInfo.Description);
                return Ok(new { code = 200, msg = "创建成功！" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message });
            }
        }

        [HttpPost]
        [Route("job/ModifyJob")]
        public async Task<object> Edit(JobInfo jobInfo)
        {
            try
            {
                await _quartzHelper.ModifyJob(jobInfo.JobName, jobInfo.GroupName, jobInfo.CronExpression, jobInfo.HttpJob, jobInfo.Description);
                return Ok(new { code = 200, msg = "修改成功！" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("job/DeleteJob")]
        public async Task<object> Delete(string jobName, string groupName)
        {
            try
            {
                await _quartzHelper.DeleteJob(jobName, groupName);
                return Ok(new { code = 200, msg = "删除成功！" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("job/PauseJob")]
        public async Task<object> PauseJob(string jobName, string groupName)
        {
            try
            {
                await _quartzHelper.PauseJob(jobName, groupName);
                return Ok(new { code = 200, msg = "暂停成功！" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message });
            }
        }

        [HttpGet]
        [Route("job/ResumeJob")]
        public async Task<object> ResumeJob(string jobName, string groupName)
        {
            try
            {
                await _quartzHelper.ResumeJob(jobName, groupName);
                return Ok(new { code = 200, msg = "开启任务成功！" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message });
            }
        }
        [HttpGet]
        [Route("job/TriggerJob")]
        public async Task<object> TriggerJob(string jobName, string groupName)
        {
            try
            {
                await _quartzHelper.TriggerJob(jobName, groupName);
                return Ok(new { code = 200, msg = "立即执行任务命令已执行！" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message });
            }
        }
    }
}
