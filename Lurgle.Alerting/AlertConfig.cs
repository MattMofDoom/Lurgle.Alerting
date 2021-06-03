using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Lurgle.Alerting
{
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
        public RendererType Renderer { get; set; }
        public string MailTemplatePath { get; set; }
        public string MailHost { get; set; }
        public int MailPort { get; set; }
        public bool MailUseTls { get; set; }
        public int MailTimeout { get; set; }
        public string MailFrom { get; set; }
        public string MailTo { get; set; }

        public static AlertConfig GetConfig(AlertConfig config = null)
        {
            AlertConfig alertConfig;
            if (config == null)
            {
                alertConfig = new AlertConfig()
                {
                    AppName = ConfigurationManager.AppSettings["AppName"],
                    MailTemplatePath = ConfigurationManager.AppSettings["MailTemplatePath"],
                    MailHost = ConfigurationManager.AppSettings["MailHost"],
                    MailPort = GetInt(ConfigurationManager.AppSettings["MailPort"]),
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

            //Attempt to set the templates if it's not specified 
            if (string.IsNullOrEmpty(alertConfig.MailTemplatePath) || !Directory.Exists(alertConfig.MailTemplatePath))
            {
                alertConfig.MailTemplatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates");
            }

            if (alertConfig.MailPort <= 0)
            {
                alertConfig.MailPort = 25;
            }

            return config;
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
        /// Resolve an email template filename given the specified <see cref="EmailTemplate"/>
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
