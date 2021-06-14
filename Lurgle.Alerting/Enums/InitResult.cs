namespace Lurgle.Alerting
{
    /// <summary>
    ///     Return a reason why initialisation failed
    /// </summary>
    public enum InitResult
    {
        /// <summary>
        ///     Mail host is not configured
        /// </summary>
        MailHostNotConfigured,

        /// <summary>
        ///     Default From address not configured
        /// </summary>
        FromAddressEmpty,

        /// <summary>
        ///     Default To Address not configured
        /// </summary>
        ToAddressEmpty,

        /// <summary>
        ///     Default Debug address not configured
        /// </summary>
        DebugAddressEmpty,

        /// <summary>
        ///     Default subject not configured
        /// </summary>
        SubjectEmpty,

        /// <summary>
        ///     SMTP host test failure
        /// </summary>
        SmtpTestFailed
    }
}