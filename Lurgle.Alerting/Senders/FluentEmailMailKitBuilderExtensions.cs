using FluentEmail.Core.Interfaces;
using Lurgle.Alerting.Senders;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     FluentEmail MailKit extensions
    /// </summary>
    public static class FluentEmailMailKitBuilderExtensions
    {
        /// <summary>
        ///     MailKit sender
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="smtpClientOptions"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddMailKitSender(this FluentEmailServicesBuilder builder,
            SmtpClientOptions smtpClientOptions)
        {
            builder.Services.TryAdd(ServiceDescriptor.Scoped<ISender>(_ => new MailKitSender(smtpClientOptions)));
            return builder;
        }
    }
}