using System.Security.Cryptography;

namespace AppSecuriAndContainer.RsaTools
{
    public class RsaExtensions
    {
        public RSA GenerateRSAPrivetKey() 
        {
            var file = File.ReadAllText(@"../private_key.pem");

            var rsa = RSA.Create();
            rsa.ImportFromPem(file);
            return rsa;


        }
    }
}
