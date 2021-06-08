namespace Lurgle.Alerting
{
    /// <summary>
    ///     Indicate whether an email address passing in to <see cref="Alert" /> is an actual email address, or if it should be
    ///     queried from the config
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        ///     Email address
        /// </summary>
        Email = 0,

        /// <summary>
        ///     Read from config
        /// </summary>
        FromConfig = 1
    }
}