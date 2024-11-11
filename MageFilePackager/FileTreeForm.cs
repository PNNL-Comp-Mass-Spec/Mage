using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mage;
using MageDisplayLib;

namespace MageFilePackager
{
    public partial class FileTreeForm : Form
    {
        // Ignore Spelling: Mage, Precalculate

        public FileTreeForm()
        {
            InitializeComponent();
            treeView1.AfterCheck += NodeAfterCheck;
        }

        // Column information for the standard file package format
        private List<MageColumnDef> mColumnDefs;
        private Dictionary<string, int> mColumnPos;

        // Mage display source from which to load the tree view
        public GVPipelineSource FileListSource { get; set; }

        // Mage filter module to use in translating source display into standard file package format
        // (optional, depending on source)
        public BaseModule PackageFilter { get; set; }

        /// <summary>
        /// Populate tree when form loads
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PopulateTreeViewFromSource();
            TotalCheckedSizedCtl.Text = "";
        }

        /// <summary>
        /// Populate the tree view from the source of file paths
        /// using the packaging filter if necessary
        /// </summary>
        private void PopulateTreeViewFromSource()
        {
            var builder = new TreeBuilder { FileTree = treeView1 };
            if (PackageFilter == null)
            {
                ProcessingPipeline.Assemble("z", FileListSource, builder).RunRoot(null);
            }
            else
            {
                ProcessingPipeline.Assemble("z", FileListSource, PackageFilter, builder).RunRoot(null);
            }
            mColumnDefs = builder.ColumnDefs;
            mColumnPos = builder.ColumnPos;
            treeView1.Refresh();
        }

        /// <summary>
        /// Return a Mage source module that outputs file objects that were checked in tree
        /// </summary>
        public BaseModule GetSource()
        {
            return new TreeSource { ColumnDefs = mColumnDefs, FileTree = treeView1 };
        }

        private float mCheckedSizeTotal;
        private int mNumChecked;

        private void CalculateCheckedTotalSize()
        {
            mCheckedSizeTotal = 0;
            mNumChecked = 0;
            // Walk the tree and output a row for each checked file
            foreach (TreeNode node in treeView1.Nodes)
            {
                float runningTally = 0;
                SizeCheckedItems(node, ref runningTally);
                TotalCheckedSizedCtl.Text = string.Format("{0:###,###,##0.0} MB [{1}]", mCheckedSizeTotal / 1024, mNumChecked);
            }
        }
        private void SizeCheckedItems(TreeNode node, ref float runningTally)
        {
            // If this node is checked, and has an object stored in tag filed, add its object's size property to total
            float myTally = 0;
            if (node.Tag != null && node.Checked)
            {
                var row = (object[])node.Tag;
                var idx = mColumnPos["KB"];
                float.TryParse(row[idx].ToString(), out var sizeKB);
                mCheckedSizeTotal += sizeKB;
                mNumChecked++;
                myTally += sizeKB;
            }

            // Otherwise recursively call this method for all nodes in its nodes collection
            foreach (TreeNode subNode in node.Nodes)
            {
                SizeCheckedItems(subNode, ref myTally);
            }
            runningTally += myTally;

            if (node.Tag == null)
            {
                node.Text = string.Format("{0} [{1:###,###,##0.0} MB]", node.Name, myTally / 1024);
            }
        }

        private void ExpandAllBtnClick(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void ExpandToLevelClick(object sender, EventArgs e)
        {
            var label = ((Label)sender).Tag.ToString();
            int.TryParse(label, out var level);
            ExpandToLevel(level);
        }

        // Expand all nodes to given level
        private void ExpandToLevel(int level)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                ExpandNodes(node, level);
            }
        }

        // Expand child nodes to given level
        private void ExpandNodes(TreeNode node, int level)
        {
            if (node.Level < level)
            {
                node.Expand();
            }
            else
            {
                node.Collapse();
            }
            foreach (TreeNode subNode in node.Nodes)
            {
                ExpandNodes(subNode, level);
            }
        }

        // Updates checked state of all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildNodes method recursively.
                    CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        // NOTE   This code can be added to the BeforeCheck event handler instead of the AfterCheck event.
        // After a tree node's Checked property is changed, all its child nodes are updated to the same value.
        private void NodeAfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    // Calls the CheckAllChildNodes method, passing in the current
                    // Checked value of the TreeNode whose checked state changed.
                    CheckAllChildNodes(e.Node, e.Node.Checked);
                }
                CalculateCheckedTotalSize();
            }
        }

        /// <summary>
        /// Mage sink module that populates tree view from file paths
        /// received via standard tabular input
        /// </summary>
        public class TreeBuilder : ContentFilter
        {
            private int mPathIdx;
            private int mNameIdx;
            private int mKbIdx;

            public TreeView FileTree;

            public List<MageColumnDef> ColumnDefs;
            public Dictionary<string, int> ColumnPos;

            // Precalculate field indexes
            protected override void ColumnDefsFinished()
            {
                //  mItemIdx = InputColumnPos["Item"];
                mNameIdx = InputColumnPos["Name"];
                mKbIdx = InputColumnPos["KB"];
                mPathIdx = InputColumnPos["Path"];
                ColumnDefs = InputColumnDefs;
                ColumnPos = InputColumnPos;
            }

            protected override bool CheckFilter(ref string[] values)
            {
                var path = values[mPathIdx];
                var sizeKBText = values[mKbIdx];

                // Break directory path into segments
                var directoryParts = path.Split('\\');

                // Find bottom directory in hierarchy
                // (and build out as necessary)
                var curNodeList = FileTree.Nodes;
                foreach (var directoryName in directoryParts)
                {
                    if (string.IsNullOrEmpty(directoryName))
                        continue;

                    var n = curNodeList.IndexOfKey(directoryName);

                    TreeNode curNode;
                    if (n < 0)
                    {
                        curNode = new TreeNode(directoryName) { Name = directoryName };
                        curNodeList.Add(curNode);
                        curNodeList = curNode.Nodes;
                    }
                    else
                    {
                        curNode = curNodeList[n];
                        curNodeList = curNode.Nodes;
                    }
                }

                // Add file to bottom directory in path
                float.TryParse(sizeKBText, out var sizeKB);
                var label = string.Format("{0} [{1:###,###,##0.0} KB]", values[mNameIdx], sizeKB);
                var fileNode = new TreeNode { Text = label, Tag = values };
                curNodeList.Add(fileNode);
                return true;
            }
        }

        /// <summary>
        /// Mage source module that returns file paths from tree view
        /// that are checked
        /// </summary>
        public class TreeSource : BaseModule
        {
            public List<MageColumnDef> ColumnDefs;
            public TreeView FileTree;

            public override void Run(object state)
            {
                OnColumnDefAvailable(new MageColumnEventArgs(ColumnDefs.ToArray()));

                // Walk the tree and output a row for each checked file
                foreach (TreeNode node in FileTree.Nodes)
                {
                    OutputCheckedItems(node);
                }
                OnDataRowAvailable(new MageDataEventArgs(null));
            }

            private void OutputCheckedItems(TreeNode node)
            {
                // If this node is checked, and has an object stored in tag filed, output it
                if (node.Tag != null && node.Checked)
                {
                    var tagData = (object[])node.Tag;

                    var tagVals = new string[tagData.Length];
                    for (var i = 0; i < tagData.Length; i++)
                        tagVals[i] = tagData[i].ToString();

                    OnDataRowAvailable(new MageDataEventArgs(tagVals));
                }
                // Otherwise recursively call this method for all nodes in its nodes collection
                foreach (TreeNode subNode in node.Nodes)
                {
                    OutputCheckedItems(subNode);
                }
            }
        }
    }
}
