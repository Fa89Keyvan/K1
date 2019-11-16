using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace K1.Libs.Infra.Hashing
{
    public class Sha1HashHelper : IHashingHelper
    {
        public string CreateHash(string simpleText, string privateKey, string publicKey = null)
        {
            string dataToHash = simpleText + privateKey;
            if (String.IsNullOrWhiteSpace(publicKey))
                dataToHash += publicKey;

            var dataToHashAsByteArray = Encoding.UTF8.GetBytes(dataToHash);
            var objSHA256Hashstring   = new SHA256Managed();

            var hashedDataAsByteArray = objSHA256Hashstring.ComputeHash(dataToHashAsByteArray);
            return Convert.ToBase64String(hashedDataAsByteArray);
        }

        public Task<string> CreateHashAsync(string simpleText, string privateKey, string publicKey = null)
            => Task.Run(() => CreateHash(simpleText, privateKey, publicKey));
        
    }
}
