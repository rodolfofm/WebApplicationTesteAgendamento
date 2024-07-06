using Quartz;

namespace WebApplicationTesteAgendamento.Tasks
{
    public class BancoJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
            await Console.Out.WriteLineAsync($"Executando {this} {context.FireInstanceId}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} {context.JobDetail.Description}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} host: {data.GetString("host")}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} banco: {data.GetString("banco")}");
        }
    }
}
