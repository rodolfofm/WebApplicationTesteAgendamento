using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using WebApplicationTesteAgendamento.Request;

namespace WebApplicationTesteAgendamento.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulerController : ControllerBase
    {
        private readonly SchedulerService _schedulerService;

        public SchedulerController(SchedulerService schedulerService)
        {
            _schedulerService = schedulerService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartScheduler()
        {
            await _schedulerService.StartAsync();
            return Ok("Scheduler started.");
        }

        [HttpPost("stop")]
        public async Task<IActionResult> StopScheduler()
        {
            await _schedulerService.StopAsync();
            return Ok("Scheduler stopped.");
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListScheduledJobs()
        {
            var jobs = await _schedulerService.ListScheduledJobsAsync();
            return Ok(jobs);
        }

        [HttpPost("schedule")]
        public async Task<IActionResult> ScheduleJob([FromBody] JobScheduleRequest request)
        {
            await _schedulerService.ScheduleJobAsync(request);
            return Ok($"Job {request.JobId} scheduled.");
        }

        [HttpDelete("unschedule")]
        public async Task<IActionResult> UnscheduleJob([FromBody] string jobId)
        {
            await _schedulerService.UnscheduleJobAsync(jobId);
            return Ok($"Job {jobId} unscheduled.");
        }
    }
}
