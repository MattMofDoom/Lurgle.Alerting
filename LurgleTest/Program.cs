using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Lurgle.Alerting;

namespace LurgleTest
{
    internal static class Program
    {
        private static void Main()
        {
            //Disable remote certificate validation
            ServicePointManager.ServerCertificateValidationCallback += ValidateCertificate;
            var initResult = Alerting.Init();

            if (!initResult.Equals(InitResult.Success))
            {
                Console.WriteLine("Error! Init failed with {0}", initResult);
            }
            else
            {
                Console.WriteLine("Send simple body ...");
                Alert.To().Subject("Test").Send("Can you fix it?");
                Console.WriteLine("Send Razor template ...");
                Alerting.SetConfig(new AlertConfig(Alerting.Config, mailRenderer: RendererType.Razor));
                Alert.To().Subject("Test Razor Template").SendTemplateFile("Razor", new { });
                Console.WriteLine("Send Liquid template ...");
                Alerting.SetConfig(new AlertConfig(Alerting.Config, mailRenderer: RendererType.Liquid));
                Alert.To().Subject("Test Liquid Template").SendTemplateFile("Liquid", new
                {
                    Alerting.Config.AppName,
                    Alerting.Config.AppVersion,
                    MailRenderer = Alerting.Config.MailRenderer.ToString(),
                    MailSender = Alerting.Config.MailSender.ToString(),
                    Alerting.Config.MailTemplatePath,
                    Alerting.Config.MailHost,
                    MailTestTimeout = Alerting.Config.MailTestTimeout / 1000,
                    Alerting.Config.MailPort,
                    Alerting.Config.MailUseAuthentication,
                    Alerting.Config.MailUsername,
                    Alerting.Config.MailPassword,
                    Alerting.Config.MailUseTls,
                    MailTimeout = Alerting.Config.MailTimeout / 1000,
                    Alerting.Config.MailFrom,
                    Alerting.Config.MailTo,
                    Alerting.Config.MailDebug,
                    Alerting.Config.MailSubject
                });
            }
        }


        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true;
        }
    }
}