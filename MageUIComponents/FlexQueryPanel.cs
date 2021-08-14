using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Mage;

namespace MageUIComponents
{
    public partial class FlexQueryPanel : UserControl, IModuleParameters
    {
        public event EventHandler<MageCommandEventArgs> OnAction;

        private readonly List<FlexQueryItemPanel> QueryItemPanels = new();

        /// <summary>
        /// Constructor
        /// </summary>
        public FlexQueryPanel()
        {
            InitializeComponent();
            QueryItemPanels.Add(flexQueryItemPanel1);
            QueryItemPanels.Add(flexQueryItemPanel2);
            QueryItemPanels.Add(flexQueryItemPanel3);
            QueryItemPanels.Add(flexQueryItemPanel4);
        }

        public Dictionary<string, string> GetParameters()
        {
            return new()
            {
                { "QueryItem0",   EncodeQueryItem(QueryItemPanels[0])},
                { "QueryItem1",   EncodeQueryItem(QueryItemPanels[1])},
                { "QueryItem2",   EncodeQueryItem(QueryItemPanels[2])},
                { "QueryItem3",   EncodeQueryItem(QueryItemPanels[3])},
            };
        }

        public void SetParameters(Dictionary<string, string> paramList)
        {
            foreach (var paramDef in paramList)
            {
                switch (paramDef.Key)
                {
                    case "QueryItem0":
                        DecodeQueryItem(paramDef.Value, QueryItemPanels[0]);
                        break;
                    case "QueryItem1":
                        DecodeQueryItem(paramDef.Value, QueryItemPanels[1]);
                        break;
                    case "QueryItem2":
                        DecodeQueryItem(paramDef.Value, QueryItemPanels[2]);
                        break;
                    case "QueryItem3":
                        DecodeQueryItem(paramDef.Value, QueryItemPanels[3]);
                        break;
                }
            }
        }

        public string QueryName { get; set; }

        public Collection<string> QueryItems
        {
            get
            {
                var items = new Collection<string>();
                foreach (var pnl in QueryItemPanels)
                {
                    if (!string.IsNullOrWhiteSpace(pnl.Value))
                    {
                        var item = EncodeQueryItem(pnl);
                        items.Add(item);
                    }
                }
                return items;
            }
        }

        public void SetComparisionPickList(string[] items)
        {
            foreach (var pnl in QueryItemPanels)
            {
                pnl.SetComparisionPickList(items);
            }
        }

        public void SetColumnPickList(string[] items)
        {
            foreach (var pnl in QueryItemPanels)
            {
                pnl.SetColumnPickList(items);
            }
        }

        // Utility methods

        private static string EncodeQueryItem(FlexQueryItemPanel pnl)
        {
            return string.Format("{0}|{1}|{2}|{3}", pnl.Relation, pnl.Column, pnl.Comparision, pnl.Value);
        }

        private static void DecodeQueryItem(string item, FlexQueryItemPanel pnl)
        {
            var fields = item.Split('|');
            pnl.Relation = fields[0];
            pnl.Column = fields[1];
            pnl.Comparision = fields[2];
            pnl.Value = fields[3];
        }

        private void GetJobsCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_flex_query", QueryName));
        }

        public SQLBuilder GetSQLBuilder(string queryTemplate)
        {
            var args = new Dictionary<string, string>();
            var builder = new SQLBuilder(queryTemplate, ref args);
            foreach (var item in QueryItems.ToArray())
            {
                var fields = item.Split('|');
                if (!string.IsNullOrEmpty(fields[0]) && !string.IsNullOrEmpty(fields[1]) && !string.IsNullOrEmpty(fields[2]) && !string.IsNullOrEmpty(fields[3]))
                {
                    builder.AddPredicateItem(fields[0], fields[1], fields[2], fields[3]);
                }
            }
            return builder;
        }
        /*
                /// <summary>
                /// Build and setup an MSSQL reader from flex query panel
                /// </summary>
                /// <param name="queryTemplate"></param>
                /// <param name="queryItems"></param>
                /// <returns></returns>
                public static SQLReader GetMSSQLReaderFromFlexQuery(string queryTemplate, string[] queryItems) {
                    Dictionary<string, string> arguments = new Dictionary<string, string>();
                    SQLBuilder builder = new SQLBuilder(queryTemplate, ref arguments);
                    for each (string item in queryItems) {
                        string[] fields = item.Split('|');
                        if (!string.IsNullOrEmpty(fields[0]) && !string.IsNullOrEmpty(fields[1]) && !string.IsNullOrEmpty(fields[2]) && !string.IsNullOrEmpty(fields[3])) {
                            builder.AddPredicateItem(fields[0], fields[1], fields[2], fields[3]);
                        }
                    }
                    SQLReader reader = new SQLReader();
                    reader.Server = builder.SpecialArgs[SQLBuilder.SERVER_NAME_KEY];
                    reader.Database = builder.SpecialArgs[SQLBuilder.DATABASE_NAME_KEY];
                    reader.SQLText = builder.BuildQuerySQL();
                    return reader;
                }
        */
    }
}
