namespace Lurgle.Alerting
{
    /// <summary>
    ///     Return a reason why initialisation failed
    /// </summary>
    public enum InitResult
    {
        /// <summary>
        ///     Successful initialisation
        /// </summary>
        Success,

        /// <summary>
        ///     SMTP host test failure
        /// </summary>
        SmtpTestFailed
    }
}