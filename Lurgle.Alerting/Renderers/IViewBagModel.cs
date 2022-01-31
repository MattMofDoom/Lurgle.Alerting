using System.Dynamic;

namespace Lurgle.Alerting.Renderers
{
    /// <summary>
    ///     Viewbag model
    /// </summary>
    public interface IViewBagModel
    {
        /// <summary>
        ///     Viewbag
        /// </summary>
        ExpandoObject ViewBag { get; }
    }
}