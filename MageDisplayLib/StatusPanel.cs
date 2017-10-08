using System;
using System.Windows.Forms;
using Mage;

namespace MageDisplayLib
{

    /// <summary>
    /// UI panel that provides a display area for status update messages
    /// and a cancel button
    /// </summary>
    public partial class StatusPanel : UserControl
    {

        /// <summary>
        /// this event fires to send command to external command handler(s)
        /// </summary>
        public event EventHandler<MageCommandEventArgs> OnAction;

        private readonly System.Collections.Generic.List<string> mWarningMessages;

        /// <summary>
        /// Construct a new StatusPanel UI component
        /// </summary>
        public StatusPanel()
        {
            InitializeComponent();
            mWarningMessages = new System.Collections.Generic.List<string>();
        }

        #region Properties

        /// <summary>
        /// the control that we are installed in
        /// </summary>
        public Control OwnerControl { get; set; }

        /// <summary>
        /// set the status message display
        /// </summary>
        /// <param name="Message"></param>
        public void SetStatusMessage(string Message)
        {
            StatusMessageCtl.Text = Message;
        }

        /// <summary>
        /// enable or disable the cancel button
        /// </summary>
        public bool EnableCancel
        {
            get => CancelCtl.Enabled;
            set => CancelCtl.Enabled = value;
        }

        /// <summary>
        /// show or hide the cancel button
        /// </summary>
        public bool ShowCancel
        {
            get => CancelCtl.Visible;
            set => CancelCtl.Visible = value;
        }

        #endregion

        /// <summary>
        /// Clear the cached warning messages
        /// </summary>
        public void ClearWarnings()
        {
            mWarningMessages.Clear();
            WarningsCtl.Text = "No warnings";
            WarningsCtl.Visible = false;
        }

        private void CancelCtl_Click(object sender, EventArgs e)
        {
            OnAction?.Invoke(this, new MageCommandEventArgs("cancel_operation"));
        }

        #region Handle inter-thread message updates

        private delegate void MessageHandler(string message);

        /// <summary>
        /// handle a status message update that arrives from a different
        /// thread than our owner control's thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleStatusMessageUpdated(object sender, MageStatusEventArgs args)
        {
            // we need to do the cross-thread thing to update the GUI
            UpdateStatusMessage(args.Message);
        }


        /// <summary>
        /// handle a warning message update that arrives from a different
        /// thread than our owner control's thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void HandleWarningMessageUpdated(object sender, MageStatusEventArgs args)
        {
            // we need to do the cross-thread thing to update the GUI
            AddNewWarningMessage(args.Message);
        }

        /// <summary>
        /// handle a status completion message that arrives from a different
        /// thread than our owner control's thread
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        public void HandleCompletionMessageUpdate(object sender, MageStatusEventArgs args)
        {
            if (string.IsNullOrEmpty(args.Message))
            {
                // pipeline didn't blow up, make nice reassuring message
                args.Message = "Process completed normally";
            }
            UpdateStatusMessage(args.Message);
        }

        /// <summary>
        /// Append a new entry to the warning message list
        /// </summary>
        /// <param name="message">New status message</param>
        private void AddNewWarningMessage(string message)
        {
            if (OwnerControl.InvokeRequired)
            {
                MessageHandler ncb = AddWarningMessage;
                try
                {
                    OwnerControl.Invoke(ncb, message);
                }
                catch (ObjectDisposedException)
                {
                    // Ignore this exception
                    // May occur if user closes application while a pipeline is running
                }

            }
            else
            {
                AddWarningMessage(message);
            }
        }

        /// <summary>
        /// Append a new warning message to mWarningMessages
        /// Update the caption on the Warning Messages button
        /// </summary>
        /// <param name="sMessage"></param>
        private void AddWarningMessage(string sMessage)
        {
            mWarningMessages.Add(sMessage);

            if (mWarningMessages.Count == 1)
            {
                WarningsCtl.Text = "1 warning";
            }
            else
            {
                WarningsCtl.Text = mWarningMessages.Count + " warnings";
            }
            WarningsCtl.Visible = true;

        }

        /// <summary>
        /// update our status message in the owner control's thread
        /// </summary>
        /// <param name="message">New status message</param>
        private void UpdateStatusMessage(string message)
        {
            if (OwnerControl.InvokeRequired)
            {
                MessageHandler ncb = SetStatusMessage;
                try
                {
                    OwnerControl.Invoke(ncb, message);
                }
                catch (ObjectDisposedException)
                {
                    // Ignore this exception
                    // Occurs if user closes application while a pipeline is running
                    // The pipeline thread tries to post a status message, but the GUI panel is disposed, and thus the control cannot be updated
                }


            }
            else
            {
                SetStatusMessage(message);
            }
        }

        #endregion

        /// <summary>
        /// Make reasonable guess at who my owner form is
        /// by walking up my parentage chain
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusPanel_Load(object sender, EventArgs e)
        {
            Control x = this;
            while (x.Parent != null)
            {
                x = x.Parent;
                if (x is Form)
                {
                    OwnerControl = x;
                    break;
                }
            }
        }

        private void WarningsCtl_Click(object sender, EventArgs e)
        {
            var frmWarnings = new TextDisplayForm
            {
                Text = "Mage Warnings",
                ReadOnly = true
            };

            frmWarnings.SetLines(mWarningMessages.ToArray());
            frmWarnings.Show(this);
        }
    }
}
