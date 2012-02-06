using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace SiouxTube
{
    class Mouse
    {
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref System.Drawing.Point cursorInfo);

        /// <summary>
        /// Gets and sets the location of the mouse
        /// </summary>
        public static Point Location
        {
            get
            {
                var point = new System.Drawing.Point();
                GetCursorPos(ref point);
                return new Point(point.X, point.Y);
            }
            set
            {
                SetCursorPos((int)value.X, (int)value.Y);
            }
        }
    }
}
