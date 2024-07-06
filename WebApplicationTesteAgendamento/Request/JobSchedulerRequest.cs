namespace WebApplicationTesteAgendamento.Request
{
    public class JobScheduleRequest
    {
        public required string JobId { get; set; }
        public required string JobType { get; set; }
        public required string CronExpression { get; set; }
        public required Dictionary<string, string> JobData { get; set; }
    }
}
