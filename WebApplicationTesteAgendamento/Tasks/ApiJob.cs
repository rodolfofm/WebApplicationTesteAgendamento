using Quartz;

namespace WebApplicationTesteAgendamento.Tasks
{
    public class ApiJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
            await Console.Out.WriteLineAsync($"Executando {this} {context.FireInstanceId}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} {context.JobDetail.Description}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} url: {data.GetString("url")}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} usuario: {data.GetString("usuario")}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} senha: {data.GetString("senha")}");
        }
    }
}
