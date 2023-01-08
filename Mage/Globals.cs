
namespace Mage
{
    /// <summary>
    /// Global, static class for informing threads that they should abort processing
    /// </summary>
    public static class Globals
    {
        // Ignore Spelling: yyyy-MM-dd

        /// <summary>
        /// Program date
        /// </summary>
        public const string PROGRAM_DATE = "January 7, 2023";

        /// <summary>
        /// Program date, short yyyy-MM-dd format
        /// </summary>
        public const string PROGRAM_DATE_SHORT = "2023-01-07";

        /// <summary>
        /// Default DMS server
        /// </summary>
        public const string DEFAULT_DMS_SERVER = "gigasax";

        /// <summary>
        /// Default DMS database
        /// </summary>
        public const string DEFAULT_DMS_DATABASE = "DMS5";

        /// <summary>
        /// This will be set to true by one of the threads if the user requests that an operation be aborted
        /// </summary>
        public static bool AbortRequested;

        /// <summary>
        /// Server name that has the DMS database
        /// </summary>
        public static string DMSServer
        {
            get => mDmsServer ?? DEFAULT_DMS_SERVER;
            set => mDmsServer = value;
        }

        /// <summary>
        /// DMS database name
        /// </summary>
        public static string DMSDatabase
        {
            get => mDmsDatabase ?? DEFAULT_DMS_DATABASE;
            set => mDmsDatabase = value;
        }

        private static string mDmsServer;
        private static string mDmsDatabase;

        /// <summary>
        /// Constructor
        /// </summary>
        static Globals()
        {
            AbortRequested = false;
            mDmsServer = DEFAULT_DMS_SERVER;
            mDmsDatabase = DEFAULT_DMS_DATABASE;
        }
    }
}
