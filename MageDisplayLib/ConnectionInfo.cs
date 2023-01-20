namespace MageDisplayLib
{
    internal class ConnectionInfo
    {
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
        /// True if Server is PostgreSQL
        /// </summary>
        public bool Postgres { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="user"></param>
        public ConnectionInfo(string server, string database, string user)
        {
            Server = server ?? string.Empty;
            Database = database ?? string.Empty;
            User = user ?? string.Empty;
            Postgres = false;
        }
    }
}
