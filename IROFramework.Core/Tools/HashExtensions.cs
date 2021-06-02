using System;
using System.Text;
using IROFramework.Core.AppEnvironment;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace IROFramework.Core.Tools
{
    public static class HashExtensions
    {
        public static string HashString(string str)
        {
            var saltStr = Env.GlobalSettings.HashSalt;
            byte[] salt = Encoding.ASCII.GetBytes(saltStr);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: str,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        /// <summary>
        /// True if match.
        /// </summary>
        public static bool Compare(string str, string hash)
        {
            return HashString(str) == hash;
        }
    }
}