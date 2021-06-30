using FluentEmail.Core.Interfaces;
using Lurgle.Alerting.Renderers;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Handlebars builder extensions, based off https://github.com/matthewturner/FluentEmail.Handlebars
    /// </summary>
    public static class FluentEmailHandlebarsBuilderExtensions
    {
        /// <summary>
        ///     Add Handlebars renderer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddHandlebarsRenderer(this FluentEmailServicesBuilder builder)
        {
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ITemplateRenderer, HandlebarsRenderer>());
            return builder;
        }

        /// <summary>
        ///     Automatically loads templates from the specified directory
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="templateRoot"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddHandlebarsRenderer(this FluentEmailServicesBuilder builder,
            string templateRoot)
        {
            builder.Services.TryAdd(
                ServiceDescriptor.Singleton<ITemplateRenderer, HandlebarsRenderer>(s =>
                    new HandlebarsRenderer(templateRoot)));
            return builder;
        }
    }
}