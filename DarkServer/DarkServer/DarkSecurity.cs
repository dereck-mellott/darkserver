using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DarkServer
{
    public static class DarkSecurity
    {
        private const int hashLength = 128;
        private const int saltLength = 64;

        public static int HashLength
        {
            get { return hashLength; }
        }
        public static int SaltLength
        {
            get { return saltLength; }
        }

        public static byte[] CreateRandomSalt(int size)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[size];
            rng.GetBytes(salt);
            return salt;
        }

        public static byte[] CreateHashWithSalt(string key, byte[] salt, int size)
        {
            Rfc2898DeriveBytes hash = new Rfc2898DeriveBytes(key, salt, 10000);
            byte[] hashWithSalt = hash.GetBytes(size);
            return hashWithSalt;
        }

        public static bool CompareHashToHash(byte[] firstHash, byte[] secondHash)
        {
            for (int i = 0; i < HashLength; i++)
            {
                if (firstHash[i] != secondHash[i])
                    return false;
            }
            return true;
        }
    }
}
