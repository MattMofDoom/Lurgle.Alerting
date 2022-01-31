using System.Collections.Generic;
using System.Threading.Tasks;
using RazorLight.Razor;

// ReSharper disable UnusedType.Global

namespace Lurgle.Alerting.Renderers
{
    /// <summary>
    ///     In memory RazorLight project
    /// </summary>
    public class InMemoryRazorLightProject : RazorLightProject
    {
        /// <summary>
        ///     Get template key
        /// </summary>
        /// <param name="templateKey"></param>
        /// <returns></returns>
        public override Task<RazorLightProjectItem> GetItemAsync(string templateKey)
        {
            return Task.FromResult<RazorLightProjectItem>(new TextSourceRazorProjectItem(templateKey, templateKey));
        }

        /// <summary>
        ///     Get imports
        /// </summary>
        /// <param name="templateKey"></param>
        /// <returns></returns>
        public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
        {
            return Task.FromResult<IEnumerable<RazorLightProjectItem>>(new List<RazorLightProjectItem>());
        }
    }
}