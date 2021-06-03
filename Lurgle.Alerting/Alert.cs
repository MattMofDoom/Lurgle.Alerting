using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Lurgle.Alerting
{
    public sealed class Alert : IAlert, IEnvelope
    {
        private bool IsMethod { get; set; }
        private bool IsDebug { get; set; }
        private string MethodName { get; set; }
        private Address from = new Address();
        private Address replyTo = new Address();
        private List<Address> to = new List<Address>();
        private readonly List<Address> cc = new List<Address>();
        private readonly List<Address> bcc = new List<Address>();
        private AlertLevel priority = AlertLevel.Normal;
        private readonly List<FluentEmail.Core.Models.Attachment> attachments = new List<FluentEmail.Core.Models.Attachment>();
        private string subject = string.Empty;
        private bool IsHtml = true;
        private AlternateView alternateView;

        private Alert()
        {
            if (string.IsNullOrEmpty(subject))
            {
                subject = Alerting.defaultSubject;
            }
        }

        private static void GetInlineFile(string inlineFile, string folderLocation, out string contentId, out string filePath, out string contentType)
        {
            contentId = string.Empty;
            filePath = string.Empty;
            contentType = string.Empty;

            if (!string.IsNullOrEmpty(inlineFile))
            {
                string[] fileId = inlineFile.Split('=');

                contentId = fileId[0];
                filePath = fileId[1];
                contentType = fileId[2];

                filePath = Path.Combine(folderLocation, filePath);
            }
        }

        /// <summary>
        /// Resolve email addresses using the given <see cref="AddressType"/> <para/>
        /// 
        /// If <see cref="Config.isDebug"/> is true, emails will automatically be replaced with the configured debug email address.<para/>
        /// 
        /// If <see cref="Config.logEmail"/> is also true, the original email and the debug email will be written as a log entry.
        /// </summary>
        /// <param name="emailType">Email config item or email address</param>
        /// <param name="addressType">Type of email being passed</param>
        /// <returns></returns>
        private static string GetEmailAddress(string emailType, AddressType addressType, bool isDebug)
        {
            //If we have an empty string, we won't be able to resolve this, so return an empty string
            if (string.IsNullOrEmpty(emailType))
            {
                return string.Empty;
            }

            string emailAddress;
            if (addressType.Equals(AddressType.FromConfig))
            {
                emailAddress = AlertConfig.GetEmailConfig(emailType);
            }
            else
            {
                emailAddress = emailType;
            }

            //Automatically substitute for a debug email address if the debug flag is set
            if (isDebug)
            {
                emailAddress = AlertConfig.GetEmailConfig("emailDebug");
            }

            return emailAddress;
        }

        /// <summary>
        /// Instantiate a new <see cref="Alert"/>  class with the desired <see cref="from"/>  address. <para />
        /// 
        /// Uses <see cref="AlertConfig.MailFrom"/> if an email is not specified.
        /// </summary>
        /// <param name="fromAddress">Email address to send the email from</param>
        /// <param name="fromName">Display name of the sender</param>
        /// <param name="showMethod">Add the calling method to the message text if using <see cref="Send"/> to send an email</param>
        /// <param name="methodName">Automatically captures the calling method via [CallerMemberName]</param>
        /// <returns></returns>
        public static IEnvelope From(string fromAddress = null, string fromName = null, AddressType addressType = AddressType.Email, bool isDebug = false, bool isMethod = false, [CallerMemberName] string methodName = null)
        {
            if (Alerting.Config == null)
            {
                Alerting.Init();
            }

            string emailAddress;

            if (string.IsNullOrEmpty(fromAddress))
            {
                emailAddress = Alerting.Config.MailFrom;
            }
            else if (addressType.Equals(AddressType.FromConfig))
            {
                emailAddress = AlertConfig.GetEmailConfig(fromAddress);
            }
            else
            {
                emailAddress = fromAddress;
            }

            return new Alert { from = new Address(emailAddress, fromName), MethodName = methodName, IsDebug = isDebug, IsMethod = isMethod };
        }

        /// <summary>
        /// Instantiate a new Alert class using the default from address and specified toAddress. You can chain additional recipients to this.<para />
        /// 
        /// Uses <see cref="Config.MailTo"/> if an email is not specified.
        /// </summary>
        /// <param name="toAddress">Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but toName will then be ignored.</param>
        /// <param name="toName">Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is passed</param>
        /// <param name="showMethod">Add the calling method to the message text if using <see cref="Send"/> to send an email</param>
        /// <param name="methodName">Automatically captures the calling method via [CallerMemberName]</param>
        /// <returns></returns>
        public static IEnvelope To(string toAddress = null, string toName = null, AddressType addressType = AddressType.Email, bool isDebug = false, bool isMethod = false, [CallerMemberName] string methodName = null)
        {
            if (Alerting.Config == null)
            {
                Alerting.Init();
            }

            string emailAddress;
            if (string.IsNullOrEmpty(toAddress))
            {
                emailAddress = GetEmailAddress(Alerting.Config.MailTo, AddressType.Email, isDebug);
            }
            else
            {
                emailAddress = GetEmailAddress(toAddress, addressType, isDebug);
            }

            string[] emailList = string.Join(",", emailAddress.Split(';')).Split(',');

            List<Address> toList = new List<Address>();
            if (emailList.Length > 1)
            {
                foreach (string toEmail in emailList)
                {
                    if (!string.IsNullOrEmpty(toEmail))
                    {
                        toList.Add(new Address(toEmail));
                    }
                }
            }
            else
            {
                toList.Add(new Address(emailAddress, toName));
            }

            return new Alert { from = new Address(Alerting.Config.MailFrom), to = toList, MethodName = methodName, IsDebug = isDebug, IsMethod = isMethod };
        }

        /// <summary>
        /// Add a single <see cref="to"/>  address to the alert. You can chain this multiple times.<para /> 
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email.
        /// </summary>
        /// <param name="toAddress">Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but toName will then be ignored.</param>
        /// <param name="toName">Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is passed</param>
        /// <returns></returns>
        public IEnvelope To(string toAddress, string toName = null, AddressType addressType = AddressType.Email)
        {
            string emailAddress;
            if (string.IsNullOrEmpty(toAddress))
            {
                emailAddress = GetEmailAddress(Alerting.Config.MailTo, AddressType.Email, IsDebug);
            }
            else
            {
                emailAddress = GetEmailAddress(toAddress, addressType, IsDebug);
            }

            {
                string[] emailList = string.Join(",", emailAddress.Split(';')).Split(',');

                if (emailList.Length > 1)
                {
                    return To(emailList);
                }
                else
                {
                    to.Add(new Address(emailAddress, toName));
                }
            }

            return this;
        }

        /// <summary>
        /// Add an array of email addresses to the <see cref="to"/>  field for the alert.<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        public IEnvelope To(string[] emailList)
        {
            foreach (string toAddress in emailList)
            {
                if (!string.IsNullOrEmpty(toAddress))
                {
                    to.Add(new Address(GetEmailAddress(toAddress, AddressType.Email, IsDebug)));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a list of email addresses to the <see cref="to"/>  field for the alert<para/> 
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses as a List of type string</param>
        /// <returns></returns>
        public IEnvelope To(List<string> emailList)
        {
            foreach (string toAddress in emailList)
            {
                if (!string.IsNullOrEmpty(toAddress))
                {
                    to.Add(new Address(GetEmailAddress(toAddress, AddressType.Email, IsDebug)));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a list of paired email address and name values to the <see cref="to"/>  field for the alert.<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        public IEnvelope To(Dictionary<string, string> emailList)
        {
            foreach (KeyValuePair<string, string> email in emailList)
            {
                if (!string.IsNullOrEmpty(email.Key))
                {
                    string emailAddress = GetEmailAddress(email.Key, AddressType.Email, IsDebug);
                    if (!string.IsNullOrEmpty(emailAddress))
                    {
                        to.Add(new Address(emailAddress, email.Value));
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Add a single <see cref="cc"/>  address to the alert. You can chain this multiple times.<para /> 
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email.
        /// </summary>
        /// <param name="ccAddress">Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but toName will then be ignored.</param>
        /// <param name="ccName">Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is passed</param>
        /// <returns></returns>
        public IEnvelope Cc(string ccAddress, string ccName = null, AddressType addressType = AddressType.Email)
        {
            if (!string.IsNullOrEmpty(ccAddress))
            {
                string[] emailList = string.Join(",", ccAddress.Split(';')).Split(',');

                if (emailList.Length > 1)
                {
                    return Cc(emailList);
                }
            }

            string emailAddress = GetEmailAddress(ccAddress, addressType, IsDebug);
            if (!string.IsNullOrEmpty(emailAddress))
            {
                cc.Add(new Address(emailAddress, ccName));
            }

            return this;
        }

        /// <summary>
        /// Add an array of email addresses to the <see cref="cc"/> field for the alert<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        public IEnvelope Cc(string[] emailList)
        {
            foreach (string ccAddress in emailList)
            {
                string emailAddress = GetEmailAddress(ccAddress, AddressType.Email, IsDebug);
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    cc.Add(new Address(emailAddress));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a list of email addresses to the <see cref="cc"/> field for the alert<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses as a List of type string</param>
        /// <returns></returns>
        public IEnvelope Cc(List<string> emailList)
        {
            foreach (string ccAddress in emailList)
            {
                string emailAddress = GetEmailAddress(ccAddress, AddressType.Email, IsDebug);
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    cc.Add(new Address(emailAddress));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a list of paired email address and name values to the <see cref="cc"/> field for the alert<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        public IEnvelope Cc(Dictionary<string, string> emailList)
        {
            foreach (KeyValuePair<string, string> email in emailList)
            {
                string emailAddress = GetEmailAddress(email.Key, AddressType.Email, IsDebug);
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    cc.Add(new Address(emailAddress, email.Value));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a single <see cref="bcc"/> address to the alert. You can chain this multiple times.<para /> 
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email.
        /// </summary>
        /// <param name="bccAddress">Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but toName will then be ignored.</param>
        /// <param name="bccName">Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is passed</param>
        /// <returns></returns>
        public IEnvelope Bcc(string bccAddress, string bccName = null, AddressType addressType = AddressType.Email)
        {
            if (!string.IsNullOrEmpty(bccAddress))
            {
                string[] emailList = string.Join(",", bccAddress.Split(';')).Split(',');

                if (emailList.Length > 1)
                {
                    return Bcc(emailList);
                }
            }

            string emailAddress = GetEmailAddress(bccAddress, addressType, IsDebug);
            if (!string.IsNullOrEmpty(emailAddress))
            {
                bcc.Add(new Address(emailAddress, bccName));
            }

            return this;
        }

        /// <summary>
        /// Add an array of email addresses to the <see cref="bcc"/> field for this alert<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        public IEnvelope Bcc(string[] emailList)
        {
            foreach (string bccAddress in emailList)
            {
                string emailAddress = GetEmailAddress(bccAddress, AddressType.Email, IsDebug);
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    bcc.Add(new Address(emailAddress));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a list of email addresses to the <see cref="bcc"/> field for this alert<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses as a List of type string</param>
        /// <returns></returns>
        public IEnvelope Bcc(List<string> emailList)
        {
            foreach (string bccAddress in emailList)
            {
                string emailAddress = GetEmailAddress(bccAddress, AddressType.Email, IsDebug);
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    bcc.Add(new Address(emailAddress));
                }
            }

            return this;
        }

        /// <summary>
        /// Add a list of paired email address and name values to the <see cref="bcc"/> field for the alert<para/>
        /// 
        /// As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        public IEnvelope Bcc(Dictionary<string, string> emailList)
        {
            foreach (KeyValuePair<string, string> email in emailList)
            {
                string emailAddress = GetEmailAddress(email.Key, AddressType.Email, IsDebug);
                if (!string.IsNullOrEmpty(emailAddress))
                {
                    bcc.Add(new Address(email.Key, email.Value));
                }
            }

            return this;
        }

        /// <summary>
        /// Set the <see cref="replyTo"/>  address for the alert.<para /> 
        /// 
        /// If no address is specified, <see cref="Config.MailFrom"/> will be used.
        /// </summary>
        /// <param name="replyToAddress">Email address to send replies to.</param>
        /// <param name="replyToName">Display name of the recipient.</param>
        /// <returns></returns>
        public IEnvelope ReplyTo(string replyToAddress = null, string replyToName = null, AddressType addressType = AddressType.Email)
        {
            string emailAddress;
            if (string.IsNullOrEmpty(replyToAddress))
            {
                emailAddress = GetEmailAddress(Alerting.Config.MailFrom, AddressType.Email, IsDebug);
            }
            else
            {
                emailAddress = GetEmailAddress(replyToAddress, addressType, IsDebug);
            }

            if (!string.IsNullOrEmpty(emailAddress))
            {
                replyTo = new Address(emailAddress, replyToName);
            }

            return this;
        }

        /// <summary>
        /// Set the <see cref="subject"/> for the alert email.<para/>
        /// 
        /// Passing an empty subjectText will use the <see cref="Alerting.defaultSubject"/> .
        /// </summary>
        /// <param name="subjectText">Subject to use for the email</param>
        /// <param name="args">Optional arguments for string replacement"</param>
        /// <returns></returns>
        public IEnvelope Subject(string subjectText = null, params object[] args)
        {
            if (string.IsNullOrEmpty(subjectText))
            {
                subject = Alerting.defaultSubject;
            }
            else if (!string.IsNullOrEmpty(subjectText) && !args.Length.Equals(0))
            {
                subject = string.Format(subjectText, args);
            }
            else
            {
                subject = subjectText;
            }

            return this;
        }

        /// <summary>
        /// Set the <see cref="priority"/> for the alert email.<para/>
        /// 
        /// Emails default to <see cref="AlertLevel.Normal"/> 
        /// </summary>
        /// <param name="alertLevel">The priority that this email should be sent with</param>
        /// <returns></returns>
        public IEnvelope Priority(AlertLevel alertLevel)
        {
            priority = alertLevel;

            return this;

        }

        /// <summary>
        /// sets the current email to html based on passed in param
        /// </summary>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public IEnvelope SetHtml(bool isHtml)
        {
            IsHtml = isHtml;
            return this;
        }

        /// <summary>
        /// Add the Alt view for the HTML contents
        /// </summary>
        /// <param name="messageBody"> Message body as string</param>
        /// <param name="linkedResourceList">List of linked resources</param>
        /// <returns></returns>
        public IEnvelope AddAlternateView(string messageBody, List<LinkedResource> linkedResourceList)
        {
            if (!string.IsNullOrEmpty(messageBody) && linkedResourceList.Count > 0)
            {
                alternateView = AlternateView.CreateAlternateViewFromString(messageBody, null, MediaTypeNames.Text.Html);
                foreach (LinkedResource linkedResource in linkedResourceList)
                {
                    alternateView.LinkedResources.Add(linkedResource);
                }
            }

            return this;
        }

        /// <summary>
        /// Attach a file to the alert. You can chain this multiple times.<para/>
        /// 
        /// If the file does not exist, it will be ignored
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="contentType">ContentType, if known (optional)</param>
        /// <returns></returns>
        public IEnvelope Attach(string filePath, string contentType = null)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                FileStream attachment = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 262144);
                attachments.Add(new FluentEmail.Core.Models.Attachment() { Data = attachment, Filename = Path.GetFileName(filePath), ContentType = contentType });
            }
            return this;
        }

        /// <summary>
        /// Attach an array of files to the alert as inline attachments<para/>
        /// 
        /// If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">Array of paths to files that will be attached</param>
        /// <param name="folderLocation">Path to folder containing files</param>
        /// <returns></returns>
        public IEnvelope AttachInline(List<string> fileList, string folderLocation)
        {
            foreach (string inlineFile in fileList)
            {

                GetInlineFile(inlineFile, folderLocation, out string contentId, out string filePath, out string contentType);

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileStream attachment = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 262144);
                    string fileName = Path.GetFileName(filePath);
                    attachments.Add(new FluentEmail.Core.Models.Attachment() { Data = attachment, Filename = fileName, ContentType = contentType, IsInline = true, ContentId = contentId });
                }
            }

            return this;
        }


        /// <summary>
        /// Attach an array of files to the alert<para/>
        /// 
        /// If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">Array of paths to files that will be attached</param>
        /// <returns></returns>
        public IEnvelope Attach(string[] fileList)
        {
            foreach (string filePath in fileList)
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileStream attachment = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 262144);
                    attachments.Add(new FluentEmail.Core.Models.Attachment() { Data = attachment, Filename = Path.GetFileName(filePath), ContentType = null });
                }
            }

            return this;
        }

        /// <summary>
        /// Attach a list of files to the alert<para/>
        /// 
        /// If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">List of paths to files that will be attached</param>
        /// <returns></returns>
        public IEnvelope Attach(List<string> fileList)
        {
            foreach (string filePath in fileList)
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    FileStream attachment = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 262144);
                    attachments.Add(new FluentEmail.Core.Models.Attachment() { Data = attachment, Filename = Path.GetFileName(filePath), ContentType = null });
                }
            }

            return this;
        }

        /// <summary>
        /// Send the alert with the specified message text
        /// 
        /// You can optionally pass a string containing format items, and the replacement objects, and a string.format will be applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public bool Send(string msg, params object[] args)
        {
            string body = msg;
            if (!args.Length.Equals(0))
            {
                body = string.Format(msg, args);
            }

            if (IsMethod)
            {
                body = string.Format("[{0}] {1}", MethodName, body);
            }

            IFluentEmail email = Email.From(from.EmailAddress, from.Name)
                .To(to)
                .CC(cc)
                .BCC(bcc)
                .Subject(subject)
                .Attach(attachments)
                .Body(body);

            if (!string.IsNullOrEmpty(replyTo.EmailAddress))
            {
                email = email.ReplyTo(replyTo.EmailAddress, replyTo.Name);
            }

            switch (priority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Data.IsHtml = IsHtml;
            email.Sender = new SmtpSender(new SmtpClient(Alerting.Config.MailHost, Alerting.Config.MailPort) { Timeout = Alerting.Config.MailTimeout, EnableSsl = Alerting.Config.MailUseTls });
            SendResponse result = email.Send();

            //Cleanup - close any open file streams and clear the list
            foreach (FluentEmail.Core.Models.Attachment attachment in attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            attachments.Clear();

            return result.Successful;
        }

        /// <summary>
        /// Send the alert with the specified message text
        /// 
        /// You can optionally pass a string containing format items, and the replacement objects, and a string.format will be applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public async Task<bool> SendAsync(string msg, params object[] args)
        {
            string body = msg;
            if (!args.Length.Equals(0))
            {
                body = string.Format(msg, args);
            }

            if (IsMethod)
            {
                body = string.Format("[{0}] {1}", MethodName, body);
            }

            IFluentEmail email = Email.From(from.EmailAddress, from.Name)
                .To(to)
                .CC(cc)
                .BCC(bcc)
                .Subject(subject)
                .Attach(attachments)
                .Body(body);

            if (!string.IsNullOrEmpty(replyTo.EmailAddress))
            {
                email = email.ReplyTo(replyTo.EmailAddress, replyTo.Name);
            }

            switch (priority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Data.IsHtml = IsHtml;
            email.Sender = new SmtpSender(new SmtpClient(Alerting.Config.MailHost, Alerting.Config.MailPort) { Timeout = Alerting.Config.MailTimeout, EnableSsl = Alerting.Config.MailUseTls });
            SendResponse result = await email.SendAsync();

            //Cleanup - close any open file streams and clear the list
            foreach (FluentEmail.Core.Models.Attachment attachment in attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            attachments.Clear();

            return result.Successful;
        }

        /// <summary>
        /// Send the alert using the specified Razor template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using Razor template format</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        public bool SendTemplate<T>(string template, T templateModel, bool isHtml = true)
        {
            IFluentEmail email = Email.From(from.EmailAddress, from.Name)
                .To(to)
                .CC(cc)
                .BCC(bcc)
                .Subject(subject)
                .Attach(attachments)
                .UsingTemplate(template, templateModel, isHtml);

            if (!string.IsNullOrEmpty(replyTo.EmailAddress))
            {
                email = email.ReplyTo(replyTo.EmailAddress, replyTo.Name);
            }

            switch (priority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Sender = new SmtpSender(new SmtpClient(Alerting.Config.MailHost, Alerting.Config.MailPort) { Timeout = Alerting.Config.MailTimeout, EnableSsl = Alerting.Config.MailUseTls });
            SendResponse result = email.Send();

            //Cleanup - close any open file streams and clear the list
            foreach (FluentEmail.Core.Models.Attachment attachment in attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            attachments.Clear();

            return result.Successful;
        }

        /// <summary>
        /// Send the alert using the specified Razor template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using Razor template format</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        public async Task<bool> SendTemplateAsync<T>(string template, T templateModel, bool isHtml = true)
        {
            IFluentEmail email = Email.From(from.EmailAddress, from.Name)
                .To(to)
                .CC(cc)
                .BCC(bcc)
                .Subject(subject)
                .Attach(attachments)
                .UsingTemplate(template, templateModel, isHtml);

            if (!string.IsNullOrEmpty(replyTo.EmailAddress))
            {
                email = email.ReplyTo(replyTo.EmailAddress, replyTo.Name);
            }

            switch (priority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Sender = new SmtpSender(new SmtpClient(Alerting.Config.MailHost, Alerting.Config.MailPort) { Timeout = Alerting.Config.MailTimeout, EnableSsl = Alerting.Config.MailUseTls });
            SendResponse result = await email.SendAsync();

            //Cleanup - close any open file streams and clear the list
            foreach (FluentEmail.Core.Models.Attachment attachment in attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            attachments.Clear();

            return result.Successful;
        }

        /// <summary>
        /// Send the alert using the specified Razor template (as configured in the application config) and model<para/> 
        /// 
        /// This does not check for the file existence, so a non-existent config item or missing file will cause an exception that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the Razor template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        public bool SendTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true)
        {
            string templatePath = Path.Combine(Alerting.Config.MailTemplatePath, Alerting.GetEmailTemplate(templateConfig));

            IFluentEmail email = Email.From(from.EmailAddress, from.Name)
                .To(to)
                .CC(cc)
                .BCC(bcc)
                .Subject(subject)
                .Attach(attachments)
                .UsingTemplateFromFile(templatePath, templateModel, isHtml);

            if (!string.IsNullOrEmpty(replyTo.EmailAddress))
            {
                email = email.ReplyTo(replyTo.EmailAddress, replyTo.Name);
            }

            switch (priority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Sender = new SmtpSender(new SmtpClient(Alerting.Config.MailHost, Alerting.Config.MailPort) { Timeout = Alerting.Config.MailTimeout, EnableSsl = Alerting.Config.MailUseTls });
            SendResponse response = email.Send();

            //Cleanup - close any open file streams and clear the list
            foreach (FluentEmail.Core.Models.Attachment attachment in attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            attachments.Clear();

            return response.Successful;
        }

        /// <summary>
        /// Send the alert using the specified Razor template (as configured in the application config) and model<para/> 
        /// 
        /// This does not check for the file existence, so a non-existent config item or missing file will cause an exception that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the Razor template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        public async Task<bool> SendTemplateFileAsync<T>(string templateConfig, T templateModel, bool isHtml)
        {
            string templatePath = Path.Combine(Alerting.Config.MailTemplatePath, Alerting.GetEmailTemplate(templateConfig));

            IFluentEmail email = Email.From(from.EmailAddress, from.Name)
                .To(to)
                .CC(cc)
                .BCC(bcc)
                .Subject(subject)
                .Attach(attachments)
                .UsingTemplateFromFile(templatePath, templateModel, isHtml);

            if (!string.IsNullOrEmpty(replyTo.EmailAddress))
            {
                email = email.ReplyTo(replyTo.EmailAddress, replyTo.Name);
            }

            switch (priority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Sender = new SmtpSender(new SmtpClient(Alerting.Config.MailHost, Alerting.Config.MailPort) { Timeout = Alerting.Config.MailTimeout, EnableSsl = Alerting.Config.MailUseTls });
            SendResponse result = await email.SendAsync();

            //Cleanup - close any open file streams and clear the list
            foreach (FluentEmail.Core.Models.Attachment attachment in attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            attachments.Clear();

            return result.Successful;
        }
    }
}

