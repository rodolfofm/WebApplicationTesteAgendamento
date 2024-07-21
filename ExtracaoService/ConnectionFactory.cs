using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace ExtracaoService
{
    public static class ConnectionFactory
    {
        public static IDbConnection GetDbConnection(DatabaseType dbType, string connectionString)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return new SqlConnection(connectionString);
                case DatabaseType.Postgres:
                    return new NpgsqlConnection(connectionString);
                case DatabaseType.MySql:
                    return new MySqlConnection(connectionString);
                case DatabaseType.Oracle:
                    return new OracleConnection(connectionString);
                default:
                    throw new ArgumentOutOfRangeException(nameof(dbType), $"Banco nao implementado: {dbType}");
            }
        }
    }
}
