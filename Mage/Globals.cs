
namespace Mage
{
    /// <summary>
    /// Global, static class for informing threads that they should abort processing
    /// </summary>
    public static class Globals
    {
        // Ignore Spelling: DMS, Mage, Postgres, yyyy-MM-dd

        /// <summary>
        /// Program date
        /// </summary>
        public const string PROGRAM_DATE = "July 19, 2024";

        /// <summary>
        /// Program date, short yyyy-MM-dd format
        /// </summary>
        public const string PROGRAM_DATE_SHORT = "2024-07-19";

        /// <summary>
        /// Default DMS server
        /// </summary>
        public const string DEFAULT_DMS_SERVER = "prismdb2";

        /// <summary>
        /// Default DMS database
        /// </summary>
        public const string DEFAULT_DMS_DATABASE = "dms";


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

        /// <summary>
        /// True if querying a PostgreSQL database
        /// </summary>
        public static bool PostgresDMS { get; set; }

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
            PostgresDMS = false;
        }
    }
}
