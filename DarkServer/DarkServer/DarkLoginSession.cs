using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DarkServer
{
    public enum AuthenticationResult { InvalidLogin, InvalidSession, Success }
    public enum LoginDevice { None, Test, PC, Mobile }
    public enum LoginApp { None, Test, Browser, Desktop }
    public class DarkLoginSession
    {
        public int UserAccountID { get; private set; } = -1;
        public int ActiveLoginSessionID { get; internal set; } = -1;
        public string Username { get; private set; } = "";
        public LoginDevice LoginDevice { get; set; } = LoginDevice.None;
        public bool IsAuthenticated { get; private set; } = false;

        public DarkLoginSession() { }

        public DarkLoginSession(LoginDevice loginDevice)
        {
            Clear();
            LoginDevice = loginDevice;
        }

        public AuthenticationResult Authenticate(string username, string password)
        {
            IsAuthenticated = false;
            Username = username;
            UserAccountID = DarkFunctionManager.GetUserAccountID(username);

            if (UserAccountID == -1)
                return 0;

            Dictionary<string, byte[]> passInfo = DarkFunctionManager.GetPasswordInfo(UserAccountID);

            byte[] userPassHash = DarkSecurity.CreateHashWithSalt(password, passInfo["Salt"], DarkSecurity.HashLength);

            bool result = DarkSecurity.CompareHashToHash(passInfo["Hash"], userPassHash);

            if (result)
            {
                if (Begin())
                {
                    IsAuthenticated = true;
                    return AuthenticationResult.Success;
                }
                else
                {
                    Logoff();
                    return AuthenticationResult.InvalidSession;
                }
            }
            return AuthenticationResult.InvalidLogin;
        }

        public void Logoff()
        {
            End();
            Clear();
        }

        private void Clear()
        {
            UserAccountID = -1;
            ActiveLoginSessionID = -1;
            Username = "";
            IsAuthenticated = false;
        }

        private bool Begin()
        {
            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand("dbo.CreateActiveLoginSession", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@USERACCOUNTID", UserAccountID);
                    cmd.Parameters.AddWithValue("@LOGINDEVICEID", (int)LoginDevice);
                    cmd.Parameters.AddWithValue("@LOGINAPPID", (int)DarkServerManager.LoginApp);
                    cmd.Parameters.Add("@SESSIONID", SqlDbType.Int).Direction = ParameterDirection.Output;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        ActiveLoginSessionID = (int)cmd.Parameters["@SESSIONID"].Value;
                        return true;
                    }
                    return false;
                }
            }
        }
        private void End()
        {
            string sql = String.Format("DELETE FROM ActiveLoginSession WHERE ActiveLoginSessionID={0}", ActiveLoginSessionID);
            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
