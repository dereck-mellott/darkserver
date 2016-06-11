using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DarkServer
{
    public static class DarkSQLManager
    {
        public static string ConnectionString { get; set; }

        public static SqlConnection GetDBConnection()
        {
            var Conn = new SqlConnection(ConnectionString);
            Conn.Open();
            Conn.Disposed += Conn_Disposed;

            return Conn;
        }

        private static void Conn_Disposed(object sender, EventArgs e)
        {
            SqlConnection Conn = sender as SqlConnection;

            if (Conn.State == System.Data.ConnectionState.Open)
                Conn.Close();
        }

        public static SqlConnection Connection
        {
            get { return GetDBConnection(); }
        }
    }
}
