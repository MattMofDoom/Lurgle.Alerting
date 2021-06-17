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
        Razor,

        /// <summary>
        ///     Use the Liquid (Fluid) renderer
        /// </summary>
        Fluid,

        /// <summary>
        ///     Use the Liquid (Fluid) renderer
        /// </summary>
        Liquid,

        /// <summary>
        ///     Use the Handlebars renderer
        /// </summary>
        Handlebars,

        /// <summary>
        ///     Use the default Replace renderer
        /// </summary>
        Replace
    }
}