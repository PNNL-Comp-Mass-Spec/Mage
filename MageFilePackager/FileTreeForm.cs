using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageFilePackager {

    public partial class FileTreeForm : Form {

        public FileTreeForm() {
            InitializeComponent();
            treeView1.AfterCheck += NodeAfterCheck;
        }

        #region Member Variables

        // column information for the standard file package format
        private List<MageColumnDef> _columnDefs;
        private Dictionary<string, int> _columnPos;

        #endregion

        #region Properties

        // Mage display source from which to load the tree veiw
        public GVPipelineSource FileListSource { get; set; }

        // Mage filter module to use in translating source display into standard file package format
        // (optional, depending on source)
        public BaseModule PackageFilter { get; set; }

        #endregion

        /// <summary>
        /// Populate tree when form loads
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            PopulateTreeViewFromSource();
            TotalCheckedSizedCtl.Text = "";
        }

        /// <summary>
        /// Populate the tree view from the source of file paths
        /// using the packaging filter if necessary
        /// </summary>
        private void PopulateTreeViewFromSource() {
            var builder = new TreeBuilder { FileTree = treeView1 };
            if (PackageFilter == null) {
                ProcessingPipeline.Assemble("z", FileListSource, builder).RunRoot(null);
            } else {
                ProcessingPipeline.Assemble("z", FileListSource, PackageFilter, builder).RunRoot(null);
            }
            _columnDefs = builder.ColumnDefs;
            _columnPos = builder.ColumnPos;
            treeView1.Refresh();
        }

        /// <summary>
        /// Return a Mage source module that outputs file objects that were checked in tree
        /// </summary>
        /// <returns></returns>
        public BaseModule GetSource() {
            return new TreeSource { ColumnDefs = _columnDefs, FileTree = treeView1 };
        }

        #region File Selection

        private float _checkedSizeTotal;
        private int _numChecked;

        private void CalculateCheckedTotalSize() {
            _checkedSizeTotal = 0;
            _numChecked = 0;
            // Walk the tree and output a row for each checked file
            foreach (TreeNode node in treeView1.Nodes) {
                float runningTally = 0;
                SizeCheckedItems(node, ref runningTally);
                TotalCheckedSizedCtl.Text = string.Format("{0:###,###,##0.0} MB [{1}]", _checkedSizeTotal / 1024, _numChecked);
            }
        }
        private void SizeCheckedItems(TreeNode node, ref float runningTally) {
            // if this node is checked, and has an object stored in tag filed, add its object's size property to total
            float myTally = 0;
            if (node.Tag != null && node.Checked) {
                var row = (object[])node.Tag;
                int idx = _columnPos["KB"];
                float kb;
                float.TryParse(row[idx].ToString(), out kb);
                _checkedSizeTotal += kb;
                _numChecked++;
                myTally += kb;

            }
            // otherwise recursively call this method for all nodes in its nodes collection
            foreach (TreeNode subNode in node.Nodes) {
                SizeCheckedItems(subNode, ref myTally);
            }
            runningTally += myTally;
            // 
            if (node.Tag == null) {
                node.Text = string.Format("{0} [{1:###,###,##0.0} MB]", node.Name, myTally / 1024);
            }
        }

        #endregion

        #region Tree Control Utilities

        private void ExpandAllBtnClick(object sender, EventArgs e) {
            treeView1.ExpandAll();
        }

        private void ExpandToLevelClick(object sender, EventArgs e) {
            int level;
            var label = ((Label)sender).Tag.ToString();
            int.TryParse(label, out level);
            ExpandToLevel(level);
        }


        // expand all nodes to given level
        private void ExpandToLevel(int level) {
            foreach (TreeNode node in treeView1.Nodes) {
                ExpandNodes(node, level);
            }
        }

        // expand child nodes to given level
        private void ExpandNodes(TreeNode node, int level) {
            if (node.Level < level) {
                node.Expand();
            } else {
                node.Collapse();
            }
            foreach (TreeNode subNode in node.Nodes) {
                ExpandNodes(subNode, level);
            }
        }

        // Updates checked state of all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked) {
            foreach (TreeNode node in treeNode.Nodes) {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0) {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        // NOTE   This code can be added to the BeforeCheck event handler instead of the AfterCheck event.
        // After a tree node's Checked property is changed, all its child nodes are updated to the same value.
        private void NodeAfterCheck(object sender, TreeViewEventArgs e) {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown) {
                if (e.Node.Nodes.Count > 0) {
                    // Calls the CheckAllChildNodes method, passing in the current 
                    // Checked value of the TreeNode whose checked state changed.
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                CalculateCheckedTotalSize();
            }
        }

        #endregion

        #region Internal Classes

        /// <summary>
        /// Mage sink module that populates tree view from file paths
        /// recevied via standard tabular input
        /// </summary>
        public class TreeBuilder : ContentFilter {

            // private int _folderIdx;
            //  private int _sourceIdx;
            private int _pathIdx;
            //  private int _itemIdx;
            private int _nameIdx;
            private int _kbIdx;

            public TreeView FileTree;

            public List<MageColumnDef> ColumnDefs;
            public Dictionary<string, int> ColumnPos;

            // Precalulate field indexes
            protected override void ColumnDefsFinished() {
                //  _itemIdx = InputColumnPos["Item"];
                _nameIdx = InputColumnPos["Name"];
                _kbIdx = InputColumnPos["KB"];
                _pathIdx = InputColumnPos["Path"];
                ColumnDefs = InputColumnDefs;
                ColumnPos = InputColumnPos;
            }

			protected override bool CheckFilter(ref string[] vals)
			{
                string path = vals[_pathIdx];
                string kb = vals[_kbIdx];

                // break folder path into segements
                string[] folders = path.Split('\\');

                // find bottom folder in hierarchy 
                // (and build out as necessary)
                TreeNodeCollection curNodeList = FileTree.Nodes;
                TreeNode curNode;
                foreach (string folder in folders) {
                    if (string.IsNullOrEmpty(folder)) continue;
                    var n = curNodeList.IndexOfKey(folder);
                    if (n < 0) {
                        curNode = new TreeNode(folder) { Name = folder };
                        curNodeList.Add(curNode);
                        curNodeList = curNode.Nodes;
                    } else {
                        curNode = curNodeList[n];
                        curNodeList = curNode.Nodes;
                    }
                }

                // add file to bottom folder in path
                float sz;
                float.TryParse(kb, out sz);
                string label = string.Format("{0} [{1:###,###,##0.0} KB]", vals[_nameIdx], sz);
                var fileNode = new TreeNode { Text = label, Tag = vals };
                curNodeList.Add(fileNode);
                return true;
            }
        }

        /// <summary>
        /// Mage source module that returns file paths from tree view
        /// that are checked
        /// </summary>
        public class TreeSource : BaseModule {

            public List<MageColumnDef> ColumnDefs;
            public TreeView FileTree;

            public override void Run(object state) {
                OnColumnDefAvailable(new MageColumnEventArgs(ColumnDefs.ToArray()));

                // Walk the tree and output a row for each checked file
                foreach (TreeNode node in FileTree.Nodes) {
                    OutputCheckedItems(node);
                }
                OnDataRowAvailable(new MageDataEventArgs(null));

            }

            private void OutputCheckedItems(TreeNode node) {
                // if this node is checked, and has an object stored in tag filed, output it
                if (node.Tag != null && node.Checked) {
					object[] tagData = (object[])node.Tag;

					string[] tagVals = new string[tagData.Length];
					for (int i = 0; i < tagData.Length; i++)
						tagVals[i] = tagData[i].ToString();

					OnDataRowAvailable(new MageDataEventArgs(tagVals));
                }
                // otherwise recursively call this method for all nodes in its nodes collection
                foreach (TreeNode subNode in node.Nodes) {
                    OutputCheckedItems(subNode);
                }
            }
        }

        #endregion


    }
}
