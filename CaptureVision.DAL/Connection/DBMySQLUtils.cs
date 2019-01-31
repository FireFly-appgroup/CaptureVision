using System;
using MySql.Data.MySqlClient;

namespace CaptureVision.DAL.Connection
{
    public class DBMySQLUtils
    {
        public static MySqlConnection GetDBConnection(DBSettings dbSettings)
        {
            String connectionString = dbSettings.ConnectionString.Value;
            MySqlConnection conn = new MySqlConnection(connectionString);

            return conn;
        }
    }
}
