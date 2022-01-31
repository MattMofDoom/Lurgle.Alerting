using MailKit.Security;

// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Lurgle.Alerting.Senders
{
    /// <summary>
    ///     SMTP Client Options
    /// </summary>
    public class SmtpClientOptions
    {
        /// <summary>
        ///     Server
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        ///     Port
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        ///     Username
        /// </summary>
        public string User { get; set; } = string.Empty;

        /// <summary>
        ///     Password
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        ///     UseSsl (SocketOptions is preferable)
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        ///     Use authentication
        /// </summary>
        public bool RequiresAuthentication { get; set; }

        /// <summary>
        ///     Preferred Encoding
        /// </summary>
        public string PreferredEncoding { get; set; } = string.Empty;

        /// <summary>
        ///     Enable pickup directory
        /// </summary>
        public bool UsePickupDirectory { get; set; } = false;

        /// <summary>
        ///     Mail pickup directory
        /// </summary>
        public string MailPickupDirectory { get; set; } = string.Empty;

        /// <summary>
        ///     Secure socket options
        /// </summary>
        public SecureSocketOptions? SocketOptions { get; set; }
    }
}