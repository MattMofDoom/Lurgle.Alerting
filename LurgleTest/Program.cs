using Lurgle.Alerting;
using System;
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
            Console.WriteLine("Send simple body ...");
            Alert.To().Subject("Test").Send("Can you fix it?");
            Console.WriteLine("Send Razor template ...");
            Alerting.Config.MailRenderer = RendererType.Razor;
            Alerting.SetConfig(Alerting.Config);
            Alert.To().Subject("Test Template").SendTemplateFile("Razor", new { });
        }


        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }
}
