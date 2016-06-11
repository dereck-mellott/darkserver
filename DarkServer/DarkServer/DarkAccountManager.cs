using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DarkServer
{
    public static class DarkAccountManager
    {
        public static int CreateUserAccount(string username, bool enabled, string password,
            string accountType, string answer, string questionType)
        {
            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand("dbo.CreateUserAccount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    DateTime dateCreated = DateTime.Today;
                    byte[] passSalt = DarkSecurity.CreateRandomSalt(DarkSecurity.SaltLength);
                    byte[] passHash = DarkSecurity.CreateHashWithSalt(password, passSalt, DarkSecurity.HashLength);

                    byte[] ansSalt = DarkSecurity.CreateRandomSalt(DarkSecurity.SaltLength);
                    byte[] ansHash = DarkSecurity.CreateHashWithSalt(answer, ansSalt, DarkSecurity.HashLength);

                    cmd.Parameters.AddWithValue("@USERNAME", username);
                    cmd.Parameters.AddWithValue("@ENABLED", enabled);
                    cmd.Parameters.AddWithValue("@DATECREATED", dateCreated);
                    cmd.Parameters.AddWithValue("@PASSHASH", passHash);
                    cmd.Parameters.AddWithValue("@PASSSALT", passSalt);
                    cmd.Parameters.AddWithValue("@ACCTYPE", accountType);
                    cmd.Parameters.AddWithValue("@ANSHASH", ansHash);
                    cmd.Parameters.AddWithValue("@ANSSALT", ansSalt);
                    cmd.Parameters.AddWithValue("@QUESTYPE", questionType);
                    cmd.Parameters.Add("@RVAL", SqlDbType.Int).Direction = ParameterDirection.Output;

                    int rowsEffected = cmd.ExecuteNonQuery();

                    return (int)cmd.Parameters["@RVAL"].Value;
                }
            }
        }
    }
}
