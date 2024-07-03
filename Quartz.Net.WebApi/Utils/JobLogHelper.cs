using Quartz.Net.WebApi.Models;
using Newtonsoft.Json;
namespace Quartz.Net.WebApi.Utils
{
    public class JobLogHelper
    {
        private static string _filePath;

        /// <summary>
        /// 根据作业名称和组名称获取当日的作业执行日志
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static List<JobLog> GetJobLog(string jobName, string groupName)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"jobsLog-{DateTime.Now:yyyyMMdd}.json");

            // 检查文件是否存在
            if (!File.Exists(_filePath))
            {
                return new List<JobLog>();
            }
            var jsonText = $"[{File.ReadAllText(_filePath)}]";
            var list = JsonConvert.DeserializeObject<List<JobLog>>(jsonText);
            if (list != null)
            {
                var result = list.Where(c => c.JobName == jobName && groupName == c.GroupName).OrderByDescending(c => c.RunTime).ToList();
                return result;
            }

            return null;
        }
        /// <summary>
        ///获取所有的 作业执行日志  //可以从这里拓展其他查询条件
        /// </summary>
        /// <returns></returns>
        public static List<JobLog> GetAllLogs()
        {
            List<JobLog> jobLogs = new List<JobLog>();
            var logFilePaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "jobsLog-*.json");
            logFilePaths.ToList().ForEach(c =>
            {
                var jsonText = $"[{File.ReadAllText(_filePath)}]";
                var list = JsonConvert.DeserializeObject<List<JobLog>>(jsonText);
                if (list != null) jobLogs.AddRange(list);
            });
            return jobLogs;
        }
        /// <summary>
        /// 添加作业执行日志
        /// </summary>
        /// <param name="jobLog"></param>
        public static void AddJobLog(JobLog jobLog)
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"jobsLog-{DateTime.Now:yyyyMMdd}.json");
            string json = JsonConvert.SerializeObject(jobLog) + ",\n";
            File.AppendAllText(_filePath, json);
        }
    }
}
