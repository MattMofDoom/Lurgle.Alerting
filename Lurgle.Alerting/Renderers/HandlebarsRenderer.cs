using System;
using System.IO;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using HandlebarsDotNet;

namespace Lurgle.Alerting.Renderers
{
    /// <summary>
    ///     Handlebars Renderer, based off https://github.com/matthewturner/FluentEmail.Handlebars
    /// </summary>
    public class HandlebarsRenderer : ITemplateRenderer
    {
        private readonly IHandlebars engine;

        /// <summary>
        ///     Handlebars Renderer constructor
        /// </summary>
        public HandlebarsRenderer()
        {
            engine = Handlebars.Create();
        }

        /// <summary>
        ///     Handlebars Renderer constructor
        /// </summary>
        /// <param name="templateRoot"></param>
        public HandlebarsRenderer(string templateRoot)
            : this()
        {
            RegisterTemplatesFrom(templateRoot);
        }

        /// <summary>
        ///     Async parse implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public async Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {
            var compiledTemplate = engine.Compile(template);

            var result = compiledTemplate(model);

            return await Task.FromResult(result);
        }

        /// <summary>
        ///     Parse implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        string ITemplateRenderer.Parse<T>(string template, T model, bool isHtml)
        {
            return ParseAsync(template, model, isHtml).GetAwaiter().GetResult();
        }

        /// <summary>
        ///     Register template implementation
        /// </summary>
        /// <param name="templateRoot"></param>
        private void RegisterTemplatesFrom(string templateRoot)
        {
            if (!Directory.Exists(templateRoot))
                throw new ArgumentException($"The templateRoot directory {templateRoot} does not exist");

            var templates = Directory.GetFiles(templateRoot, "*.html.hbs");

            foreach (var template in templates)
            {
                var name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(template)).ToLower();
                var content = File.ReadAllText(template);
                engine.RegisterTemplate(name, content);
            }
        }
    }
}