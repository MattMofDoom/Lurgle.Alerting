// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo
namespace Lurgle.Alerting
{
    /// <summary>
    /// Type of mail delivery
    /// </summary>
    public enum DeliveryType
    {
        /// <summary>
        /// Delivery via mailhost
        /// </summary>
        MailHost,
        /// <summary>
        /// Delivery via mailhost fallback
        /// </summary>
        MailFallback,
        /// <summary>
        /// Delivery via DNS
        /// </summary>
        Dns,
        /// <summary>
        /// Delivery via DNS fallback
        /// </summary>
        DnsFallback,
        /// <summary>
        /// Delivery via Mailhost DNS fallback
        /// </summary>
        HostDnsFallback,
        /// <summary>
        /// N/A
        /// </summary>
        None = -1
    }
}
