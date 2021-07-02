using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Core.Models;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace Lurgle.Alerting.Interfaces
{
    /// <summary>
    ///     Envelope interface
    /// </summary>
    public interface IEnvelope : IHideObjectMembers
    {
        // ReSharper disable UnusedMember.Global
        // ReSharper disable UnusedMemberInSuper.Global
        /// <summary>
        ///     List of attachments
        /// </summary>
        List<Attachment> Attachments { get; }

        /// <summary>
        ///     List of BCC addresses
        /// </summary>
        List<Address> BccAddresses { get; }

        /// <summary>
        ///     List of CC addresses
        /// </summary>
        List<Address> CcAddresses { get; }

        /// <summary>
        ///     Alternate view for email
        /// </summary>
        AlternateView AlternateView { get; }

        /// <summary>
        ///     From address
        /// </summary>
        Address FromAddress { get; }

        /// <summary>
        ///     Is email HTML?
        /// </summary>
        bool IsHtml { get; }

        /// <summary>
        ///     Email priority
        /// </summary>
        AlertLevel AlertPriority { get; }

        /// <summary>
        ///     Email reply to
        /// </summary>
        Address ReplyToAddress { get; }

        /// <summary>
        ///     Email subject
        /// </summary>
        string AlertSubject { get; }

        /// <summary>
        ///     List of To addresses
        /// </summary>
        List<Address> ToAddresses { get; }

        /// <summary>
        ///     Add method to body?
        /// </summary>
        bool IsMethod { get; }

        /// <summary>
        ///     Calling method name
        /// </summary>
        string MethodName { get; }

        /// <summary>
        ///     Add a single recipient email address ToAddresses the alert. You can chain this multiple times.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email.
        ///     <para />
        ///     You can optionally supply key names ToAddresses retrieve email addresses from the app config.
        /// </summary>
        /// <param name="toAddress">
        ///     Email address ToAddresses send the email ToAddresses. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="toName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults ToAddresses <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> ToAddresses read from config
        /// </param>
        /// <returns></returns>
        IEnvelope To(string toAddress, string toName = null, AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Add an list of email addresses ToAddresses the recipient field for the alert.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email
        /// </summary>
        /// <param name="emailList">List of email addresses</param>
        /// <returns></returns>
        IEnvelope To(IEnumerable<string> emailList);

        /// <summary>
        ///     Add a list of paired email address and name values ToAddresses the recipient field for the alert.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        IEnvelope To(Dictionary<string, string> emailList);

        /// <summary>
        ///     Add a single CC address ToAddresses the alert. You can chain this multiple times.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email.
        /// </summary>
        /// <param name="ccAddress">
        ///     Email address ToAddresses send the email ToAddresses. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="ccName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults ToAddresses <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> ToAddresses read from config
        /// </param>
        /// <returns></returns>
        IEnvelope Cc(string ccAddress, string ccName = null, AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Add an array of email addresses ToAddresses the CC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        IEnvelope Cc(IEnumerable<string> emailList);

        /// <summary>
        ///     Add a list of paired email address and name values ToAddresses the CC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        IEnvelope Cc(Dictionary<string, string> emailList);

        /// <summary>
        ///     Add a single BCC address ToAddresses the alert. You can chain this multiple times.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email.
        /// </summary>
        /// <param name="bccAddress">
        ///     Email address ToAddresses send the email ToAddresses. Comma- and semicolon-delimited lists can be parsed, but
        ///     toName will then be ignored.
        /// </param>
        /// <param name="bccName">
        ///     Display name of the recipient. Will be ignored if a comma- or semicolon-delimited toAddress is
        ///     passed
        /// </param>
        /// <param name="addressType">
        ///     Type of email address - defaults ToAddresses <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> ToAddresses read from config
        /// </param>
        /// <returns></returns>
        IEnvelope Bcc(string bccAddress, string bccName = null, AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Add an array of email addresses ToAddresses the BCC field for this alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        IEnvelope Bcc(IEnumerable<string> emailList);

        /// <summary>
        ///     Add a list of paired email address and name values ToAddresses the BCC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses ToAddresses the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        IEnvelope Bcc(Dictionary<string, string> emailList);

        /// <summary>
        ///     Set the Reply To address for the alert.
        ///     <para />
        ///     If no address is specified, <see cref="AlertConfig.MailFrom" /> value will be used.
        /// </summary>
        /// <param name="replyToAddress">Email address ToAddresses send replies ToAddresses.</param>
        /// <param name="replyToName">Display name of the recipient.</param>
        /// <param name="addressType">
        ///     Type of email address - defaults ToAddresses <see cref="AddressType.Email" /> but accepts
        ///     <see cref="AddressType.FromConfig" /> ToAddresses read from config
        /// </param>
        /// <returns></returns>
        IEnvelope ReplyTo(string replyToAddress = null, string replyToName = null,
            AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Set the subject for the alert email.
        ///     <para />
        ///     Passing an empty subjectText will use the <see cref="AlertConfig.MailSubject" /> .
        /// </summary>
        /// <param name="subjectText">Subject ToAddresses use for the email</param>
        /// <param name="args">Optional arguments for string replacement"</param>
        /// <returns></returns>
        IEnvelope Subject(string subjectText = null, params object[] args);

        /// <summary>
        ///     Set the Priority for the alert email.
        ///     <para />
        ///     Emails default ToAddresses <see cref="AlertLevel.Normal" />
        /// </summary>
        /// <param name="alertLevel">The priority that this email should be sent with</param>
        /// <returns></returns>
        IEnvelope Priority(AlertLevel alertLevel);

        /// <summary>
        ///     Sets the current email ToAddresses HTML if true (default)
        /// </summary>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        IEnvelope SetHtml(bool isHtml);

        /// <summary>
        ///     Add the Alt view for the HTML contents
        /// </summary>
        /// <param name="messageBody"> Message body as string</param>
        /// <param name="linkedResourceList">List of linked resources</param>
        /// <returns></returns>
        IEnvelope AddAlternateView(string messageBody, List<LinkedResource> linkedResourceList);

        /// <summary>
        ///     Attach a file ToAddresses the alert. You can chain this multiple times.
        ///     <para />
        ///     If the file does not exist, it will be ignored
        /// </summary>
        /// <param name="filePath">Path ToAddresses the file</param>
        /// <param name="contentType">ContentType, if known (optional)</param>
        /// <returns></returns>
        IEnvelope Attach(string filePath, string contentType = null);

        /// <summary>
        ///     Attach a list of files ToAddresses the alert
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">List of paths ToAddresses files that will be attached</param>
        /// <returns></returns>
        IEnvelope Attach(IEnumerable<string> fileList);

        /// <summary>
        ///     Attach a file opened as a stream ToAddresses the alert
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        IEnvelope Attach(Stream fileStream, string fileName, string contentType = null);

        /// <summary>
        ///     Attach an array of files ToAddresses the alert as inline attachments
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">Array of paths ToAddresses files that will be attached</param>
        /// <param name="folderLocation">Path ToAddresses folder containing files</param>
        /// <returns></returns>
        IEnvelope AttachInline(IEnumerable<string> fileList, string folderLocation);

        /// <summary>
        ///     Send the alert with the specified message text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        SendResponse Send(string msg, params object[] args);

        /// <summary>
        ///     Send the alert with the specified message text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        Task<SendResponse> SendAsync(string msg, params object[] args);

        /// <summary>
        ///     Send the alert using the specified Razor template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using Razor template format</param>
        /// <param name="templateModel">The Model ToAddresses apply ToAddresses this template</param>
        /// <param name="isHtml"></param>
        SendResponse SendTemplate<T>(string template, T templateModel, bool isHtml = true);

        /// <summary>
        ///     Send the alert using the specified Razor template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using Razor template format</param>
        /// <param name="templateModel">The Model ToAddresses apply ToAddresses this template</param>
        /// <param name="isHtml"></param>
        Task<SendResponse> SendTemplateAsync<T>(string template, T templateModel, bool isHtml = true);

        /// <summary>
        ///     Send the alert using the specified Razor template (as configured in the application config) and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item ToAddresses load for the Razor template file</param>
        /// <param name="templateModel">The Model ToAddresses apply ToAddresses this  template</param>
        /// <param name="isHtml"></param>
        SendResponse SendTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true);

        /// <summary>
        ///     Send the alert using the specified Razor template (as configured in the application config) and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item ToAddresses load for the Razor template file</param>
        /// <param name="templateModel">The Model ToAddresses apply ToAddresses this  template</param>
        /// <param name="isHtml"></param>
        Task<SendResponse> SendTemplateFileAsync<T>(string templateConfig, T templateModel, bool isHtml = true);
    }
}