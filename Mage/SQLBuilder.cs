using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Mage
{

    /// <summary>
    /// Constructs SQL query from templates and run-time parameters
    /// </summary>
    public class SQLBuilder
    {

        /// <summary>
        /// Name in mPredicates of the key that specifies whether the server is a PostgreSQL server
        /// </summary>
        /// <remarks>This constant's value must match the property named IsPostgres in the SQLReader class</remarks>
        public const string IS_POSTGRES_KEY = "IsPostgres";

        /// <summary>
        /// Name in mPredicates of the key that specifies the server
        /// </summary>
        /// <remarks>This constant's value must match the property named Server in the SQLReader class</remarks>
        public const string SERVER_NAME_KEY = "Server";

        /// <summary>
        /// Name in mPredicates of the key that specifies the database
        /// </summary>
        /// <remarks>This constant's value must match the property named Database in the SQLReader class</remarks>
        public const string DATABASE_NAME_KEY = "Database";

        /// <summary>
        /// Name in mPredicates of the key that specifies the database user
        /// </summary>
        /// <remarks>This constant's value must match the property named Username in the SQLReader class</remarks>
        public const string USERNAME_KEY = "Username";

        #region Member Variables

        private readonly List<QueryPredicate> mPredicates = new List<QueryPredicate>();

        private readonly Dictionary<string, QueryPredicate> mDefaultPredicates = new Dictionary<string, QueryPredicate>();

        private readonly List<QuerySort> mSortingItems = new List<QuerySort>();

        #endregion

        #region Properties

        /// <summary>
        /// Database table for SQL query
        /// </summary>
        public string Table { set; get; }

        /// <summary>
        /// List of columns for SQL query
        /// </summary>
        public string Columns { set; get; }

        /// <summary>
        /// True if columns with spaces should be quoted with double quotes instead of square brackets
        /// </summary>
        public bool IsPostgres { get; set; }

        /// <summary>
        /// Format of final SQL query:
        /// - column_data_only
        /// - count_only
        /// - filtered_and_sorted
        /// - filtered_only
        /// </summary>
        public string QueryType { get; set; }

        /// <summary>
        /// Name of stored procedure to call for query
        /// (must be blank if not applicable)
        /// </summary>
        public string SprocName { get; set; }

        /// <summary>
        /// SpecialArgs are used to pass module properties in the collection of query parameters
        /// </summary>
        public Dictionary<string, string> SpecialArgs { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Get list of stored procedure arguments
        /// </summary>
        public Dictionary<string, string> SprocParameters { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Are there any predicate clauses?
        /// </summary>
        public bool HasPredicate => (mPredicates.Count > 0);

        #endregion

        #region Private Classes

        /// <summary>
        /// A query predicate item
        /// </summary>
        protected class QueryPredicate
        {

            /// <summary>
            /// Relationship with other predicate items ("AND" or "OR")
            /// </summary>
            public string rel { get; set; }

            /// <summary>
            /// Column
            /// </summary>
            public string col { get; set; }

            /// <summary>
            /// Comparison
            /// </summary>
            public string cmp { get; set; }


            /// <summary>
            /// Value to compare
            /// </summary>
            public string val { get; set; }

            /// <summary>
            /// Construct a basic query predicate item
            /// </summary>
            public QueryPredicate()
            {
                rel = "AND";
            }
        }

        /// <summary>
        /// List of sort column/direction pairs
        /// </summary>
        protected class QuerySort
        {

            /// <summary>
            /// Sorting column
            /// </summary>
            public string col { get; set; }

            /// <summary>
            /// Sorting direction
            /// </summary>
            public string dir { get; set; }

            /// <summary>
            /// Construct new QuerySort object
            /// </summary>
            public QuerySort()
            {
                col = string.Empty;
                dir = "ASC";
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Construct new Mage SQLBuilder object
        /// </summary>
        public SQLBuilder()
        {
            SprocName = string.Empty;
            Table = string.Empty;
            Columns = "*";
            QueryType = "filtered_and_sorted";
        }

        /// <summary>
        /// Construct new Mage SQLBuilder object
        /// and build SQL query from given xml template and runtime args
        /// </summary>
        /// <param name="xml">XML template with specifications for the query</param>
        /// <param name="args">Key/Value parameter that will be mixed into query</param>
        public SQLBuilder(string xml, ref Dictionary<string, string> args)
        {
            QueryType = "filtered_and_sorted";
            InitializeFromXML(xml, ref args);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Populate this object from query in xml specification
        /// and any parameters passed in via args
        /// </summary>
        /// <param name="xml">Specifications for query</param>
        /// <param name="args">Key/Value parameter that will be mixed into query</param>
        public void InitializeFromXML(string xml, ref Dictionary<string, string> args)
        {
            Columns = "*";
            SprocName = string.Empty;
            Table = string.Empty;
            SpecialArgs.Clear();
            SprocParameters.Clear();

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var queryNode = doc.SelectSingleNode(".//query");
            if (queryNode == null)
                return;

            // Step through all item nodes in query
            foreach (XmlNode itemNode in queryNode.ChildNodes)
            {
                if (itemNode.Attributes == null)
                    continue;

                switch (itemNode.Name)
                {
                    case "connection":
                        SpecialArgs[SERVER_NAME_KEY] = GetAttributeValue(itemNode, "server", string.Empty);
                        SpecialArgs[DATABASE_NAME_KEY] = GetAttributeValue(itemNode, "database", string.Empty);
                        SpecialArgs[USERNAME_KEY] = GetAttributeValue(itemNode, "user", string.Empty);
                        SpecialArgs[IS_POSTGRES_KEY] = GetAttributeValue(itemNode, "postgres", "false");
                        break;

                    case "table":
                        Table = itemNode.Attributes["name"].InnerText;
                        Columns = itemNode.Attributes["cols"].InnerText;
                        break;

                    case "predicate":
                        var rel = itemNode.Attributes["rel"].InnerText;
                        var predicateColumn = itemNode.Attributes["col"].InnerText;
                        var cmp = itemNode.Attributes["cmp"].InnerText;
                        var val = itemNode.Attributes["val"].InnerText;
                        SetColumnDefaultPredicate(rel, predicateColumn, cmp, val);
                        break;

                    case "sort":
                        var sortColumn = itemNode.Attributes["col"].InnerText;
                        var dir = itemNode.Attributes["dir"].InnerText;
                        AddSortingItem(sortColumn, dir);
                        break;

                    case "sproc":
                        SpecialArgs["SprocName"] = itemNode.Attributes["name"].InnerText;
                        SprocName = itemNode.Attributes["name"].InnerText;
                        break;

                    case "param":
                        var key = itemNode.Attributes["name"].InnerText;
                        if (itemNode.Attributes["value"] != null)
                        {
                            var value = itemNode.Attributes["value"].InnerText;
                            SprocParameters[key] = value;
                        }
                        break;

                }
            }

            // Find any special runtime arguments (name is marked by prefix)
            // Strip the prefix, add to specialArgs, and remove from runtime arguments list
            var tempArgs = new Dictionary<string, string>(args);
            foreach (var arg in tempArgs)
            {
                if (arg.Key.StartsWith(":"))
                { // Special runtime parameters have prefix
                    var key = arg.Key.Substring(1, arg.Key.Length - 1);
                    SpecialArgs[key] = arg.Value;
                    args.Remove(arg.Key);
                }
            }

            // If this is straight query, apply args to predicate
            if (string.IsNullOrEmpty(SprocName) && args != null)
            {
                foreach (var arg in args)
                {
                    if (!string.IsNullOrEmpty(arg.Value))
                    {
                        AddPredicateItem(arg.Key, arg.Value);
                    }
                }
            }

            if (string.IsNullOrEmpty(SprocName) || args == null)
                return;

            // Working with a stored procedure query
            // Add the parameters and the value for each
            foreach (var arg in args)
            {
                SprocParameters[arg.Key] = arg.Value;
            }

        }

        #endregion

        #region Client functions

        /// <summary>
        /// Return a dictionary object containing the description text for query components
        /// </summary>
        /// <param name="xml">Specifications for query</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDescriptionsFromXML(string xml)
        {
            var descriptions = new Dictionary<string, string>();

            var doc = new System.Xml.XmlDocument();
            doc.LoadXml(xml);

            var queryNode = doc.SelectSingleNode(".//query");
            if (queryNode == null)
                return descriptions;

            // Step through all item nodes in query
            foreach (System.Xml.XmlNode itemNode in queryNode.ChildNodes)
            {
                switch (itemNode.Name)
                {
                    case "description":
                        descriptions[":Description:"] = itemNode.InnerText;
                        break;

                    case "param":
                        if (itemNode.Attributes != null)
                        {
                            var name = itemNode.Attributes["name"].InnerText;
                            descriptions[name] = itemNode.InnerText;
                        }
                        break;

                    case "predicate":
                        if (itemNode.Attributes != null)
                        {
                            var col = itemNode.Attributes["col"].InnerText;
                            descriptions[col] = itemNode.InnerText;
                        }
                        break;

                }
            }
            return descriptions;
        }


        /// <summary>
        /// Set default values to use when adding predicate clause for a column
        /// </summary>
        /// <param name="rel">Relationship ("AND" or "OR")</param>
        /// <param name="col">Column name</param>
        /// <param name="cmp">Comparision type</param>
        /// <param name="val">Comparision value</param>
        public void SetColumnDefaultPredicate(string rel, string col, string cmp, string val)
        {
            var p = new QueryPredicate
            {
                rel = rel,
                col = col,
                cmp = cmp,
                val = val
            };
            mDefaultPredicates[col] = p;
        }

        /// <summary>
        /// Add an item for building the query predicate
        /// default predicate items for column
        /// </summary>
        /// <param name="col">Column name</param>
        /// <param name="val">Comparision value</param>
        public void AddPredicateItem(string col, string val)
        {
            if (mDefaultPredicates.ContainsKey(col))
            {
                var p = mDefaultPredicates[col];
                val = val ?? p.val;
                AddPredicateItem(p.rel, p.col, p.cmp, val);
            }
            else if (val != null)
            {
                AddPredicateItem("AND", col, "ContainsText", val);
            }
        }

        /// <summary>
        /// Add an item for building the query predicate (with automatic "AND" relationship)
        /// </summary>
        /// <param name="col">Column name</param>
        /// <param name="cmp">Comparision type</param>
        /// <param name="val">Comparision value</param>
        public void AddPredicateItem(string col, string cmp, string val)
        {
            AddPredicateItem("AND", col, cmp, val);
        }

        /// <summary>
        /// Add an item for building the query predicate ('WHERE' clause)
        /// </summary>
        /// <param name="rel">Relationship ("AND" or "OR")</param>
        /// <param name="col">Column name</param>
        /// <param name="cmp">Comparision type</param>
        /// <param name="val">Comparision value</param>
        public void AddPredicateItem(string rel, string col, string cmp, string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                // (someday) reject if any field empty, not just the value field

                //  ConvertWildcards(ref cmp, ref val);
                var p = new QueryPredicate
                {
                    rel = rel,
                    col = col,
                    cmp = cmp,
                    val = val
                };
                mPredicates.Add(p);
            }
        }

        /// <summary>
        /// Add item to be used for building the 'Order by' clause
        /// </summary>
        /// <param name="col">Sort column name</param>
        /// <param name="dir">Sort direction ("ASC"/"DESC")</param>
        public void AddSortingItem(string col, string dir)
        {
            if (string.IsNullOrEmpty(col))
            {
                return;
            }

            var sorting = new QuerySort
            {
                col = col
            };
            var d = dir.ToUpper();
            sorting.dir = (d == "DESC") ? d : "ASC";
            mSortingItems.Add(sorting);
        }

        /// <summary>
        /// Wildcards and special characters:
        /// the presence of regex/glob style wildcard characters
        /// will cause the defined column comparison to be
        /// overridden by a 'LIKE' operator, with substitution of
        /// SQL wildcards for regex/glob.
        /// a leading tilde will force an exact match
        /// </summary>
        /// <param name="cmp"></param>
        /// <param name="val"></param>
        protected static void ConvertWildcards(ref string cmp, ref string val)
        {
            // Look for wildcard characters

            var exact_match = (val.Substring(0, 1) == "~");
            var regex_all = val.Contains("*");
            var regex_one = val.Contains("?");
            var sql_any = val.Contains("%");

            // Force exact match
            if (exact_match)
            {
                val = val.Replace("~", "");
                cmp = "MatchesText";
            }
            else
                if (regex_all || regex_one)
            {
                cmp = "wildcards";
            }
            else
            {
                var exceptions = new[] { "MatchesText", "MTx", "MatchesTextOrBlank", "MTxOB" };
                if (!sql_any && !exceptions.Contains(cmp))
                {
                    // Quote underscores in the absence of '%' or regex/glob wildcards
                    val = val.Replace("_", "[_]");
                }
            }
        }

        /// <summary>
        /// Build mssql T-SQL SQL query from component parts
        /// </summary>
        /// <returns>SQL string</returns>
        public string BuildQuerySQL()
        {

            // Process the predicate list
            var p_and = new List<string>();
            var p_or = new List<string>();

            foreach (var predicate in mPredicates)
            {
                var sWhereItem = MakeWhereItem(predicate, IsPostgres);
                if (!string.IsNullOrEmpty(sWhereItem))
                {
                    switch (predicate.rel.ToLower())
                    {
                        case "and":
                            p_and.Add(sWhereItem);
                            break;

                        case "or":
                            p_or.Add(sWhereItem);
                            break;

                    }
                }
            }

            // Build guts of query
            var baseSql = new StringBuilder();
            baseSql.Append(" FROM " + PossiblyQuoteName(Table, IsPostgres));

            // Collect all 'or' clauses as one grouped item and put it into the 'and' item array
            if (p_or.Count > 0)
            {
                p_and.Add("(" + string.Join(" OR ", p_or) + ")");
            }
            //
            // 'and' all predicate clauses together
            var andPredicates = string.Join(" AND ", p_and);
            if (!string.IsNullOrEmpty(andPredicates))
            {
                baseSql.Append(" WHERE " + andPredicates);
            }

            // columns to display
            var display_cols = Columns;

            // Construct final query according to its intended use
            var sql = new StringBuilder();
            switch (QueryType)
            {
                case "count_only": // Query for returning count of total rows
                    sql.Append("SELECT COUNT(*) AS NumRows");
                    sql.Append(baseSql);
                    break;

                case "column_data_only":
                    sql.Append("SELECT TOP(1)" + display_cols);
                    sql.Append(baseSql);
                    break;

                case "filtered_only":
                    sql.Append("SELECT " + display_cols);
                    sql.Append(baseSql);
                    break;

                case "filtered_and_sorted": // (not paged)
                    sql.Append("SELECT " + display_cols);
                    sql.Append(baseSql);
                    var orderBy = MakeOrderBy(mSortingItems, IsPostgres);
                    sql.Append(!string.IsNullOrEmpty(orderBy) ? string.Format(" ORDER BY {0}", orderBy) : "");
                    break;

            }
            return sql.ToString();
        }

        #endregion

        #region Private Functions

        private string GetAttributeValue(XmlNode itemNode, string attributeName, string valueIfMissing)
        {
            if (itemNode.Attributes == null)
                return valueIfMissing;

            var attribute = itemNode.Attributes[attributeName];

            if (attribute?.InnerText == null)
                return valueIfMissing;

            return attribute.InnerText;
        }

        /// <summary>
        /// Build "order by" clause
        /// </summary>
        /// <param name="sortItems"></param>
        /// <param name="isPostgres"></param>
        /// <returns>SQL text for sorting</returns>
        private static string MakeOrderBy(IEnumerable<QuerySort> sortItems, bool isPostgres)
        {
            var a = new List<string>();
            foreach (var item in sortItems)
            {
                a.Add(string.Format("{0} {1}", PossiblyQuoteName(item.col, isPostgres), item.dir));
            }
            return string.Join(", ", a);
        }

        /// <summary>
        /// Generate sql predicate string from predicate specification object
        /// (column name, comparison operator, comparison value)
        /// </summary>
        /// <param name="predicate">List of predicate items</param>
        /// <param name="isPostgres"></param>
        /// <returns>SQL text for predicate</returns>
        private static string MakeWhereItem(QueryPredicate predicate, bool isPostgres)
        {
            var col = PossiblyQuoteName(predicate.col, isPostgres);
            var cmp = predicate.cmp;
            var val = predicate.val;

            var str = string.Empty;
            switch (cmp)
            {
                case "wildcards":
                    val = val.Replace("_", "[_]");
                    val = val.Replace("*", "%");
                    val = val.Replace("?", "_");
                    str += string.Format("{0} LIKE '{1}'", col, val);
                    break;

                case "ContainsText":
                case "CTx":
                    val = (val.Substring(0, 1) == "`") ? val.Replace("`", "") + "%" : "%" + val + "%";
                    str += string.Format("{0} LIKE '{1}'", col, val);
                    break;


                case "DoesNotContainText":
                case "DNCTx":
                    val = (val.Substring(0, 1) == "`") ? val.Replace("`", "") + "%" : "%" + val + "%";
                    str += string.Format("NOT {0} LIKE '{1}'", col, val);
                    break;

                case "MatchesText":
                case "MTx":
                    str += string.Format("{0} = '{1}'", col, val);
                    break;

                case "StartsWithText":
                case "SWTx":
                    val = val + "%";
                    str += string.Format("{0} LIKE '{1}'", col, val);
                    break;

                case "Equals":
                case "EQn":
                    if (double.TryParse(val, out _))
                    {
                        str += string.Format("{0} = {1}", col, val);
                    }
                    else
                    {
                        str += string.Format("{0} = '{1}'", col, val);
                    }
                    break;

                case "NotEqual":
                case "NEn":
                    if (double.TryParse(val, out _))
                    {
                        str += string.Format("NOT {0} = {1}", col, val);
                    }
                    else
                    {
                        str += string.Format("NOT {0} = '{1}'", col, val);
                    }
                    break;

                case "GreaterThan":
                case "GTn":
                    if (double.TryParse(val, out _))
                    {
                        str += string.Format("{0} > {1}", col, val);
                    }
                    break;

                case "LessThan":
                case "LTn":
                    if (double.TryParse(val, out _))
                    {
                        str += string.Format("{0} < {1}", col, val);
                    }
                    break;

                case "LessThanOrEqualTo":
                case "LTOEn":
                    if (double.TryParse(val, out _))
                    {
                        str += string.Format("{0} <= {1}", col, val);
                    }
                    break;

                case "GreaterThanOrEqualTo":
                case "GTOEn":
                    if (double.TryParse(val, out _))
                    {
                        str += string.Format("{0} >= {1}", col, val);
                    }
                    break;

                case "InList":
                    str += string.Format(" {0} IN ({1}) ", col, val);
                    break;

                case "InListText":
                    str += string.Format(" {0} IN ({1}) ", col, QuoteList(val));
                    break;

                case "MatchesTextOrBlank":
                case "MTxOB":
                    str += string.Format("{0} = '{1}' OR {0} = ''", col, val);
                    break;

                case "LaterThan":
                case "LTd":
                    str += string.Format("{0} > '{1}'", col, val);
                    break;

                case "EarlierThan":
                case "ETd":
                    str += string.Format("{0} < '{1}'", col, val);
                    break;

                case "MostRecentWeeks":
                case "MRWd":
                    // ReSharper disable StringLiteralTypo
                    str += string.Format(" {0} > DATEADD(Week, -{1}, GETDATE()) ", col, val);
                    // ReSharper restore StringLiteralTypo
                    break;

                default:
                    str += " unrecognized";
                    break;

            }
            return str;
        }

        private static string PossiblyQuoteName(string objectName, bool isPostgres)
        {
            var charsToQuote = new[] {' ', '%'};

            if (objectName.StartsWith("[") && objectName.EndsWith("]") && isPostgres)
            {
                return "\"" + objectName.Substring(1, objectName.Length - 2) + "\"";
            }

            if (objectName.StartsWith("\"") && objectName.EndsWith("\"") && !isPostgres)
            {
                return "[" + objectName.Substring(1, objectName.Length - 2) + "]";
            }

            if (objectName.IndexOfAny(charsToQuote) >= 0)
            {
                if (isPostgres)
                    return "\"" + objectName + "\"";

                return "[" + objectName + "]";
            }

            return objectName;
        }

        /// <summary>
        /// Adds single quotes to a comma separated list of values
        /// </summary>
        /// <param name="valList"></param>
        /// <returns></returns>
        private static string QuoteList(string valList)
        {
            var values = valList.Split(',');
            var quotedList = new StringBuilder();

            foreach (var value in values)
            {
                if (quotedList.Length > 0)
                    quotedList.Append(",");

                quotedList.Append("'" + value.Trim() + "'");
            }

            return quotedList.ToString();
        }

        #endregion

    }
}
