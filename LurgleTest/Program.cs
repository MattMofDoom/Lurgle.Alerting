using System;
using System.IO;
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

            if (!initResult)
            {
                Console.WriteLine("Error! Init failed with {0}", string.Join(",", Alerting.AlertFailures.ToArray()));
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
                //Basic Handlebars templates are similar to Liquid so we re-use the Liquid template
                Console.WriteLine("Send Handlebars template ...");
                Alerting.SetConfig(new AlertConfig(Alerting.Config, mailRenderer: RendererType.Handlebars));
                Alert.To().Subject("Test Handlebars Template").SendTemplateFile("Liquid", new
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
                Console.WriteLine("Test sending with no subject");
                Alert.To().Send("Test No Subject");
                Console.WriteLine("Test FromConfig ...");
                Alert.To("EmailTest", addressType: AddressType.FromConfig).Subject("Test FromConfig")
                    .Send("Pre-configured email address");
                Console.WriteLine("Test Debug mode ...");
                Alerting.SetDebug(true);
                Alert.To().Subject("Test Debug").Send("Aaaah it's a debug mode");
                Console.WriteLine("Test attachment with SmtpClient");
                Alerting.SetConfig(new AlertConfig(Alerting.Config, mailSender: SenderType.SmtpClient));
                Alert.To().Subject("Test Attachment").Attach(Path.Combine(Alerting.Config.MailTemplatePath,
                    Alerting.GetEmailTemplate("Liquid"))).Send("Test attachment");
                Console.WriteLine("Test attachment with MailKit");
                Alerting.SetConfig(new AlertConfig(Alerting.Config, mailSender: SenderType.MailKit));
                Alert.To().Subject("Test Attachment").Attach(Path.Combine(Alerting.Config.MailTemplatePath,
                    Alerting.GetEmailTemplate("Liquid"))).Send("Test attachment");
            }
        }


        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true;
        }
    }
}