using FluentEmail.Core.Models;

namespace Lurgle.Alerting
{
    /// <summary>
    /// Indicate whether an email address passing in to <see cref="Alert"/> is an actual email address, or if it should be queried from the config
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Email address
        /// </summary>
        Email = 0,
        /// <summary>
        /// Read from config
        /// </summary>
        FromConfig = 1
    }

    /// <summary>
    /// The priority of the email that will be sent. Abstracts FluentEmail's <see cref="Priority"/>  enum so that it does not need to be referenced outside of the <see cref="Alert"/>  class.
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal = Priority.Normal,
        /// <summary>
        /// Low
        /// </summary>
        Low = Priority.Low,
        /// <summary>
        /// High
        /// </summary>
        High = Priority.High
    }

    /// <summary>
    /// Renderer type to use with email templates
    /// </summary>
    public enum RendererType
    {
        /// <summary>
        /// Use the Razor renderer
        /// </summary>
        Razor,
        /// <summary>
        /// Use the Liquid (Fluid) renderer
        /// </summary>
        Fluid,
        /// <summary>
        /// Use the Liquid (Fluid) renderer
        /// </summary>
        Liquid,
        /// <summary>
        /// Use the default Replace renderer
        /// </summary>
        Replace
    }

    /// <summary>
    /// Send using the old SmtpClient (deprecated) or MailKit
    /// </summary>
    public enum SenderType
    {
        /// <summary>
        /// Use the .NET SmtpClient (deprecate)
        /// </summary>
        SmtpClient,
        /// <summary>
        /// Use MailKit
        /// </summary>
        MailKit
    }
}
