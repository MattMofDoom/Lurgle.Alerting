// ReSharper disable InconsistentNaming

namespace Lurgle.Alerting
{
    /// <summary>
    ///     Send using the old SmtpClient (deprecated) or MailKit
    /// </summary>
    public enum SenderType
    {
        /// <summary>
        ///     Use the .NET SmtpClient (deprecate)
        /// </summary>
        SmtpClient,

        /// <summary>
        ///     Use MailKit
        /// </summary>
        MailKit
    }
}