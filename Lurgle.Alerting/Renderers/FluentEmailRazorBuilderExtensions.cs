using System;
using FluentEmail.Core.Interfaces;
using Lurgle.Alerting.Renderers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RazorLight.Razor;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     FluentEmail Razor Extensions
    /// </summary>
    public static class FluentEmailRazorBuilderExtensions
    {
        /// <summary>
        ///     Add Razor renderer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder)
        {
            builder.Services.TryAdd(
                ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(_ => new RazorRenderer()));
            return builder;
        }

        /// <summary>
        ///     Add razor renderer with project views and layouts
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="templateRootFolder"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder,
            string templateRootFolder)
        {
            builder.Services.TryAdd(
                ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(
                    _ => new RazorRenderer(templateRootFolder)));
            return builder;
        }

        /// <summary>
        ///     Add razor renderer with embedded views and layouts
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="embeddedResourceRootType"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder,
            Type embeddedResourceRootType)
        {
            builder.Services.TryAdd(
                ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(_ =>
                    new RazorRenderer(embeddedResourceRootType)));
            return builder;
        }

        /// <summary>
        ///     Add razor renderer with a RazorLightProject to support views and layouts
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="razorLightProject"></param>
        /// <returns></returns>
        public static FluentEmailServicesBuilder AddRazorRenderer(this FluentEmailServicesBuilder builder,
            RazorLightProject razorLightProject)
        {
            builder.Services.TryAdd(
                ServiceDescriptor.Singleton<ITemplateRenderer, RazorRenderer>(_ =>
                    new RazorRenderer(razorLightProject)));
            return builder;
        }
    }
}