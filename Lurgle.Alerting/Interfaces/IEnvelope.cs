using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

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
        IEnvelope To(string toAddress, string toName = null, AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Add an list of email addresses to the recipient field for the alert.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses</param>
        /// <returns></returns>
        IEnvelope To(IEnumerable<string> emailList);

        /// <summary>
        ///     Add a list of paired email address and name values to the recipient field for the alert.
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        IEnvelope To(Dictionary<string, string> emailList);

        /// <summary>
        ///     Add a single CC address to the alert. You can chain this multiple times.
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
        IEnvelope Cc(string ccAddress, string ccName = null, AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Add an array of email addresses to the CC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        IEnvelope Cc(IEnumerable<string> emailList);

        /// <summary>
        ///     Add a list of paired email address and name values to the CC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        IEnvelope Cc(Dictionary<string, string> emailList);

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
        IEnvelope Bcc(string bccAddress, string bccName = null, AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Add an array of email addresses to the BCC field for this alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses in array form</param>
        /// <returns></returns>
        IEnvelope Bcc(IEnumerable<string> emailList);

        /// <summary>
        ///     Add a list of paired email address and name values to the BCC field for the alert
        ///     <para />
        ///     As an optional parameter, this method will not add empty or null addresses to the email
        /// </summary>
        /// <param name="emailList">List of email addresses and display names as a Dictionary of Key = string, Value = string</param>
        /// <returns></returns>
        IEnvelope Bcc(Dictionary<string, string> emailList);

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
        IEnvelope ReplyTo(string replyToAddress = null, string replyToName = null,
            AddressType addressType = AddressType.Email);

        /// <summary>
        ///     Set the subject for the alert email.
        ///     <para />
        ///     Passing an empty subjectText will use the <see cref="AlertConfig.MailSubject" /> .
        /// </summary>
        /// <param name="subjectText">Subject to use for the email</param>
        /// <param name="args">Optional arguments for string replacement"</param>
        /// <returns></returns>
        IEnvelope Subject(string subjectText = null, params object[] args);

        /// <summary>
        ///     Set the Priority for the alert email.
        ///     <para />
        ///     Emails default to <see cref="AlertLevel.Normal" />
        /// </summary>
        /// <param name="alertLevel">The priority that this email should be sent with</param>
        /// <returns></returns>
        IEnvelope Priority(AlertLevel alertLevel);

        /// <summary>
        ///     Sets the current email to HTML if true (default)
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
        ///     Attach a file to the alert. You can chain this multiple times.
        ///     <para />
        ///     If the file does not exist, it will be ignored
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="contentType">ContentType, if known (optional)</param>
        /// <returns></returns>
        IEnvelope Attach(string filePath, string contentType = null);

        /// <summary>
        ///     Attach a list of files to the alert
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">List of paths to files that will be attached</param>
        /// <returns></returns>
        IEnvelope Attach(IEnumerable<string> fileList);

        /// <summary>
        ///     Attach an array of files to the alert as inline attachments
        ///     <para />
        ///     If any file does not exist, it will be ignored
        /// </summary>
        /// <param name="fileList">Array of paths to files that will be attached</param>
        /// <param name="folderLocation">Path to folder containing files</param>
        /// <returns></returns>
        IEnvelope AttachInline(IEnumerable<string> fileList, string folderLocation);

        /// <summary>
        ///     Send the alert with the specified message text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        bool Send(string msg, params object[] args);

        /// <summary>
        ///     Send the alert with the specified message text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        Task<bool> SendAsync(string msg, params object[] args);

        /// <summary>
        ///     Send the alert using the specified Razor template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using Razor template format</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        bool SendTemplate<T>(string template, T templateModel, bool isHtml = true);

        /// <summary>
        ///     Send the alert using the specified Razor template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using Razor template format</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        Task<bool> SendTemplateAsync<T>(string template, T templateModel, bool isHtml = true);

        /// <summary>
        ///     Send the alert using the specified Razor template (as configured in the application config) and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the Razor template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        /// <param name="isHtml"></param>
        bool SendTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true);

        /// <summary>
        ///     Send the alert using the specified Razor template (as configured in the application config) and model
        ///     <para />
        ///     This does not check for the file existence, so a non-existent config item or missing file will cause an exception
        ///     that can be caught.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templateConfig">Config item to load for the Razor template file</param>
        /// <param name="templateModel">The Model to apply to this  template</param>
        /// <param name="isHtml"></param>
        Task<bool> SendTemplateFileAsync<T>(string templateConfig, T templateModel, bool isHtml = true);
    }
}