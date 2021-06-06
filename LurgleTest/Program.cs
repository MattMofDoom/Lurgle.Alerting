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
            Alert.To().Subject("Test Razor Template").SendTemplateFile("Razor", new { });
            Console.WriteLine("Send Liquid template ...");
            Alerting.Config.MailRenderer = RendererType.Liquid;
            Alerting.SetConfig(Alerting.Config);
            Alert.To().Subject("Test Liquid Template").SendTemplateFile("Liquid", new
            {
                Alerting.Config.AppName,
                Alerting.Config.AppVersion,
                MailRenderer = Alerting.Config.MailRenderer.ToString(),
                MailSender = Alerting.Config.MailSender.ToString(),
                Alerting.Config.MailTemplatePath,
                Alerting.Config.MailHost,
                Alerting.Config.MailPort,
                Alerting.Config.MailUseAuthentication,
                Alerting.Config.MailUsername,
                Alerting.Config.MailPassword,
                Alerting.Config.MailUseTls,
                MailTimeout = Alerting.Config.MailTimeout / 1000,
                Alerting.Config.MailFrom,
                Alerting.Config.MailTo
            });
        }


        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }
}
