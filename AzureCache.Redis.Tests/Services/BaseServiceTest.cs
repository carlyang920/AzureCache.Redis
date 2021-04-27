using System;

namespace AzureCache.Redis.Tests.Services
{
    public class BaseServiceTest
    {
        protected string Sha256Encrypt(string plainTxt)
        {
            var bytes = System.Text.Encoding.Default.GetBytes(plainTxt);
            var sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            var encryptBytes = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(encryptBytes);
        }
    }
}
