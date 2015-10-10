namespace OfflineMedia.Common.Framework.Logs
{
    public enum LogLevel
    {
        /// <summary>
        /// something relevant to the developper, but which does not impact the user experience
        /// </summary>
        Info = 0,

        /// <summary>
        /// something which has a small impact on the user experience 
        /// </summary>
        Warning = 1,

        /// <summary>
        /// something which has a large impact on a part of the user experience
        /// </summary>
        Error = 2,

        /// <summary>
        /// something which does not allow the application to run as expected
        /// </summary>
        FatalError = 3
    }
}
