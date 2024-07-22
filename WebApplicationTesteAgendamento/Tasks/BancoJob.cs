using ExtracaoCompiladorExecucao.Compiler;
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

            var compiler = new Compiler();
            var result = await compiler.CompileAsync(data.GetString("codigoFonte"));

            var resultado = result.ObterResultadoJson(retorno);
            if (!resultado.Item2.Any())
            {
                await Console.Out.WriteLineAsync($"Executando {this} {context.JobDetail.Key} A soma dos valores é: {resultado.Item1}");
            }
            else
            {
                foreach (var item in resultado.Item2)
                {
                    await Console.Out.WriteLineAsync($"Erro {this} {context.JobDetail.Key} {item}");
                }
            }
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
