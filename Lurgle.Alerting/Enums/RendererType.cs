namespace Lurgle.Alerting
{
    /// <summary>
    ///     Renderer type to use with email templates
    /// </summary>
    public enum RendererType
    {
        /// <summary>
        ///     Use the Razor renderer
        /// </summary>
        Razor = 0,

        /// <summary>
        ///     Use the Liquid (Fluid) renderer
        /// </summary>
        Liquid = 1,

        /// <summary>
        ///     Use the Liquid (Fluid) renderer
        /// </summary>
        Fluid = 2,

        /// <summary>
        ///     Use the Handlebars renderer
        /// </summary>
        Handlebars = 3,

        /// <summary>
        ///     Use the default Replace renderer
        /// </summary>
        Replace = -1
    }
}