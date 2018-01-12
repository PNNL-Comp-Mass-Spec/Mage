using System.Collections.Generic;
using System.Text;
using Mage;

namespace MageFilePackager
{

    class XMLSink : BaseModule
    {

        #region Member Variables

        readonly StringBuilder _text = new StringBuilder();

        #endregion

        #region Properties

        public string Text { get { return _text.ToString(); } }

        public Dictionary<string, string> Prefixes { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        #endregion

        #region Constructors

        #endregion


        #region IBaseModule Members

        public override void Prepare()
        {
            base.Prepare();
            OutputParameterElements();
            OutputPrefixElements();
        }

        /// <summary>
        /// Handler for Mage standard tablular input data rows
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

        #endregion

        #region Support Functions

        private void OutputParameterElements()
        {
            if (Parameters != null)
            {
                foreach (var kv in Parameters)
                {
                    _text.Append(string.Format("<parameter name='{0}' value='{1}' />\n", kv.Key, kv.Value));
                }
            }
        }

        private void OutputPrefixElements()
        {
            if (Prefixes != null)
            {
                foreach (var kv in Prefixes)
                {
                    _text.Append(string.Format("<prefix source='{0}' value='{1}' />\n", kv.Key, kv.Value));
                }
            }
        }

        private void OutputFooter()
        {
        }

        private void OutputDataRow(string[] vals)
        {
            _text.Append("<path ");
            for (var i = 0; i < InputColumnDefs.Count; i++)
            {
                _text.Append(string.Format("{0}='{1}' ", InputColumnDefs[i].Name, vals[i]));
            }
            _text.Append(" />\n");
        }

        #endregion
    }
}
