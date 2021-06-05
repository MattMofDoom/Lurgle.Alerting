using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Lurgle.Alerting
{
    /// <summary>
    /// Alerting configuration. Loaded from AppSettings if available but can be configured from code.
    /// </summary>
    public class AlertConfig
    {
        private const int timeoutDefault = 60;
        private const int timeoutMin = 30000;
        private const int timeoutMax = 600000;

        /// <summary>
        /// Meaningful app name that is used for alerting. Will be auto-set if not specified.
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// App version will be determined from the binary version, but can be overriden
        /// </summary>
        public string AppVersion { get; set; }
        /// <summary>
        /// Set the MailRenderer to use for templates - Razor, Fluid, or Replace
        /// </summary>
        public RendererType MailRenderer { get; set; }
        /// <summary>
        /// Set the MailSender to use - SmtpClient or MailKit
        /// </summary>
        public SenderType MailSender { get; set; }
        /// <summary>
        /// Set the mail template path. If not specified, will attempt to automatically set to [ExePath]\Templates
        /// </summary>
        public string MailTemplatePath { get; set; }
        /// <summary>
        /// Set the SMTP mail host. Must be specified.
        /// </summary>
        public string MailHost { get; set; }
        /// <summary>
        /// Set the TCP port for SMTP mail - defaults to 25
        /// </summary>
        public int MailPort { get; set; }
        /// <summary>
        /// Set whether the mail host needs authentication
        /// </summary>
        public bool MailUseAuthentication { get; set; }
        /// <summary>
        /// Username for mail host authentication
        /// </summary>
        public string MailUsername { get; set; }
        /// <summary>
        /// Password for mail host authentication
        /// </summary>
        public string MailPassword { get; set; }
        /// <summary>
        /// Enable TLS over SMTP - defaults to false
        /// </summary>
        public bool MailUseTls { get; set; }
        /// <summary>
        /// Set the timeout for SMTP sends - defaults to 60 seconds
        /// </summary>
        public int MailTimeout { get; set; }
        /// <summary>
        /// Default From address for emails. Must be set.
        /// </summary>
        public string MailFrom { get; set; }
        /// <summary>
        /// Default To address for emails. Must be set.
        /// </summary>
        public string MailTo { get; set; }

        /// <summary>
        /// Get a config. Optionally a logging config can be passed.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static AlertConfig GetConfig(AlertConfig config = null)
        {
            AlertConfig alertConfig;
            if (config == null)
            {
                alertConfig = new AlertConfig()
                {
                    AppName = ConfigurationManager.AppSettings["AppName"],
                    MailRenderer = GetRenderer(ConfigurationManager.AppSettings["MailRenderer"]),
                    MailSender = GetSender(ConfigurationManager.AppSettings["MailSender"]),
                    MailTemplatePath = ConfigurationManager.AppSettings["MailTemplatePath"],
                    MailHost = ConfigurationManager.AppSettings["MailHost"],
                    MailPort = GetInt(ConfigurationManager.AppSettings["MailPort"]),
                    MailUseAuthentication = GetBool(ConfigurationManager.AppSettings["MailUseAuthentication"]),
                    MailUsername = ConfigurationManager.AppSettings["MailUsername"],
                    MailPassword = ConfigurationManager.AppSettings["MailPassword"],
                    MailUseTls = GetBool(ConfigurationManager.AppSettings["MailUseTls"]),
                    MailTimeout = GetTimeout(ConfigurationManager.AppSettings["MailTimeout"]),
                    MailFrom = ConfigurationManager.AppSettings["MailFrom"],
                    MailTo = ConfigurationManager.AppSettings["MailTo"]
                };
            }
            else
            {
                alertConfig = config;
            }

            bool isSuccess = true;

            //If AppName is not specified in config, attempt to populate it. Populate AppVersion while we're at it.
            try
            {
                if (string.IsNullOrEmpty(alertConfig.AppName))
                {
                    alertConfig.AppName = Assembly.GetEntryAssembly().GetName().Name;
                }

                alertConfig.AppVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
            catch
            {
                isSuccess = false;
            }

            if (!isSuccess)
            {
                try
                {
                    if (string.IsNullOrEmpty(alertConfig.AppName))
                    {
                        alertConfig.AppName = Assembly.GetExecutingAssembly().GetName().Name;
                    }

                    alertConfig.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch
                {
                    //We surrender ...
                    alertConfig.AppVersion = string.Empty;
                }
            }

            //Attempt to set the templates if it's not specified 
            if (string.IsNullOrEmpty(alertConfig.MailTemplatePath) || !Directory.Exists(alertConfig.MailTemplatePath))
            {
                alertConfig.MailTemplatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates");
            }

            if (alertConfig.MailPort <= 0)
            {
                alertConfig.MailPort = 25;
            }

            if (alertConfig.MailUseAuthentication && string.IsNullOrEmpty(alertConfig.MailUsername))
            {
                alertConfig.MailUseAuthentication = false;
            }

            return alertConfig;
        }

        /// <summary>
        /// Convert the supplied <see cref="object"/> to an <see cref="int"/><para/>
        /// This will filter out nulls that could otherwise cause exceptions
        /// </summary>
        /// <param name="sourceObject">An object that can be converted to an int</param>
        /// <returns></returns>
        public static int GetInt(object sourceObject)
        {
            string sourceString = string.Empty;

            if (!Convert.IsDBNull(sourceObject))
            {
                sourceString = (string)sourceObject;
            }

            if (int.TryParse(sourceString, out int destInt))
            {
                return destInt;
            }

            return -1;
        }

        /// <summary>
        /// Convert the supplied <see cref="object"/> to a <see cref="bool"/><para/>
        /// This will filter out nulls that could otherwise cause exceptions
        /// </summary>
        /// <param name="sourceObject">An object that can be converted to a bool</param>
        /// <returns></returns>
        public static bool GetBool(object sourceObject)
        {
            string sourceString = string.Empty;

            if (!Convert.IsDBNull(sourceObject))
            {
                sourceString = (string)sourceObject;
            }

            if (bool.TryParse(sourceString, out bool destBool))
            {
                return destBool;
            }

            return false;
        }

        /// <summary>
        /// Translate a <see cref="string"/> setting representing seconds to an <see cref="int"/>  timeout value representing milliseconds
        /// </summary>
        /// <param name="timeoutSetting">Timeout setting in seconds</param>
        /// <returns></returns>
        private static int GetTimeout(string timeoutSetting)
        {

            if (int.TryParse(timeoutSetting, out int tryTimeout))
            {
                //Convert to milliseconds
                tryTimeout *= 1000;

                if (tryTimeout > timeoutMax | tryTimeout < timeoutMin)
                {
                    tryTimeout = timeoutDefault * 1000;
                }
            }
            else
            {
                tryTimeout = timeoutDefault * 1000;
            }

            return tryTimeout;
        }

        /// <summary>
        /// Parse a config value to a <see cref="RendererType"/>
        /// </summary>
        /// <param name="configValue"></param>
        /// <returns></returns>
        private static RendererType GetRenderer(string configValue)
        {
            if (!string.IsNullOrEmpty(configValue))
            {
                if (Enum.TryParse(configValue, true, out RendererType renderer))
                {
                    return renderer;
                }
            }

            return RendererType.Replace;
        }

        /// <summary>
        /// Parse a config value to a <see cref="SenderType"/>
        /// </summary>
        /// <param name="configValue"></param>
        /// <returns></returns>
        private static SenderType GetSender(string configValue)
        {
            if (!string.IsNullOrEmpty(configValue))
            {
                if (Enum.TryParse(configValue, true, out SenderType sender))
                {
                    return sender;
                }
            }

            return SenderType.MailKit;

        }

        /// <summary>
        /// Resolve an email template filename from config, given the specified email template name<para/>
        /// Config format is EmailTemplate{template}/>
        /// </summary>
        /// <param name="template">Email template to return the filename for</param>
        public static string GetEmailTemplate(string template)
        {
            return ConfigurationManager.AppSettings[string.Format("EmailTemplate{0}", template)];
        }

        /// <summary>
        /// Resolve the email config for a given email type <see cref="string"/> 
        /// </summary>
        /// <param name="emailType">Email type to retrieve</param>
        /// <returns></returns>
        public static string GetEmailConfig(string emailType)
        {
            string emailConfig = ConfigurationManager.AppSettings[emailType];
            if (string.IsNullOrEmpty(emailConfig))
            {
                emailConfig = ConfigurationManager.AppSettings[emailType];
            }

            return emailConfig;
        }
    }

}
