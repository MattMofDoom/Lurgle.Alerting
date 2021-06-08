using System.Net.Mail;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using FluentEmail.Core;
using FluentEmail.Core.Defaults;
using FluentEmail.Liquid;
using FluentEmail.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Lurgle.Alerting
{
    /// <summary>
    ///     Static Lurgle.Alerting instance that provides an interface to properties and methods for alerting
    /// </summary>
    public static class Alerting
    {
        /// <summary>
        ///     Default alert subject if not specified
        /// </summary>
        public const string DefaultSubject = "Alert!";

        /// <summary>
        ///     Current Lurgle.Alerting configuration
        /// </summary>
        public static AlertConfig Config { get; private set; }

        /// <summary>
        ///     Set the <see cref="Config" /> by passing an <see cref="AlertConfig" /> or reading from app config
        /// </summary>
        /// <param name="alertConfig"></param>
        public static void SetConfig(AlertConfig alertConfig = null)
        {
            Config = AlertConfig.GetConfig(alertConfig);

            switch (Config.MailRenderer)
            {
                case RendererType.Razor:
                    Email.DefaultRenderer = new RazorRenderer();
                    break;
                case RendererType.Fluid:
                case RendererType.Liquid:
                    Email.DefaultRenderer = new LiquidRenderer(Options.Create(new LiquidRendererOptions
                    {
                        TextEncoder = HtmlEncoder.Default,
                        FileProvider = new PhysicalFileProvider(Config.MailTemplatePath)
                    }));
                    break;
                default:
                    Email.DefaultRenderer = new ReplaceRenderer();
                    break;
            }
        }


        /// <summary>
        ///     Initialise alerting and test availability of SMTP
        /// </summary>
        public static InitResult Init()
        {
            if (Config == null) SetConfig();

            return !TestSmtp() ? InitResult.SmtpTestFailed : InitResult.Success;
        }

        /// <summary>
        ///     Test that the SMTP server is reachable
        /// </summary>
        /// <returns></returns>
        public static bool TestSmtp()
        {
            //If MailTestTimeout is 0, the test is disabled
            if (Alerting.Config.MailTestTimeout.Equals(0))
                return true;
            
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var result = socket.BeginConnect(Config.MailHost, Config.MailPort, null, null);
            // Five second timeout
            var isSuccess = result.AsyncWaitHandle.WaitOne(Alerting.Config.MailTestTimeout, true);
            socket.Close();

            return isSuccess;
        }

        /// <summary>
        ///     Attempt to resolve an email address as a valid <see cref="MailAddress" />
        /// </summary>
        /// <param name="emailAddress">Email address to resolve</param>
        /// <returns></returns>
        public static bool IsValidEmail(string emailAddress)
        {
            //Make sure email address is valid
            try
            {
                var mailAddress = new MailAddress(emailAddress);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Attempt to resolve an array of email addresses as valid <see cref="MailAddress" />
        /// </summary>
        /// <param name="emailAddresses">Array of email addresses to resolve</param>
        /// <returns></returns>
        public static bool HasValidEmails(string[] emailAddresses)
        {
            //Make sure email addresses are valid
            foreach (var emailAddress in emailAddresses)
                if (!IsValidEmail(emailAddress))
                    return false;

            return true;
        }

        /// <summary>
        ///     Resolve an email template filename given the specified template string />
        /// </summary>
        /// <param name="template">Email template to return the filename for</param>
        public static string GetEmailTemplate(string template)
        {
            var templateConfig = AlertConfig.GetEmailTemplate(template);
            if (string.IsNullOrEmpty(AlertConfig.GetEmailTemplate(template))) templateConfig = $"alert{template}.html";

            return templateConfig;
        }
    }
}