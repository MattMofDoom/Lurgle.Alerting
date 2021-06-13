using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Lurgle.Alerting
{
    /// <summary>
    ///     Alerting configuration. Loaded from AppSettings if available but can be configured from code.
    /// </summary>
    public class AlertConfig
    {
        private const int SmtpTestTimeoutDefault = 3000;
        private const int SmtpTestTimeoutMin = 0;
        private const int SmtpTestTimeoutMax = 20000;
        private const int TimeoutDefault = 60000;
        private const int TimeoutMin = 30000;
        private const int TimeoutMax = 600000; // ReSharper disable UnusedMember.Global

        /// <summary>
        ///     AlertConfig constructor
        /// </summary>
        private AlertConfig()
        {
        }

        /// <summary>
        ///     Constructor that permits passing a config and optional overrides of any property
        /// </summary>
        /// <param name="config"></param>
        /// <param name="appName"></param>
        /// <param name="appVersion"></param>
        /// <param name="mailRenderer"></param>
        /// <param name="mailSender"></param>
        /// <param name="mailTemplatePath"></param>
        /// <param name="mailHost"></param>
        /// <param name="mailPort"></param>
        /// <param name="mailTestTimeout"></param>
        /// <param name="mailUseAuthentication"></param>
        /// <param name="mailUsername"></param>
        /// <param name="mailPassword"></param>
        /// <param name="mailUseTls"></param>
        /// <param name="mailTimeout"></param>
        /// <param name="mailFrom"></param>
        /// <param name="mailTo"></param>
        /// <param name="mailDebug"></param>
        /// <param name="mailSubject"></param>
        public AlertConfig(AlertConfig config = null, string appName = null, string appVersion = null,
            RendererType? mailRenderer = null,
            SenderType? mailSender = null, string mailTemplatePath = null, string mailHost = null,
            int? mailPort = null, int? mailTestTimeout = null, bool? mailUseAuthentication = null,
            string mailUsername = null,
            string mailPassword = null, bool? mailUseTls = null, int? mailTimeout = null, string mailFrom = null,
            string mailTo = null, string mailDebug = null, string mailSubject = null)
        {
            if (config != null)
            {
                AppName = config.AppName;
                AppVersion = config.AppVersion;
                MailRenderer = config.MailRenderer;
                MailSender = config.MailSender;
                MailTemplatePath = config.MailTemplatePath;
                MailHost = config.MailHost;
                MailPort = config.MailPort;
                MailTestTimeout = config.MailTestTimeout;
                MailUseAuthentication = config.MailUseAuthentication;
                MailUsername = config.MailUsername;
                MailPassword = config.MailPassword;
                MailUseTls = config.MailUseTls;
                MailTimeout = config.MailTimeout;
                MailFrom = config.MailFrom;
                MailTo = config.MailTo;
                MailDebug = config.MailDebug;
                MailSubject = config.MailSubject;
            }

            if (!string.IsNullOrEmpty(appName))
                AppName = appName;
            if (!string.IsNullOrEmpty(appVersion))
                AppVersion = appVersion;
            if (mailRenderer != null)
                MailRenderer = (RendererType) mailRenderer;
            if (mailSender != null)
                MailSender = (SenderType) mailSender;
            if (!string.IsNullOrEmpty(mailTemplatePath))
                MailTemplatePath = mailTemplatePath;
            if (!string.IsNullOrEmpty(mailHost))
                MailHost = mailHost;
            if (mailPort != null)
                MailPort = (int) mailPort;
            if (mailTestTimeout != null)
                MailTestTimeout = (int) mailTestTimeout;
            if (mailUseAuthentication != null)
                MailUseAuthentication = (bool) mailUseAuthentication;
            if (!string.IsNullOrEmpty(mailUsername))
                MailUsername = mailUsername;
            if (!string.IsNullOrEmpty(mailPassword))
                MailPassword = mailPassword;
            if (mailUseTls != null)
                MailUseTls = (bool) mailUseTls;
            if (mailTimeout != null)
                MailTimeout = (int) mailTimeout;
            if (!string.IsNullOrEmpty(mailFrom))
                MailFrom = mailFrom;
            if (!string.IsNullOrEmpty(mailTo))
                MailTo = mailTo;
            if (!string.IsNullOrEmpty(mailDebug))
                MailDebug = mailDebug;
            if (!string.IsNullOrEmpty(mailSubject))
                MailSubject = mailSubject;

            //Don't allow invalid TCP port
            if (MailPort <= 0 || MailPort > 65535)
                MailPort = 25;

            //Don't accept timeouts less than 1 second (unless 0, which disables the test), negatives, or higher than 20 seconds
            if (MailTestTimeout > SmtpTestTimeoutMax || MailTestTimeout < SmtpTestTimeoutMin ||
                MailTestTimeout > 0 && MailTestTimeout < 1000)
                MailTestTimeout = SmtpTestTimeoutDefault;

            //Disable authentication if a username isn't specified
            if (MailUseAuthentication && string.IsNullOrEmpty(MailUsername))
                MailUseAuthentication = false;

            if (string.IsNullOrEmpty(MailSubject))
                MailSubject = "Alert!";
        }

        /// <summary>
        ///     Meaningful app name that is used for alerting. Will be auto-set if not specified.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string AppName { get; private set; }

        /// <summary>
        ///     App version will be determined from the binary version, but can be overriden
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public string AppVersion { get; private set; }

        /// <summary>
        ///     Set the MailRenderer to use for templates - Razor, Fluid, or Replace
        /// </summary>
        public RendererType MailRenderer { get; private set; }

        /// <summary>
        ///     Set the MailSender to use - SmtpClient or MailKit
        /// </summary>
        public SenderType MailSender { get; private set; }

        /// <summary>
        ///     Set the mail template path. If not specified, will attempt to automatically set to [ExePath]\Templates
        /// </summary>
        public string MailTemplatePath { get; private set; }

        /// <summary>
        ///     Set the SMTP mail host. Must be specified.
        /// </summary>
        public string MailHost { get; private set; }

        /// <summary>
        ///     Set the TCP port for SMTP mail - defaults to 25
        /// </summary>
        public int MailPort { get; private set; }

        /// <summary>
        ///     Interval in seconds for the SMTP test to timeout
        /// </summary>
        public int MailTestTimeout { get; private set; }

        /// <summary>
        ///     Set whether the mail host needs authentication
        /// </summary>
        public bool MailUseAuthentication { get; private set; }

        /// <summary>
        ///     Username for mail host authentication
        /// </summary>
        public string MailUsername { get; private set; }

        /// <summary>
        ///     Password for mail host authentication
        /// </summary>
        public string MailPassword { get; private set; }

        /// <summary>
        ///     Enable TLS over SMTP - defaults to false
        /// </summary>
        public bool MailUseTls { get; private set; }

        /// <summary>
        ///     Set the timeout for SMTP sends - defaults to 60 seconds
        /// </summary>
        public int MailTimeout { get; private set; }

        /// <summary>
        ///     Default From address for emails. Must be set.
        /// </summary>
        public string MailFrom { get; private set; }

        /// <summary>
        ///     Default To address for emails. Must be set.
        /// </summary>
        public string MailTo { get; private set; }

        /// <summary>
        ///     Default recipient address to substitute if IsDebug is enabled.
        /// </summary>
        public string MailDebug { get; private set; }

        /// <summary>
        ///     Default subject for emails. Defaults to "Alert!"
        /// </summary>
        public string MailSubject { get; private set; }

        /// <summary>
        ///     Get a config. Optionally a logging config can be passed.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static AlertConfig GetConfig(AlertConfig config = null)
        {
            AlertConfig alertConfig;
            if (config == null)
                alertConfig = new AlertConfig
                {
                    AppName = ConfigurationManager.AppSettings["AppName"],
                    MailRenderer = GetRenderer(ConfigurationManager.AppSettings["MailRenderer"]),
                    MailSender = GetSender(ConfigurationManager.AppSettings["MailSender"]),
                    MailTemplatePath = ConfigurationManager.AppSettings["MailTemplatePath"],
                    MailHost = ConfigurationManager.AppSettings["MailHost"],
                    MailPort = GetInt(ConfigurationManager.AppSettings["MailPort"]),
                    MailTestTimeout = GetTestTimeout(ConfigurationManager.AppSettings["MailTestTimeout"]),
                    MailUseAuthentication = GetBool(ConfigurationManager.AppSettings["MailUseAuthentication"]),
                    MailUsername = ConfigurationManager.AppSettings["MailUsername"],
                    MailPassword = ConfigurationManager.AppSettings["MailPassword"],
                    MailUseTls = GetBool(ConfigurationManager.AppSettings["MailUseTls"]),
                    MailTimeout = GetTimeout(ConfigurationManager.AppSettings["MailTimeout"]),
                    MailFrom = ConfigurationManager.AppSettings["MailFrom"],
                    MailTo = ConfigurationManager.AppSettings["MailTo"],
                    MailDebug = ConfigurationManager.AppSettings["MailDebug"],
                    MailSubject = ConfigurationManager.AppSettings["MailSubject"]
                };
            else
                alertConfig = config;

            var isSuccess = true;

            //If AppName is not specified in config, attempt to populate it. Populate AppVersion while we're at it.
            try
            {
                if (string.IsNullOrEmpty(alertConfig.AppName))
                    alertConfig.AppName = Assembly.GetEntryAssembly()?.GetName().Name;

                alertConfig.AppVersion = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
            }
            catch
            {
                isSuccess = false;
            }

            if (!isSuccess)
                try
                {
                    if (string.IsNullOrEmpty(alertConfig.AppName))
                        alertConfig.AppName = Assembly.GetExecutingAssembly().GetName().Name;

                    alertConfig.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch
                {
                    //We surrender ...
                    alertConfig.AppVersion = string.Empty;
                }

            //Attempt to set the templates if it's not specified 
            if (string.IsNullOrEmpty(alertConfig.MailTemplatePath) || !Directory.Exists(alertConfig.MailTemplatePath))
                alertConfig.MailTemplatePath =
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                        "Templates");

            if (alertConfig.MailPort <= 0 || alertConfig.MailPort > 65535) alertConfig.MailPort = 25;

            if (alertConfig.MailUseAuthentication && string.IsNullOrEmpty(alertConfig.MailUsername))
                alertConfig.MailUseAuthentication = false;

            return alertConfig;
        }

        /// <summary>
        ///     Convert the supplied <see cref="object" /> to an <see cref="int" />
        ///     <para />
        ///     This will filter out nulls that could otherwise cause exceptions
        /// </summary>
        /// <param name="sourceObject">An object that can be converted to an int</param>
        /// <returns></returns>
        private static int GetInt(object sourceObject)
        {
            var sourceString = string.Empty;

            if (!Convert.IsDBNull(sourceObject)) sourceString = (string) sourceObject;

            if (int.TryParse(sourceString, out var destInt)) return destInt;

            return -1;
        }

        /// <summary>
        ///     Convert the supplied <see cref="object" /> to a <see cref="bool" />
        ///     <para />
        ///     This will filter out nulls that could otherwise cause exceptions
        /// </summary>
        /// <param name="sourceObject">An object that can be converted to a bool</param>
        /// <returns></returns>
        private static bool GetBool(object sourceObject)
        {
            var sourceString = string.Empty;

            if (!Convert.IsDBNull(sourceObject)) sourceString = (string) sourceObject;

            return bool.TryParse(sourceString, out var destBool) && destBool;
        }

        /// <summary>
        ///     Translate a <see cref="string" /> setting representing seconds to an <see cref="int" />  timeout value representing
        ///     milliseconds
        /// </summary>
        /// <param name="timeoutSetting">Timeout setting in seconds</param>
        /// <returns></returns>
        private static int GetTimeout(string timeoutSetting)
        {
            if (int.TryParse(timeoutSetting, out var tryTimeout))
            {
                //Convert to milliseconds
                tryTimeout *= 1000;

                if ((tryTimeout > TimeoutMax) | (tryTimeout < TimeoutMin)) tryTimeout = TimeoutDefault;
            }
            else
            {
                tryTimeout = TimeoutDefault;
            }

            return tryTimeout;
        }

        private static int GetTestTimeout(string timeoutSetting)
        {
            if (int.TryParse(timeoutSetting, out var tryTimeout))
            {
                //Convert to milliseconds
                tryTimeout *= 1000;

                if ((tryTimeout > SmtpTestTimeoutMax) | (tryTimeout < SmtpTestTimeoutMin))
                    tryTimeout = SmtpTestTimeoutDefault;
            }
            else
            {
                tryTimeout = SmtpTestTimeoutDefault;
            }

            return tryTimeout;
        }

        /// <summary>
        ///     Parse a config value to a <see cref="RendererType" />
        /// </summary>
        /// <param name="configValue"></param>
        /// <returns></returns>
        private static RendererType GetRenderer(string configValue)
        {
            if (string.IsNullOrEmpty(configValue)) return RendererType.Replace;
            return Enum.TryParse(configValue, true, out RendererType renderer) ? renderer : RendererType.Replace;
        }

        /// <summary>
        ///     Parse a config value to a <see cref="SenderType" />
        /// </summary>
        /// <param name="configValue"></param>
        /// <returns></returns>
        private static SenderType GetSender(string configValue)
        {
            if (string.IsNullOrEmpty(configValue)) return SenderType.MailKit;
            return Enum.TryParse(configValue, true, out SenderType sender) ? sender : SenderType.MailKit;
        }

        /// <summary>
        ///     Resolve an email template filename from config, given the specified email template name
        ///     <para />
        ///     Config format is EmailTemplate{template}/>
        /// </summary>
        /// <param name="template">Email template to return the filename for</param>
        public static string GetEmailTemplate(string template)
        {
            return ConfigurationManager.AppSettings[$"EmailTemplate{template}"];
        }

        /// <summary>
        ///     Resolve the email config for a given email type <see cref="string" />
        /// </summary>
        /// <param name="emailType">Email type to retrieve</param>
        /// <returns></returns>
        public static string GetEmailConfig(string emailType)
        {
            var emailConfig = ConfigurationManager.AppSettings[emailType];
            if (string.IsNullOrEmpty(emailConfig)) emailConfig = ConfigurationManager.AppSettings[emailType];

            return emailConfig;
        }
    }
}