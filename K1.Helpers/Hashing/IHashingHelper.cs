using System.Threading.Tasks;

namespace K1.Libs.Infra.Hashing
{
    public interface IHashingHelper
    {
        string CreateHash(string simpleText, string privateKey, string publicKey = null);
        Task<string> CreateHashAsync(string simpleText, string privateKey, string publicKey = null);
    }
}
