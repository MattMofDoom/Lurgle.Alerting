using Lurgle.Alerting;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace LurgleTest
{
    internal class Program
    {
        private static void Main()
        {
            //Disable remote certificate validation
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateCertificate);
            Alert.To().Subject("Test").Send("Can you fix it?");
        }


        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }
}
