namespace MageDisplayLib
{
    internal class ConnectionInfo
    {
        // Ignore Spelling: Mage, Postgres

        /// <summary>
        /// Server name
        /// </summary>
        public string Server { get; }

        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Database user (empty string if using integrated authentication)
        /// </summary>
        public string User { get; }

        /// <summary>
        /// Database user's password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// True if Server is PostgreSQL
        /// </summary>
        public bool Postgres { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public ConnectionInfo(string server, string database, string user, string password)
        {
            Server = server ?? string.Empty;
            Database = database ?? string.Empty;
            User = user ?? string.Empty;
            Password = password ?? string.Empty;
            Postgres = false;
        }
    }
}
