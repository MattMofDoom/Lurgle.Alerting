﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Lurgle.Alerting.Classes;
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
        ///     Sets the current email to HTML if true (default).
        ///     Shouldn't be needed any more since we have HTML and alternate text implementations.
        /// </summary>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        [Obsolete]
        IEnvelope SetHtml(bool isHtml);

        /// <summary>
        ///     Add the Alt view for the HTML contents.
        ///     Shouldn't typically be needed since we have HTML and alternate text implementations.
        /// </summary>
        /// <param name="messageBody"> Message body as string</param>
        /// <param name="linkedResourceList">List of linked resources</param>
        /// <returns></returns>
        [Obsolete]
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
        ///     Attach a file opened as a stream to the alert
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        IEnvelope Attach(Stream fileStream, string fileName, string contentType = null);

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
        ///     Send the alert with the specified message text - assumes plain text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        MailResult Send(string msg, params object[] args);

        /// <summary>
        ///     Return a rendered alert email with the specified message text - assumes plain text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        IFluentEmail Get(string msg, params object[] args);

        /// <summary>
        ///     Send the alert with the specified message text and alternate body
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="altMsg">Alternate text body. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        MailResult SendHtml(string msg, string altMsg = null, params object[] args);

        /// <summary>
        ///     Return a rendered alert email with the specified message text and alternate body
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="altMsg">Alternate text body. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        IFluentEmail GetHtml(string msg, string altMsg = null, params object[] args);

        /// <summary>
        ///     Send the alert with the specified message text - assumes plain text
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        Task<MailResult> SendAsync(string msg, params object[] args);

        /// <summary>
        ///     Send the alert with the specified message text and alternate body
        ///     You can optionally pass a string containing format items, and the replacement objects, and a string.format will be
        ///     applied.
        /// </summary>
        /// <param name="msg">Body of the email. Can contain format items for string replacement.</param>
        /// <param name="altMsg">Alternate text body. Can contain format items for string replacement.</param>
        /// <param name="args">Optional arguments for string replacement</param>
        Task<MailResult> SendHtmlAsync(string msg, string altMsg = null, params object[] args);

        /// <summary>
        ///     Send the alert using the selected <see cref="RendererType" /> template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using selected <see cref="RendererType" /> template format and model</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateTemplate">Alternate text template</param>
        MailResult SendTemplate<T>(string template, T templateModel, bool isHtml = true,
            string alternateTemplate = null);

        /// <summary>
        ///     Return a rendered alert email using the selected <see cref="RendererType" /> template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using selected <see cref="RendererType" /> template format and model</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateTemplate">Alternate text template</param>
        IFluentEmail GetTemplate<T>(string template, T templateModel, bool isHtml = true,
            string alternateTemplate = null);

        /// <summary>
        ///     Send the alert using the selected <see cref="RendererType" /> template and model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">Body of the email, using selected <see cref="RendererType" /> template format and model</param>
        /// <param name="templateModel">The Model to apply to this template</param>
        /// <param name="isHtml"></param>
        /// <param name="alternateTemplate">Alternate text template</param>
        Task<MailResult> SendTemplateAsync<T>(string template, T templateModel, bool isHtml = true,
            string alternateTemplate = null);

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
        MailResult SendTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true,
            bool alternateText = false);

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
        IFluentEmail GetTemplateFile<T>(string templateConfig, T templateModel, bool isHtml = true,
            bool alternateText = false);

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
        Task<MailResult> SendTemplateFileAsync<T>(string templateConfig, T templateModel, bool isHtml = true,
            bool alternateText = false);
    }
}