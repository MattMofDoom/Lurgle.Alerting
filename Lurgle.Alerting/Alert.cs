using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;
using FluentEmail.MailKitSmtp;
using FluentEmail.Smtp;
using Lurgle.Alerting.Classes;
using Lurgle.Alerting.Interfaces;
using static MimeMapping.MimeUtility;
using Attachment = FluentEmail.Core.Models.Attachment;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

namespace Lurgle.Alerting
{
    // ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
    // ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
    // ReSharper disable UnusedMember.Global
    // ReSharper disable UnusedType.Global

    /// <summary>
    ///     Send an alert with Lurgle.Alerting
    /// </summary>
    public sealed class Alert : IAlert, IEnvelope
    {
        private Alert()
        {
            if (string.IsNullOrEmpty(AlertSubject)) AlertSubject = Alerting.Config.MailSubject;
        }

        /// <summary>
        ///     List of attachments
        /// </summary>
        public List<Attachment> Attachments { get; private set; } = new List<Attachment>();

        /// <summary>
        ///     List of BCC addresses
        /// </summary>
        public List<Address> BccAddresses { get; private set; } = new List<Address>();

        /// <summary>
        ///     List of CC addresses
        /// </summary>
        public List<Address> CcAddresses { get; private set; } = new List<Address>();

        /// <summary>
        ///     Alternate view for email
        /// </summary>
        public AlternateView AlternateView { get; private set; }

        /// <summary>
        ///     From address
        /// </summary>
        public Address FromAddress { get; private set; } = new Address();

        /// <summary>
        ///     Is email HTML?
        /// </summary>
        public bool IsHtml { get; private set; } = true;

        /// <summary>
        ///     Email priority
        /// </summary>
        public AlertLevel AlertPriority { get; private set; } = AlertLevel.Normal;

        /// <summary>
        ///     Email reply to
        /// </summary>
        public Address ReplyToAddress { get; private set; } = new Address();

        /// <summary>
        ///     Email subject
        /// </summary>
        public string AlertSubject { get; private set; } = string.Empty;

        /// <summary>
        ///     List of To addresses
        /// </summary>
        public List<Address> ToAddresses { get; private set; } = new List<Address>();

        /// <summary>
        ///     Add method to body?
        /// </summary>
        public bool IsMethod { get; private set; }

        /// <summary>
        ///     Calling method name
        /// </summary>
        public string MethodName { get; private set; }

        /// <summary>
        ///     Add a single recipient email address to the alert. You can chain this multiple times.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email.
        ///     <para />
        ///     You can optionally supply key names to retrieve email addresses from the app config.
        /// </summary>
        /// <param name="toAddress">
        ///     Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="toName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults to <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> to read from config
        /// </param>
        /// <returns></returns>
        public IEnvelope To(string toAddress, string toName = null, AddressType addressType = AddressType.Email)
        {
            var emailAddress = string.IsNullOrEmpty(toAddress)
                ? Alerting.GetEmailAddress(Alerting.Config.MailTo, addressType)
                : Alerting.GetEmailAddress(toAddress, addressType);

            ToAddresses.AddRange(ToAddressList(emailAddress, toName));

            return this;
        }

        /// <summary>
        ///     Add an array of email addresses to the recipient field for the alert.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        public IEnvelope To(IEnumerable<string> emailList)
        {
            foreach (var toAddress in emailList)
                if (!string.IsNullOrEmpty(toAddress))
                    ToAddresses.AddRange(ToAddressList(Alerting.GetEmailAddress(toAddress)));

            return this;
        }

        /// <summary>
        ///     Add a list of paired email address and name values to the recipient field for the alert.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        public IEnvelope To(Dictionary<string, string> emailList)
        {
            foreach (var email in emailList)
                if (!string.IsNullOrEmpty(email.Key))
                {
                    var emailAddress = Alerting.GetEmailAddress(email.Key);
                    if (!string.IsNullOrEmpty(emailAddress))
                        ToAddresses.AddRange(ToAddressList(emailAddress, email.Value));
                }

            return this;
        }

        /// <summary>
        ///     Add a single CC  address to the alert. You can chain this multiple times.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email.
        /// </summary>
        /// <param name="ccAddress">
        ///     Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="ccName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults to <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> to read from config
        /// </param>
        /// <returns></returns>
        public IEnvelope Cc(string ccAddress, string ccName = null, AddressType addressType = AddressType.Email)
        {
            var emailAddress = Alerting.GetEmailAddress(ccAddress, addressType);
            if (!string.IsNullOrEmpty(emailAddress)) CcAddresses.AddRange(ToAddressList(emailAddress, ccName));

            return this;
        }

        /// <summary>
        ///     Add an array of email addresses to the CC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        public IEnvelope Cc(IEnumerable<string> emailList)
        {
            foreach (var ccAddress in emailList)
            {
                var emailAddress = Alerting.GetEmailAddress(ccAddress);
                if (!string.IsNullOrEmpty(emailAddress)) CcAddresses.AddRange(ToAddressList(emailAddress));
            }

            return this;
        }

        /// <summary>
        ///     Add a list of paired email address and name values to the CC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        public IEnvelope Cc(Dictionary<string, string> emailList)
        {
            foreach (var email in emailList)
            {
                var emailAddress = Alerting.GetEmailAddress(email.Key);
                if (!string.IsNullOrEmpty(emailAddress)) CcAddresses.AddRange(ToAddressList(emailAddress, email.Value));
            }

            return this;
        }

        /// <summary>
        ///     Add a single BCC address to the alert. You can chain this multiple times.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email.
        /// </summary>
        /// <param name="bccAddress">
        ///     Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="bccName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults to <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> to read from config
        /// </param>
        /// <returns></returns>
        public IEnvelope Bcc(string bccAddress, string bccName = null, AddressType addressType = AddressType.Email)
        {
            var emailAddress = Alerting.GetEmailAddress(bccAddress, addressType);
            if (!string.IsNullOrEmpty(emailAddress)) BccAddresses.AddRange(ToAddressList(emailAddress, bccName));

            return this;
        }

        /// <summary>
        ///     Add an array of email addresses to the BCC field for this alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        public IEnvelope Bcc(IEnumerable<string> emailList)
        {
            foreach (var bccAddress in emailList)
            {
                var emailAddress = Alerting.GetEmailAddress(bccAddress);
                if (!string.IsNullOrEmpty(emailAddress)) BccAddresses.AddRange(ToAddressList(emailAddress));
            }

            return this;
        }

        /// <summary>
        ///     Add a list of paired email address and name values to the <see cref="BccAddresses" /> field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        public IEnvelope Bcc(Dictionary<string, string> emailList)
        {
            foreach (var email in from email in emailList
                let emailAddress = Alerting.GetEmailAddress(email.Key)
                where !string.IsNullOrEmpty(emailAddress)
                select email) BccAddresses.AddRange(ToAddressList(email.Key, email.Value));

            return this;
        }

        /// <summary>
        ///     Set the Reply To address for the alert.
        ///     <para />
        ///     If no address is specified, <see cref="AlertConfig.MailFrom" /> value will be used.
        /// </summary>
        /// <param name="replyToAddress">Email address to send replies to.</param>
        /// <param name="replyToName">Display name of the recipient.</param>
        /// <param name="addressType">
        ///     Type of email address - defaults to <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> to read from config
        /// </param>
        /// <returns></returns>
        public IEnvelope ReplyTo(string replyToAddress = null, string replyToName = null,
            AddressType addressType = AddressType.Email)
        {
            var emailAddress = string.IsNullOrEmpty(replyToAddress)
                ? Alerting.GetEmailAddress(Alerting.Config.MailFrom, addressType)
                : Alerting.GetEmailAddress(replyToAddress, addressType);

            //We can only select one email address for ReplyToAddress, so pick the first
            if (!string.IsNullOrEmpty(emailAddress)) ReplyToAddress = ToAddressList(emailAddress, replyToName).First();

            return this;
        }

        /// <summary>
        ///     Set the subject for the alert email.
        ///     <para />
        ///     Passing an empty subjectText will use the <see cref="AlertConfig.MailSubject" /> .
        /// </summary>
        /// <param name="subjectText">Subject to use for the email</param>
        /// <param name="args">Optional arguments for string replacement"</param>
        /// <returns></returns>
        public IEnvelope Subject(string subjectText = null, params object[] args)
        {
            if (string.IsNullOrEmpty(subjectText))
                AlertSubject = Alerting.Config.MailSubject;
            else if (!string.IsNullOrEmpty(subjectText) && !args.Length.Equals(0))
                AlertSubject = string.Format(subjectText, args);
            else
                AlertSubject = subjectText;

            return this;
        }

        /// <summary>
        ///     Set the Priority for the alert email.
        ///     <para />
        ///     Emails default to <see cref="AlertLevel.Normal" />
        /// </summary>
        /// <param name="alertLevel">The priority that this email should be sent with</param>
        /// <returns></returns>
        public IEnvelope Priority(AlertLevel alertLevel)
        {
            AlertPriority = alertLevel;

            return this;
        }

        /// <summary>
        ///     Sets the current email to HTML if true (default).
        ///     Shouldn't be needed any more since we have HTML and alternate text implementations.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        [Obsolete]
        public IEnvelope SetHtml(bool html)
        {
            IsHtml = html;
            return this;
        }

        /// <summary>
        ///     Add the Alt view for the HTML contents.
        ///     Shouldn't be needed any more since we have HTML and alternate text implementations.
        /// </summary>
        /// <param name="messageBody"> Message body as string</param>
        /// <param name="linkedResourceList">List of linked resources</param>
        /// <returns></returns>
        [Obsolete]
        public IEnvelope AddAlternateView(string messageBody, List<LinkedResource> linkedResourceList)
        {
            if (string.IsNullOrEmpty(messageBody) || linkedResourceList.Count <= 0) return this;
            AlternateView =
                AlternateView.CreateAlternateViewFromString(messageBody, null, MediaTypeNames.Text.Html);
            foreach (var linkedResource in linkedResourceList) AlternateView.LinkedResources.Add(linkedResource);

            return this;
        }

        /// <summary>
        ///     Attach a file to the alert. You can chain this multiple times.
        ///     <para />
        ///     If the file does not exist, it will be ignored
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="contentType">ContentType, if known (optional)</param>
        /// <returns></returns>
        public IEnvelope Attach(string filePath, string contentType = null)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return this;
            AddAttachment(filePath, contentType);

            return this;
        }

        /// <summary>
        ///     Attach an array of files to the alert
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">Array of paths to files that will be attached</param>
        /// <returns></returns>
        public IEnvelope Attach(IEnumerable<string> fileList)
        {
            foreach (var filePath in fileList)
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    AddAttachment(filePath);

            return this;
        }

        /// <summary>
        ///     Attach a file opened as a stream to the alert
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public IEnvelope Attach(Stream fileStream, string fileName, string contentType = null)
        {
            AddAttachment(fileStream, fileName, contentType);

            return this;
        }

        /// <summary>
        ///     Attach an array of files to the alert as inline attachments
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">Array of paths to files that will be attached</param>
        /// <param name="folderLocation">Path to folder containing files</param>
        /// <returns></returns>
        public IEnvelope AttachInline(IEnumerable<string> fileList, string folderLocation)
        {
            foreach (var inlineFile in fileList)
            {
                GetInlineFile(inlineFile, folderLocation, out var contentId, out var filePath, out var contentType);

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) continue;
                AddAttachment(filePath, contentType, contentId, true);
            }

            return this;
        }

        /// <summary>
        ///     Send the alert with the specified message text - assumes plain text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public MailResult Send(string msg, params object[] args)
        {
            var result = SendEmail(GetEnvelopeWithBody(msg, false, null, args));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Return a rendered alert email with the specified message text - assumes plain text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public IFluentEmail Get(string msg, params object[] args)
        {
            return GetEnvelopeWithBody(msg, false, null, args);
        }

        /// <summary>
        ///     Send the alert with the specified message text and alternate body
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="altMsg">Alternate text body. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public MailResult SendHtml(string msg, string altMsg = null, params object[] args)
        {
            var result = SendEmail(GetEnvelopeWithBody(msg, true, altMsg, args));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Return a rendered alert email with the specified message text and alternate body
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="altMsg">Alternate text body. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public IFluentEmail GetHtml(string msg, string altMsg = null, params object[] args)
        {
            return GetEnvelopeWithBody(msg, true, altMsg, args);
        }

        /// <summary>
        ///     Send the alert with the specified message text - assumes plain text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public async Task<MailResult> SendAsync(string msg, params object[] args)
        {
            var result = await SendEmailAsync(GetEnvelopeWithBody(msg, false, null, args));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Send the alert with the specified message text and alternate body
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="altMsg">Alternate text body. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        public async Task<MailResult> SendHtmlAsync(string msg, string altMsg = null, params object[] args)
        {
            var result = await SendEmailAsync(GetEnvelopeWithBody(msg, true, altMsg, args));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Send the alert using the selected <see cref="RendererType" /> template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using selected <see cref="RendererType" /> template format and model</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateTemplate">Alternate text template</param>
        public MailResult SendTemplate<T>(string template, T templateModel, bool isHtml = true,
            string alternateTemplate = null)
        {
            var result = SendEmail(GetEnvelopeWithTemplate(template, templateModel, false, isHtml,
                string.IsNullOrEmpty(alternateTemplate), alternateTemplate));

            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Return a rendered alert email using the selected <see cref="RendererType" /> template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using selected <see cref="RendererType" /> template format and model</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateTemplate">Alternate text template</param>
        public IFluentEmail GetTemplate<T>(string template, T templateModel, bool isHtml = true,
            string alternateTemplate = null)
        {
            return GetEnvelopeWithTemplate(template, templateModel, false, isHtml,
                string.IsNullOrEmpty(alternateTemplate), alternateTemplate);
        }

        /// <summary>
        ///     Send the alert using selected <see cref="RendererType" /> template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using selected <see cref="RendererType" /> template format</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateTemplate">Alternate text template</param>
        public async Task<MailResult> SendTemplateAsync<T>(string template, T templateModel, bool isHtml = true,
            string alternateTemplate = null)
        {
            var result = await SendEmailAsync(GetEnvelopeWithTemplate(template, templateModel, false, isHtml,
                string.IsNullOrEmpty(alternateTemplate), alternateTemplate));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Send the alert using the selected <see cref="RendererType" /> template file and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the selected <see cref="RendererType" /> template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateText">Render the text version of your template as an alternate text</param>
        public MailResult SendTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true,
            bool alternateText = false)
        {
            var result = SendEmail(GetEnvelopeWithTemplate(templateConfig, templateModel, true, isHtml, alternateText));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Return a rendered alert email using the selected <see cref="RendererType" /> template file and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the selected <see cref="RendererType" /> template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateText">Render the text version of your template as an alternate text</param>
        public IFluentEmail GetTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true,
            bool alternateText = false)
        {
            return GetEnvelopeWithTemplate(templateConfig, templateModel, true, isHtml, alternateText);
        }

        /// <summary>
        ///     Send the alert using the selected <see cref="RendererType" /> template (as configured in the application config)
        ///     and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the selected <see cref="RendererType" /> template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateText">Render the text version of your template as an alternate text</param>
        public async Task<MailResult> SendTemplateFileAsync<T>(string templateConfig, T templateModel, bool isHtml,
            bool alternateText = false)
        {
            var result = await SendEmailAsync(GetEnvelopeWithTemplate(templateConfig, templateModel, true, isHtml, alternateText));
            ClearAttachments();

            return result;
        }

        /// <summary>
        ///     Return an envelope with the specified body
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isHtml"></param>
        /// <param name="altMsg"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private IFluentEmail GetEnvelopeWithBody(string msg, bool isHtml, string altMsg = null, params object[] args)
        {
            var body = msg;
            var altBody = altMsg;
            if (!args.Length.Equals(0))
            {
                body = string.Format(body, args);
                if (!string.IsNullOrEmpty(altBody)) altBody = string.Format(altBody, args);
            }

            if (!IsMethod)
                return !string.IsNullOrEmpty(altBody)
                    ? GetEnvelope().PlaintextAlternativeBody(altBody).Body(body, true)
                    : GetEnvelope().Body(body);
            //Doesn't make sense to append method in a HTML email
            if (!isHtml)
                body = $"[{MethodName}] {body}";
            if (!string.IsNullOrEmpty(altBody))
                altBody = $"[{MethodName}] {altBody}";

            return !string.IsNullOrEmpty(altBody)
                ? GetEnvelope().PlaintextAlternativeBody(altBody).Body(body, isHtml)
                : GetEnvelope().Body(body, isHtml);
        }


        /// <summary>
        ///     Return an envelope using the specified template
        /// </summary>
        /// <param name="templateConfig"></param>
        /// <param name="templateModel"></param>
        /// <param name="isFile"></param>
        /// <param name="isHtml"></param>
        /// <param name="alternateText"></param>
        /// <param name="alternateBody"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private IFluentEmail GetEnvelopeWithTemplate<T>(string templateConfig, T templateModel, bool isFile,
            bool isHtml = true,
            bool alternateText = false, string alternateBody = null)
        {
            var alert = GetEnvelope();
            if (isFile)
            {
                var templatePath =
                    Path.Combine(Alerting.Config.MailTemplatePath, Alerting.GetEmailTemplate(templateConfig, isHtml));
                if (isHtml && alternateText)
                    alert = alert.PlaintextAlternativeUsingTemplateFromFile(
                        Path.Combine(Alerting.Config.MailTemplatePath,
                            Alerting.GetEmailTemplate(templateConfig, false)),
                        templateModel);

                return alert.UsingTemplateFromFile(templatePath, templateModel, isHtml);
            }

            if (isHtml && alternateText && !string.IsNullOrEmpty(alternateBody))
                alert = alert.PlaintextAlternativeUsingTemplate(alternateBody, templateModel);

            return alert.UsingTemplate(templateConfig, templateModel, isHtml);
        }

        /// <summary>
        ///     Get an envelope without a body
        /// </summary>
        /// <returns></returns>
        private IFluentEmail GetEnvelope()
        {
            var email = Email.From(FromAddress.EmailAddress, FromAddress.Name)
                .To(ToAddresses)
                .CC(CcAddresses)
                .BCC(BccAddresses)
                .Subject(AlertSubject)
                .Attach(Attachments);

            if (!string.IsNullOrEmpty(ReplyToAddress.EmailAddress))
                email = email.ReplyTo(ReplyToAddress.EmailAddress, ReplyToAddress.Name);
            switch (AlertPriority)
            {
                case AlertLevel.High:
                    email = email.HighPriority();
                    break;
                case AlertLevel.Low:
                    email = email.LowPriority();
                    break;
            }

            email.Data.IsHtml = IsHtml;

            return email;
        }

        /// <summary>
        ///     Add an attachment to the envelope
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        /// <param name="contentId"></param>
        /// <param name="isInline"></param>
        private void AddAttachment(string filePath, string contentType = null, string contentId = null,
            bool isInline = false)
        {
            var attachment = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 262144);
            var fileName = Path.GetFileName(filePath);
            if (string.IsNullOrEmpty(contentId) && !isInline)
                Attachments.Add(new Attachment
                {
                    Data = attachment, Filename = fileName,
                    ContentType = string.IsNullOrEmpty(contentType) ? GetMimeMapping(filePath) : contentType
                });
            else
                Attachments.Add(new Attachment
                {
                    Data = attachment, Filename = fileName,
                    ContentType = string.IsNullOrEmpty(contentType) ? GetMimeMapping(fileName) : contentType,
                    IsInline = true,
                    ContentId = contentId
                });
        }

        /// <summary>
        ///     Add an attachment stream to the envelope
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        private void AddAttachment(Stream fileStream, string fileName, string contentType = null)
        {
            Attachments.Add(new Attachment
            {
                Data = fileStream, Filename = fileName,
                ContentType = string.IsNullOrEmpty(contentType) ? GetMimeMapping(fileName) : contentType
            });
        }

        /// <summary>
        ///     Close any open file streams and clear the list
        /// </summary>
        private void ClearAttachments()
        {
            foreach (var attachment in Attachments)
            {
                attachment.Data.Close();
                attachment.Data.Dispose();
            }

            Attachments.Clear();
        }

        /// <summary>
        ///     Return a list of valid email addresses
        /// </summary>
        /// <param name="emailValue"></param>
        /// <param name="toName"></param>
        /// <returns></returns>
        public static IEnumerable<Address> ToAddressList(string emailValue, string toName = null)
        {
            return (from emailAddress in string.Join(",", emailValue.Split(';')).Split(',').ToList()
                where Alerting.IsValidEmail(emailAddress)
                select new Address(emailAddress, toName)).ToList();
        }

        /// <summary>
        ///     Attach a list of files to the alert
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">List of paths to files that will be attached</param>
        /// <returns></returns>
        public IEnvelope Attach(List<string> fileList)
        {
            foreach (var filePath in fileList)
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    var attachment = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 262144);
                    Attachments.Add(new Attachment
                        {Data = attachment, Filename = Path.GetFileName(filePath), ContentType = null});
                }

            return this;
        }

        /// <summary>
        ///     Retrieve the configured <see cref="SenderType" /> with settings applied
        /// </summary>
        /// <returns></returns>
        private static ISender GetSender(string host)
        {
            switch (Alerting.Config.MailSender)
            {
                case SenderType.SmtpClient:
                    if (Alerting.Config.MailUseAuthentication)
                        return new SmtpSender(new SmtpClient(host, Alerting.Config.MailPort)
                        {
                            Timeout = Alerting.Config.MailTimeout,
                            EnableSsl = Alerting.Config.MailUseTls,
                            Credentials = new NetworkCredential(Alerting.Config.MailUsername,
                                Alerting.Config.MailPassword)
                        });
                    else
                        return new SmtpSender(new SmtpClient(host, Alerting.Config.MailPort)
                        {
                            Timeout = Alerting.Config.MailTimeout,
                            EnableSsl = Alerting.Config.MailUseTls
                        });

                default:
                    return new MailKitSender(new SmtpClientOptions
                    {
                        Server = host,
                        Port = Alerting.Config.MailPort,
                        UseSsl = Alerting.Config.MailUseTls,
                        RequiresAuthentication = Alerting.Config.MailUseAuthentication,
                        User = Alerting.Config.MailUsername,
                        Password = Alerting.Config.MailPassword,
                        SocketOptions = Alerting.Config.MailTlsOptions
                    });
            }
        }

        /// <summary>
        ///     Retrieve a file spec for attachment as an inline file
        /// </summary>
        /// <param name="inlineFile"></param>
        /// <param name="folderLocation"></param>
        /// <param name="contentId"></param>
        /// <param name="filePath"></param>
        /// <param name="contentType"></param>
        private static void GetInlineFile(string inlineFile, string folderLocation, out string contentId,
            out string filePath, out string contentType)
        {
            contentId = string.Empty;
            filePath = string.Empty;
            contentType = string.Empty;

            if (string.IsNullOrEmpty(inlineFile)) return;
            var fileId = inlineFile.Split('=');

            contentId = fileId[0];
            filePath = fileId[1];
            contentType = string.IsNullOrEmpty(fileId[2]) ? GetMimeMapping(filePath) : fileId[2];

            filePath = Path.Combine(folderLocation, filePath);
        }

        private static MailResult SendEmail(IFluentEmail email)
        {
            var mailResult = new MailResult() {Successful = false, ErrorMessages = new List<string>(), DeliveryAttempts = new List<DeliveryAttempt>()};

            if (Alerting.Config.MailHost.Any())
            {
                var deliveryType = DeliveryType.MailHost;

                foreach (var server in Alerting.Config.MailHost)
                {
                    var attempt = new DeliveryAttempt() {DeliveryType = deliveryType};
                    email.Sender = GetSender(server);
                    attempt.Result = email.Send();
                    mailResult.MailHost = server;
                    mailResult.DeliveryType = deliveryType;
                    mailResult.ErrorMessages = attempt.Result.ErrorMessages;
                    mailResult.MessageId = attempt.Result.MessageId;
                    mailResult.DeliveryAttempts.Add(attempt);

                    if (attempt.Result.Successful)
                    {
                        mailResult.Successful = true;
                        return mailResult;
                    }

                    deliveryType = DeliveryType.MailFallback;
                }
            }

            if (Alerting.Config.MailUseDns)
            {
                var deliveryType = Alerting.Config.MailHost.Any() ? DeliveryType.HostDnsFallback : DeliveryType.Dns;
                var domains = GetDomains(email).ToList();
                
                foreach (var domain in domains)
                {
                    var mx = Alerting.DnsResolver.Query(domain, QueryType.MX);
                    var mxServers =
                        (from mxServer in mx.Answers
                            where !string.IsNullOrEmpty(((MxRecord) mxServer).Exchange)
                            select ((MxRecord) mxServer).Exchange).Select(dummy => (string) dummy).ToList();
                    foreach (var server in mxServers)
                    {
                        var attempt = new DeliveryAttempt() {DeliveryType = deliveryType, MailHost = server};
                        email.Sender = GetSender(server);
                        attempt.Result = email.Send();
                        mailResult.MailHost = server;
                        mailResult.DeliveryType = deliveryType;
                        mailResult.ErrorMessages = attempt.Result.ErrorMessages;
                        mailResult.MessageId = attempt.Result.MessageId;
                        mailResult.DeliveryAttempts.Add(attempt);

                        if (attempt.Result.Successful)
                        {
                            mailResult.Successful = true;
                            return mailResult;
                        }

                        deliveryType = DeliveryType.DnsFallback;
                    }
                }
            }

            return mailResult;
        }

        private static async Task<MailResult> SendEmailAsync(IFluentEmail email)
        {
            var mailResult = new MailResult() { Successful = false, ErrorMessages = new List<string>(), DeliveryAttempts = new List<DeliveryAttempt>() };

            if (Alerting.Config.MailHost.Any())
            {
                var deliveryType = DeliveryType.MailHost;

                foreach (var server in Alerting.Config.MailHost)
                {
                    var attempt = new DeliveryAttempt() { DeliveryType = deliveryType };
                    email.Sender = GetSender(server);
                    attempt.Result = await email.SendAsync();
                    mailResult.ErrorMessages = attempt.Result.ErrorMessages;
                    mailResult.MessageId = attempt.Result.MessageId;
                    mailResult.DeliveryAttempts.Add(attempt);

                    if (attempt.Result.Successful)
                    {
                        mailResult.Successful = true;
                        return mailResult;
                    }

                    deliveryType = DeliveryType.MailFallback;
                }
            }

            if (Alerting.Config.MailUseDns)
            {
                var deliveryType = Alerting.Config.MailHost.Any() ? DeliveryType.HostDnsFallback : DeliveryType.Dns;
                var domains = GetDomains(email).ToList();

                foreach (var domain in domains)
                {
                    var mx = await Alerting.DnsResolver.QueryAsync(domain, QueryType.MX);
                    var mxServers =
                        (from mxServer in mx.Answers
                         where !string.IsNullOrEmpty(((MxRecord)mxServer).Exchange)
                         select ((MxRecord)mxServer).Exchange).Select(dummy => (string)dummy).ToList();
                    foreach (var server in mxServers)
                    {
                        var attempt = new DeliveryAttempt() { DeliveryType = deliveryType };
                        email.Sender = GetSender(server);
                        attempt.Result = await email.SendAsync();
                        mailResult.ErrorMessages = attempt.Result.ErrorMessages;
                        mailResult.MessageId = attempt.Result.MessageId;
                        mailResult.DeliveryAttempts.Add(attempt);

                        if (attempt.Result.Successful)
                        {
                            mailResult.Successful = true;
                            return mailResult;
                        }

                        deliveryType = DeliveryType.DnsFallback;
                    }
                }
            }

            return mailResult;
        }

        /// <summary>
        /// Parse email domains from envelope
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDomains(IFluentEmail message)
        {
            var domains = new List<string>();
            foreach (var to in message.Data.ToAddresses)
            {
                var toDomain = to.EmailAddress.Split('@')[1];
                if (string.IsNullOrEmpty(toDomain)) continue;
                if (!domains.Any(domain => domain.Equals(toDomain, StringComparison.OrdinalIgnoreCase)))
                    domains.Add(toDomain);
            }

            foreach (var cc in message.Data.CcAddresses)
            {
                var ccDomain = cc.EmailAddress.Split('@')[1];
                if (string.IsNullOrEmpty(ccDomain)) continue;
                if (!domains.Any(domain => domain.Equals(ccDomain, StringComparison.OrdinalIgnoreCase)))
                    domains.Add(ccDomain);
            }


            foreach (var bcc in message.Data.BccAddresses)
            {
                var bccDomain = bcc.EmailAddress.Split('@')[1];
                if (string.IsNullOrEmpty(bccDomain)) continue;
                if (!domains.Any(domain => domain.Equals(bccDomain, StringComparison.OrdinalIgnoreCase)))
                    domains.Add(bccDomain);
            }

            return domains;
        }

        /// <summary>
        ///     Instantiate a new email with the desired From address.
        ///     <para />
        ///     Uses <see cref="AlertConfig.MailFrom" /> if an email is not specified.
        /// </summary>
        /// <param name="fromAddress">Email address to send the email from</param>
        /// <param name="fromName">Display name of the sender</param>
        /// <param name="addressType">
        ///     Type of email address - defaults to <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> to read from config
        /// </param>
        /// <param name="isMethod">Add the calling method to the message text if using <see cref="Send" /> to send an email</param>
        /// <param name="methodName">Automatically captures the calling method via [CallerMemberName]</param>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnvelope From(string fromAddress = null, string fromName = null,
            AddressType addressType = AddressType.Email, bool isMethod = false,
            [CallerMemberName] string methodName = null)
        {
            if (Alerting.Config == null) Alerting.Init();

            string emailAddress;

            if (string.IsNullOrEmpty(fromAddress))
                emailAddress = Alerting.Config?.MailFrom;
            else if (addressType.Equals(AddressType.FromConfig))
                emailAddress = AlertConfig.GetEmailConfig(fromAddress);
            else
                emailAddress = fromAddress;

            return new Alert
            {
                //We can only select one email address for From, so pick the first
                FromAddress = ToAddressList(emailAddress, fromName).First(), MethodName = methodName,
                IsMethod = isMethod
            };
        }

        /// <summary>
        ///     Instantiate a new Alert class using the default from address and specified toAddress. You can chain additional
        ///     recipients to this.
        ///     <para />
        ///     Uses <see cref="AlertConfig.MailFrom" />, and <see cref="AlertConfig.MailTo" /> if an email is not specified.
        /// </summary>
        /// <param name="toAddress">
        ///     Email address to send the email to. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="toName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults to <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> to read from config
        /// </param>
        /// <param name="isMethod">Add the calling method to the message text if using <see cref="Send" /> to send an email</param>
        /// <param name="methodName">Automatically captures the calling method via [CallerMemberName]</param>
        /// <returns></returns>
        public static IEnvelope To(string toAddress = null, string toName = null,
            // ReSharper disable once MethodOverloadWithOptionalParameter
            AddressType addressType = AddressType.Email, bool isMethod = false,
            [CallerMemberName] string methodName = null)
        {
            return From(methodName: methodName, isMethod: isMethod).To(toAddress, toName, addressType);
        }
    }
}