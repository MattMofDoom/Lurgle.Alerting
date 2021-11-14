using System.Collections.Generic;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable CollectionNeverQueried.Global

namespace Lurgle.Alerting.Classes
{
    /// <summary>
    /// Record of mail send result, including all results for iteration
    /// </summary>
    public class MailResult
    {
        /// <summary>
        /// Overall Success / Failure
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// Last send's delivery type
        /// </summary>
        public DeliveryType DeliveryType { get; set; }

        /// <summary>
        /// Last send's mail host
        /// </summary>
        public string MailHost { get; set; }

        /// <summary>
        /// Last send's ErrorMessages for backward compatibility
        /// </summary>
        public IList<string> ErrorMessages { get; set; }

        /// <summary>
        /// Last send's MessageId for backward compatibility
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// List of all attempts
        /// </summary>
        public List<DeliveryAttempt> DeliveryAttempts { get; set; }
    }
}
