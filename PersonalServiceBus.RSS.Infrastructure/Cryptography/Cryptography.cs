using System.Security.Cryptography;
using System.Text;
using PersonalServiceBus.RSS.Core.Domain.Interface;

namespace PersonalServiceBus.RSS.Infrastructure.Cryptography
{
    public class Cryptography : ICryptography
    {
        public string CreateHash(string unhash)
        {
            var provider = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(unhash);
            data = provider.ComputeHash(data);
            return Encoding.ASCII.GetString(data);
        }

        public bool MatchHash(string hashdata, string unhash)
        {
            return hashdata == CreateHash(unhash);
        }
    }
}