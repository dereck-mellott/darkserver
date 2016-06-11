using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DarkServer
{
    public static class DarkFunctionManager
    {
        public static List<string> GetAccountTypes()
        {
            List<string> types = new List<string>();

            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT TypeName FROM UserAccountType", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            types.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return types;
        }

        public static List<string> GetSecurityQuestions()
        {
            List<string> questions = new List<string>();

            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT QuestionText FROM SecurityQuestionType", conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            questions.Add(reader.GetString(0));
                        }
                    }
                }
            }

            return questions;
        }

        public static int GetUserAccountID(string username)
        {
            int id = -1;

            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT UserAccountID FROM UserAccount WHERE Username='" + username + "'", conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        id = (int)result;
                    }
                    else return -1;
                }
            }

            return id;
        }

        public static Dictionary<string, byte[]> GetPasswordInfo(int ID)
        {
            byte[] hash = new byte[DarkSecurity.HashLength];
            byte[] salt = new byte[DarkSecurity.SaltLength];

            using (SqlConnection conn = DarkSQLManager.Connection)
            {
                using (SqlCommand cmd = new SqlCommand("SELECT PasswordHash, PasswordSalt FROM UserPassword WHERE UserAccountID=" + ID, DarkSQLManager.Connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        hash = (byte[])reader.GetValue(0);
                        salt = (byte[])reader.GetValue(1);
                    }
                }
            }

            return new Dictionary<string, byte[]> { { "Hash", hash }, { "Salt", salt } };
        }
    }
}