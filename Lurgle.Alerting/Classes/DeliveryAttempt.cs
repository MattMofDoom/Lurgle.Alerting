using FluentEmail.Core.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Lurgle.Alerting.Classes
{
    /// <summary>
    /// Delivery Attempt record
    /// </summary>
    public class DeliveryAttempt
    {
        /// <summary>
        /// Type of delivery
        /// </summary>
        public DeliveryType DeliveryType { get; set; }
        /// <summary>
        /// Host attempted
        /// </summary>
        public string MailHost { get; set; }
        /// <summary>
        /// Send response
        /// </summary>
        public SendResponse Result { get; set; }
    }
}
