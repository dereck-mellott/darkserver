using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkServer
{
    public static class DarkServerManager
    {
        public static LoginApp LoginApp { get; private set; }
        public static void ApplicationStart(string connectionString, LoginApp loginApp)
        {
            DarkSQLManager.ConnectionString = connectionString;
            LoginApp = loginApp;
        }
    }
}
