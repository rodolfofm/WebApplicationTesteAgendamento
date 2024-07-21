using ExtracaoService;
using Quartz;

namespace WebApplicationTesteAgendamento.Tasks
{
    public class BancoJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
            
            DatabaseType dataType = ConvertStringToDatabaseType(data.GetString("dbType"));

            string retorno = await DatabaseService.ExecuteQuery(dataType, data.GetString("connectionString"), data.GetString("query"));

            await Console.Out.WriteLineAsync($"Executando {this} {context.FireInstanceId}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} {context.JobDetail.Description}");
            await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} retorno: {retorno}");
        }
        private DatabaseType ConvertStringToDatabaseType(string dataType)
        {
            // Use Enum.Parse to convert the string to the corresponding DatabaseType enum value
            // This assumes that the string exactly matches the enum names and is case-sensitive
            // Use Enum.TryParse for a more flexible and error-handling approach
            return (DatabaseType)Enum.Parse(typeof(DatabaseType), dataType);
        }
    }
}
