using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// ReSharper disable UnusedMember.Local

namespace MageDisplayLib
{
    /// <summary>
    /// Intercepts horizontal scrollbar events, gets X position of scroll, and fires the OnScroll event
    /// </summary>
    public class ListViewEx : ListView
    {
        /// <summary>
        /// Event that is fired to simulate a .NET scroll event
        /// </summary>
        public event EventHandler<ScrollEventArgs> OnScroll;

        // ReSharper disable IdentifierTypo

        // Windows messages
        private const int WM_HSCROLL = 0x0114;
        private const int WM_VSCROLL = 0x0115;

        // ScrollBar types
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        // ReSharper restore IdentifierTypo

        /// <summary>
        /// Override WndProc to snare horizontal scrolling event from raw Windows message stream
        /// (since ListView does not provide scrolling event)
        /// </summary>
        /// <param name="message">Message object</param>
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == WM_HSCROLL)
            {
                var args = new ScrollEventArgs(ScrollEventType.EndScroll, GetScrollPos(Handle, SB_HORZ));
                OnScroll?.Invoke(this, args);
            }
        }

        /// <summary>
        /// Call to raw WinAPI function to get position of horizontal scroll bar
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
    }
}
