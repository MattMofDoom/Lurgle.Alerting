using FluentEmail.Core.Models;

namespace Lurgle.Alerting
{
    /// <summary>
    /// Indicate whether an email address passing in to <see cref="Alerting.Alert"/> is an actual email address, or if it should be queried from the config
    /// </summary>
    public enum AddressType
    {
        Email = 0,
        FromConfig = 1
    }

    /// <summary>
    /// The priority of the email that will be sent. Abstracts FluentEmail's <see cref="Priority"/>  enum so that it does not need to be referenced outside of the <see cref="Alert"/>  class.
    /// </summary>
    public enum AlertLevel
    {
        Normal = Priority.Normal,
        Low = Priority.Low,
        High = Priority.High
    }

    /// <summary>
    /// Renderer type to use with email templates
    /// </summary>
    public enum RendererType
    {
        Razor,
        Fluid,
        Replace
    }
}
