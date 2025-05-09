﻿
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
        public const string PROGRAM_DATE = "April 22, 2025";

        /// <summary>
        /// Program date, short yyyy-MM-dd format
        /// </summary>
        public const string PROGRAM_DATE_SHORT = "2025-04-22";

        /// <summary>
        /// Default DMS server
        /// </summary>
        public const string DEFAULT_DMS_SERVER = "prismdb2.emsl.pnl.gov";

        /// <summary>
        /// Default DMS database
        /// </summary>
        public const string DEFAULT_DMS_DATABASE = "dms";

        /// <summary>
        /// Default DMS database
        /// </summary>
        public const string DEFAULT_DMS_USER = "dmsreader";

        /// <summary>
        /// Default DMS database
        /// </summary>
        public const string DEFAULT_DMS_USER_PASSWORD = "dms4fun";

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
        /// DMS database user
        /// </summary>
        public static string DMSUser
        {
            get => mDmsUser ?? DEFAULT_DMS_USER;
            set => mDmsUser = value;
        }

        /// <summary>
        /// DMS database user's password
        /// </summary>
        public static string DMSUserPassword
        {
            get => mDmsUserPassword ?? DEFAULT_DMS_USER_PASSWORD;
            set => mDmsUserPassword = value;
        }

        /// <summary>
        /// True if querying a PostgreSQL database
        /// </summary>
        public static bool PostgresDMS { get; set; }

        private static string mDmsServer;
        private static string mDmsDatabase;
        private static string mDmsUser;
        private static string mDmsUserPassword;

        /// <summary>
        /// Constructor
        /// </summary>
        static Globals()
        {
            AbortRequested = false;
            mDmsServer = DEFAULT_DMS_SERVER;
            mDmsDatabase = DEFAULT_DMS_DATABASE;
            mDmsUser = DEFAULT_DMS_USER;
            mDmsUserPassword = DEFAULT_DMS_USER_PASSWORD;
            PostgresDMS = true;
        }
    }
}
