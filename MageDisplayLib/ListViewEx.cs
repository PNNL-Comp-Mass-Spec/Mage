using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MageDisplayLib
{

    /// <summary>
    /// Intercepts horizontal scrollbar events, gets X position of scroll, and fires onScroll event
    /// </summary>
    public class ListViewEx : ListView
    {

        /// <summary>
        /// Event that is fired to simulate a .Net scroll event
        /// </summary>
        public event ScrollEventHandler onScroll;

        // Windows messages
        private const int WM_HSCROLL = 0x0114;
        private const int WM_VSCROLL = 0x0115;

        // ScrollBar types
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        /// <summary>
        /// Override WndProc to snare horizontal scrolling event from raw Windows message stream
        /// (since ListView does not provide scrolling event)
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HSCROLL)
            {
                var hargs = new ScrollEventArgs(ScrollEventType.EndScroll, GetScrollPos(Handle, SB_HORZ));
                onScroll?.Invoke(this, hargs);
            }

        }

        /// <summary>
        /// Call to raw WinAPI function to get position of horizontal scroll bar
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

    }


}
