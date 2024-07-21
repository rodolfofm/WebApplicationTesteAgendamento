using Dapper;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace ExtracaoService
{
    public class DatabaseService
    {
        public static async Task<string> ExecuteQuery(DatabaseType dbType, string connectionString, string query, string? parameters = null)
        {
            try
            {
                using var connection = ConnectionFactory.GetDbConnection(dbType, connectionString);

                var result = await connection.QueryAsync(query, parameters);
                return JsonConvert.SerializeObject(result);
            }
            catch (SqlException ex) when (dbType == DatabaseType.SqlServer)
            {
                // Tratamento específico para erros do SQL Server
                Console.WriteLine($"Erro de SQL Server: {ex.Message}");
                return JsonConvert.SerializeObject(new { error = "Erro ao executar a consulta no SQL Server." });
            }
            catch (NpgsqlException ex) when (dbType == DatabaseType.Postgres)
            {
                // Tratamento específico para erros do PostgreSQL
                Console.WriteLine($"Erro de PostgreSQL: {ex.Message}");
                return JsonConvert.SerializeObject(new { error = "Erro ao executar a consulta no PostgreSQL." });
            }
            catch (MySqlException ex) when (dbType == DatabaseType.MySql)
            {
                // Tratamento específico para erros do MySQL
                Console.WriteLine($"Erro de MySQL: {ex.Message}");
                return JsonConvert.SerializeObject(new { error = "Erro ao executar a consulta no MySQL." });
            }
            catch (OracleException ex) when (dbType == DatabaseType.Oracle)
            {
                // Tratamento específico para erros do Oracle
                Console.WriteLine($"Erro de Oracle: {ex.Message}");
                return JsonConvert.SerializeObject(new { error = "Erro ao executar a consulta no Oracle." });
            }
            catch (Exception ex)
            {
                // Tratamento genérico para outros tipos de exceções
                Console.WriteLine($"Erro genérico: {ex.Message}");
                return JsonConvert.SerializeObject(new { error = "Erro ao executar a consulta." });
            }
        }
    }
}
