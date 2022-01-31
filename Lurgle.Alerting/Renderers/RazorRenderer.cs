using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using RazorLight;
using RazorLight.Razor;

// ReSharper disable MemberCanBePrivate.Global

namespace Lurgle.Alerting.Renderers
{
    /// <summary>
    ///     Razor renderer
    /// </summary>
    public class RazorRenderer : ITemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        /// <summary>
        ///     Razor renderer
        /// </summary>
        /// <param name="root"></param>
        public RazorRenderer(string root = null)
        {
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(root ?? Directory.GetCurrentDirectory())
                .UseMemoryCachingProvider()
                .Build();
        }

        /// <summary>
        ///     Razor renderer
        /// </summary>
        /// <param name="project"></param>
        public RazorRenderer(RazorLightProject project)
        {
            _engine = new RazorLightEngineBuilder()
                .UseProject(project)
                .UseMemoryCachingProvider()
                .Build();
        }

        /// <summary>
        ///     Razor renderer
        /// </summary>
        /// <param name="embeddedResRootType"></param>
        public RazorRenderer(Type embeddedResRootType)
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(embeddedResRootType)
                .UseMemoryCachingProvider()
                .Build();
        }

        /// <summary>
        ///     Parse Razor template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="isHtml"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {
            dynamic viewBag = (model as IViewBagModel)?.ViewBag;
            return _engine.CompileRenderStringAsync<T>(GetHashString(template), template, model, viewBag);
        }

        string ITemplateRenderer.Parse<T>(string template, T model, bool isHtml)
        {
            return ParseAsync(template, model, isHtml).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Get SHA256 hash
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            var hashbytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (var b in hashbytes) sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}