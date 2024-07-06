using Quartz;
using System.Collections.Specialized;
using WebApplicationTesteAgendamento.Request;

namespace WebApplicationTesteAgendamento
{
    public class SchedulerService
    {
        private IScheduler? scheduler;
        public SchedulerService()
        {
            
        }
        public async Task StartAsync()
        {
            NameValueCollection? properties = null;
            //    = new NameValueCollection
            //    {
            //    { "quartz.serializer.type", "binary" }
            //};

            scheduler = await SchedulerBuilder.Create(properties)
           // default max concurrency is 10
           .UseDefaultThreadPool(x => x.MaxConcurrency = 5)
           // this is the default
           // .WithMisfireThreshold(TimeSpan.FromSeconds(60))
           .UsePersistentStore(x =>
           {
           //    // force job data map values to be considered as strings
           //    // prevents nasty surprises if object is accidentally serialized and then
           //    // serialization format breaks, defaults to false
            x.UseProperties = true;
           //    //x.UseClustering();
           //    // there are other SQL providers supported too
                x.UsePostgres("Host=agendamento-postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres");
               //    x.UseSqlServer("my connection string");
               //    // this requires Quartz.Serialization.SystemTextJson NuGet package
                x.UseSystemTextJsonSerializer();
           })
           // job initialization plugin handles our xml reading, without it defaults are used
           // requires Quartz.Plugins NuGet package
           //.UseXmlSchedulingConfiguration(x =>
           //{
           //    x.Files = new[] { "~/quartz_jobs.xml" };
           //    // this is the default
           //    x.FailOnFileNotFound = true;
           //    // this is not the default
           //    x.FailOnSchedulingError = true;
           //})
           .BuildScheduler();
            await scheduler.Start();
        }
        public async Task StopAsync()
        {
            if (scheduler != null)
            {
                await scheduler.Shutdown();
            }
        }
        public async Task ScheduleJobAsync(JobScheduleRequest request)
        {
            if (scheduler != null)
            {
                var jobType = Type.GetType(request.JobType);
                var jobKey = new JobKey(request.JobId);
                var job = JobBuilder.Create(jobType)
                    .WithIdentity(jobKey)
                    .SetJobData(getJobDataMap(request.JobData))
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"{request.JobId}.trigger")
                    .ForJob(jobKey)
                    .WithCronSchedule(request.CronExpression)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
        }
        private JobDataMap getJobDataMap(Dictionary<string,string> Items)
        {
            JobDataMap map = new();
            foreach (var item in Items)
            {
                map.Add(item.Key, item.Value);
            }

            return map;
        }
        public async Task UnscheduleJobAsync(string jobId)
        {
            var jobKey = new JobKey(jobId);
            if (scheduler != null)
            {
                // Verifica se o job existe antes de tentar removê-lo
                if (await scheduler.CheckExists(jobKey))
                {
                    // Desagendar todas as triggers associadas a este job, o que também remove o job
                    await scheduler.DeleteJob(jobKey);
                }
            }
        }
        public async Task<List<string>> ListScheduledJobsAsync()
        {
            var jobDetails = new List<string>();

            if (scheduler != null)
            {
                var jobGroups = await scheduler.GetJobGroupNames();
                foreach (var group in jobGroups)
                {
                    var groupMatcher = Quartz.Impl.Matchers.GroupMatcher<JobKey>.GroupContains(group);
                    var jobKeys = await scheduler.GetJobKeys(groupMatcher);
                    foreach (var jobKey in jobKeys)
                    {
                        var detail = await scheduler.GetJobDetail(jobKey);
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);
                        foreach (var trigger in triggers)
                        {
                            var nextFireTime = trigger.GetNextFireTimeUtc()?.LocalDateTime.ToString() ?? "N/A";
                            jobDetails.Add($"Job: {jobKey.Name}, Group: {jobKey.Group}, Next Fire Time: {nextFireTime}");
                        }
                    }
                }
            }

            return jobDetails;
        }
    }
}
