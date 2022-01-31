using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.Encodings.Web;
using DnsClient;
using FluentEmail.Core;
using FluentEmail.Core.Defaults;
using Lurgle.Alerting.Renderers;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Lurgle.Alerting
{
    /// <summary>
    ///     Static Lurgle.Alerting instance that provides an interface to properties and methods for alerting
    /// </summary>
    public static class Alerting
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedMember.Global
        // ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

        /// <summary>
        ///     If enabled, recipient addresses will be substituted with the configured debug address
        /// </summary>
        public static bool IsDebug { get; private set; }

        /// <summary>
        ///     Current Lurgle.Alerting configuration
        /// </summary>
        public static AlertConfig Config { get; private set; }

        /// <summary>
        ///     List of issues found during alerting initialisation
        /// </summary>
        public static List<InitResult> AlertFailures { get; private set; }

        /// <summary>
        ///     Static DNS resolver
        /// </summary>
        public static readonly LookupClient DnsResolver = new();

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
                case RendererType.Liquid:
                case RendererType.Fluid:
                    Email.DefaultRenderer = new LiquidRenderer(Options.Create(new LiquidRendererOptions
                    {
                        TextEncoder = HtmlEncoder.Default,
                        FileProvider = new PhysicalFileProvider(Config.MailTemplatePath)
                    }));
                    break;
                case RendererType.Handlebars:
                    Email.DefaultRenderer = new HandlebarsRenderer();
                    break;
                default:
                    Email.DefaultRenderer = new ReplaceRenderer();
                    break;
            }
        }

        /// <summary>
        ///     Enable debug mode for email substitution
        /// </summary>
        /// <param name="isDebug"></param>
        public static void SetDebug(bool isDebug)
        {
            IsDebug = isDebug;
        }

        /// <summary>
        ///     Initialise alerting and test availability of SMTP
        /// </summary>
        public static bool Init()
        {
            if (Config == null) SetConfig();

            AlertFailures = new List<InitResult>();

            if (Config != null && !Config.MailHost.Any() && !Config.MailUseDns)
                AlertFailures.Add(InitResult.MailHostNotConfigured);

            if (string.IsNullOrEmpty(Config?.MailFrom) || !Alert.ToAddressList(Config.MailFrom).Any())
                AlertFailures.Add(InitResult.FromAddressEmpty);

            if (string.IsNullOrEmpty(Config?.MailTo) || !Alert.ToAddressList(Config.MailTo).Any())
                AlertFailures.Add(InitResult.ToAddressEmpty);

            if (string.IsNullOrEmpty(Config?.MailDebug) || !Alert.ToAddressList(Config.MailDebug).Any())
                AlertFailures.Add(InitResult.DebugAddressEmpty);

            if (string.IsNullOrEmpty(Config?.MailSubject))
                AlertFailures.Add(InitResult.SubjectEmpty);

            //SMTP test should automatically fail if we don't have a mail host
            if (Config != null &&
                (!Config.MailHost.Any() && !Config.MailUseDns || Config.MailHost.Any() && !TestSmtp()))
                AlertFailures.Add(InitResult.SmtpTestFailed);

            return AlertFailures.Count.Equals(0);
        }

        /// <summary>
        ///     Test that the SMTP server is reachable
        /// </summary>
        /// <returns></returns>
        public static bool TestSmtp(string mailHost = null, int? mailPort = null, int? timeout = null)
        {
            //If MailTestTimeout is 0, the test is disabled
            if ((timeout == null || (int) timeout == 0) && Config.MailTestTimeout.Equals(0))
                return true;

            var useTimeout = timeout ?? Config.MailTestTimeout;

            var useMailHost = !string.IsNullOrEmpty(mailHost) ? new List<string> {mailHost} : Config.MailHost;

            var useMailPort = mailPort ?? Config.MailPort;

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var isSuccess = false;

            foreach (var result in useMailHost.Select(host => socket.BeginConnect(host, useMailPort, null, null)))
            {
                // X second timeout
                isSuccess = result.AsyncWaitHandle.WaitOne(useTimeout, true);
                socket.Close();

                //End on first success - at least one mail host is available
                if (isSuccess)
                    break;
            }

            return isSuccess;
        }

        /// <summary>
        ///     Resolve email addresses using the given <see cref="AddressType" />
        ///     <para />
        ///     If isDebug is true, emails will automatically be replaced with the configured debug email address.
        ///     <para />
        /// </summary>
        /// <param name="emailType">Email config item or email address</param>
        /// <param name="addressType">Type of email being passed</param>
        /// <returns></returns>
        public static string GetEmailAddress(string emailType, AddressType addressType = AddressType.Email)
        {
            //If we have an empty string, we won't be able to resolve this, so return an empty string
            if (string.IsNullOrEmpty(emailType)) return string.Empty;

            var emailAddress = addressType.Equals(AddressType.FromConfig)
                ? AlertConfig.GetEmailConfig(emailType)
                : emailType;

            //Automatically substitute for a debug email address if the debug flag is set
            return IsDebug ? Config.MailDebug : emailAddress;
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
                var unused = new MailAddress(emailAddress);
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
        public static bool HasValidEmails(IEnumerable<string> emailAddresses)
        {
            //Make sure email addresses are valid
            return emailAddresses.All(IsValidEmail);
        }

        /// <summary>
        ///     Resolve an email template filename given the specified template string />
        /// </summary>
        /// <param name="template">Email template to return the filename for</param>
        /// <param name="isHtml"></param>
        public static string GetEmailTemplate(string template, bool isHtml = true)
        {
            var templateConfig = AlertConfig.GetEmailTemplate(template);
            if (!string.IsNullOrEmpty(AlertConfig.GetEmailTemplate(template))) return templateConfig;
            return isHtml ? $"alert{template}.html" : $"alert{template}.txt";
        }
    }
}