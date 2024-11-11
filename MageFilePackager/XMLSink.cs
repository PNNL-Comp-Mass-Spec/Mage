using System.Collections.Generic;
using System.Text;
using Mage;

namespace MageFilePackager
{
    internal class XMLSink : BaseModule
    {
        // Ignore Spelling: Mage

        private readonly StringBuilder mText = new();

        public string Text => mText.ToString();

        public Dictionary<string, string> Prefixes { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public override void Prepare()
        {
            base.Prepare();
            OutputParameterElements();
            OutputPrefixElements();
        }

        /// <summary>
        /// Handler for Mage standard tabular input data rows
        /// (override of base class)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void HandleDataRow(object sender, MageDataEventArgs args)
        {
            if (args.DataAvailable)
            {
                OutputDataRow(args.Fields);
            }
            else
            {
                OutputFooter();
            }
        }

        // Support methods

        private void OutputParameterElements()
        {
            if (Parameters != null)
            {
                foreach (var kv in Parameters)
                {
                    mText.AppendFormat("<parameter name='{0}' value='{1}' />\n", kv.Key, kv.Value);
                }
            }
        }

        private void OutputPrefixElements()
        {
            if (Prefixes != null)
            {
                foreach (var kv in Prefixes)
                {
                    mText.AppendFormat("<prefix source='{0}' value='{1}' />\n", kv.Key, kv.Value);
                }
            }
        }

        private void OutputFooter()
        {
        }

        private void OutputDataRow(IReadOnlyList<string> values)
        {
            mText.Append("<path ");
            for (var i = 0; i < InputColumnDefs.Count; i++)
            {
                mText.AppendFormat("{0}='{1}' ", InputColumnDefs[i].Name, values[i]);
            }
            mText.Append(" />\n");
        }
    }
}
