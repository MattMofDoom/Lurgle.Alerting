using FluentEmail.Core.Models;

namespace Lurgle.Alerting
{
    /// <summary>
    ///     The priority of the email that will be sent. Abstracts FluentEmail's <see cref="Priority" />  enum so that it does
    ///     not need to be referenced outside of the <see cref="Alert" />  class.
    /// </summary>
    public enum AlertLevel
    {
        /// <summary>
        ///     Normal
        /// </summary>
        Normal = Priority.Normal,

        /// <summary>
        ///     Low
        /// </summary>
        Low = Priority.Low,

        /// <summary>
        ///     High
        /// </summary>
        High = Priority.High
    }
}