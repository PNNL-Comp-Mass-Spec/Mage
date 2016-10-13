using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Mage;

namespace MageUIComponents {

    public partial class FlexQueryPanel : UserControl, IModuleParameters {

        public event EventHandler<MageCommandEventArgs> OnAction;

        #region Member Variables

        private readonly List<FlexQueryItemPanel> QueryItemPanels = new List<FlexQueryItemPanel>();

        #endregion

        #region Constructiors

        public FlexQueryPanel() {
            InitializeComponent();
            QueryItemPanels.Add(flexQueryItemPanel1);
            QueryItemPanels.Add(flexQueryItemPanel2);
            QueryItemPanels.Add(flexQueryItemPanel3);
            QueryItemPanels.Add(flexQueryItemPanel4);

        }

        #endregion

        #region IModuleParameters Members

        public Dictionary<string, string> GetParameters() {
            return new Dictionary<string, string>() { 
                { "QueryItem0",   EncodeQueryItem(QueryItemPanels[0])},
                { "QueryItem1",   EncodeQueryItem(QueryItemPanels[1])},
                { "QueryItem2",   EncodeQueryItem(QueryItemPanels[2])},
                { "QueryItem3",   EncodeQueryItem(QueryItemPanels[3])},
            };
        }

        public void SetParameters(Dictionary<string, string> paramList) {
            foreach (var paramDef in paramList) {
                switch (paramDef.Key) {
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

        #endregion

        #region Properties

        public string QueryName { get; set; }

        public Collection<string> QueryItems {
            get {
                var items = new Collection<string>();
                foreach (var pnl in QueryItemPanels) {
                    if (pnl.Value != "") {
                        var item = EncodeQueryItem(pnl);
                        items.Add(item);
                    }
                }
                return items;
            }
        }

        #endregion

        #region Initialize Query Item Picklists

        public void SetComparisionPickList(string[] items) {
            foreach (var pnl in QueryItemPanels) {
                pnl.SetComparisionPickList(items);
            }
        }

        public void SetColumnPickList(string[] items) {
            foreach (var pnl in QueryItemPanels) {
                pnl.SetColumnPickList(items);
            }
        }

        #endregion

        #region Utility Functions

        private static string EncodeQueryItem(FlexQueryItemPanel pnl) {
            return string.Format("{0}|{1}|{2}|{3}", pnl.Relation, pnl.Column, pnl.Comparision, pnl.Value);
        }

        private static void DecodeQueryItem(string item, FlexQueryItemPanel pnl) {
            var flds = item.Split('|');
            pnl.Relation = flds[0];
            pnl.Column = flds[1];
            pnl.Comparision = flds[2];
            pnl.Value = flds[3];
        }

        private void GetJobsCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("get_entities_from_flex_query", QueryName));
        }

        #endregion

        public SQLBuilder GetSQLBuilder(string queryTemplate) {
            var args = new Dictionary<string, string>();
            var builder = new SQLBuilder(queryTemplate, ref args);
            foreach (var item in QueryItems.ToArray()) {
                var flds = item.Split('|');
                if (!string.IsNullOrEmpty(flds[0]) && !string.IsNullOrEmpty(flds[1]) && !string.IsNullOrEmpty(flds[2]) && !string.IsNullOrEmpty(flds[3])) {
                    builder.AddPredicateItem(flds[0], flds[1], flds[2], flds[3]);
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
        public static MSSQLReader GetMSSQLReaderFromFlexQuery(string queryTemplate, string[] queryItems) {
            Dictionary<string, string> args = new Dictionary<string, string>();
            SQLBuilder builder = new SQLBuilder(queryTemplate, ref args);
            foreach (string item in queryItems) {
                string[] flds = item.Split('|');
                if (!string.IsNullOrEmpty(flds[0]) && !string.IsNullOrEmpty(flds[1]) && !string.IsNullOrEmpty(flds[2]) && !string.IsNullOrEmpty(flds[3])) {
                    builder.AddPredicateItem(flds[0], flds[1], flds[2], flds[3]);
                }
            }
            MSSQLReader reader = new MSSQLReader();
            reader.Server = builder.SpecialArgs["Server"];
            reader.Database = builder.SpecialArgs["Database"];
            reader.SQLText = builder.BuildQuerySQL();
            return reader;
        }
*/
    }
}
