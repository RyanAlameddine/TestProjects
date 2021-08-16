using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Library;

namespace Win32WindowPong
{
    public partial class Form1 : Form
    {
        public RECT FormRect;

        public int scoreL = 0;
        public int scoreR = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            int vx = 10;
            int vy = 7;

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            Paddle paddle = new Paddle(this);
            Score score = new Score(this);
            score.Show();
            paddle.Show();

            IntPtr[] specialWindows = new IntPtr[] { Handle, paddle.Handle, score.Handle };

            IntPtr thisWindow = Handle;

            while (true)
            {
                await Task.Delay(1);
                IntPtr topWindow = GetTopWindow(specialWindows);
                Debug.WriteLine(Win32.GetText(topWindow));

                RECT topRect;

                Win32.GetWindowRect(new HandleRef(this, thisWindow), out FormRect);
                Win32.GetWindowRect(new HandleRef(null, topWindow), out topRect);

                CheckIntersection(ref FormRect, topRect, paddle.paddleRect, ref vx, ref vy);
                CheckBounds(ref FormRect, screenBounds, ref vx, ref vy);

                Win32.SetWindowPos(thisWindow, (IntPtr) 0, FormRect.Left + vx, FormRect.Top + vy, FormRect.Right - FormRect.Left, FormRect.Bottom - FormRect.Top, SetWindowPosFlags.SWP_DRAWFRAME);
            }
        }

        private void CheckIntersection(ref RECT thisRect, RECT topRect, RECT paddleRect, ref int vx, ref int vy)
        {
            int middleY = (thisRect.Bottom + thisRect.Top) / 2;
            if (thisRect.Right > paddleRect.Left && thisRect.Left < paddleRect.Left &&
                (middleY < paddleRect.Bottom && middleY > paddleRect.Top)) vx = -Math.Abs(vx);

            else if (thisRect.Left < topRect.Right && thisRect.Right > topRect.Right &&
                (middleY < topRect.Bottom && middleY > topRect.Top)) vx = Math.Abs(vx);

            //if (thisRect.Top < topRect.Bottom) vy = Math.Abs(vy);
            //else if (thisRect.Bottom > topRect.Top) vy = -Math.Abs(vy);
        }

        private void CheckBounds(ref RECT thisRect, Rectangle screenBounds, ref int vx, ref int vy)
        {
            if (thisRect.Left < 0)
            {
                vx = Math.Abs(vx);
                scoreR++;
            }
            else if (thisRect.Right > screenBounds.X + screenBounds.Width)
            {
                vx = -Math.Abs(vx);
                scoreL++;
            }
            
            if (thisRect.Top < 0) vy = Math.Abs(vy);
            else if (thisRect.Bottom > screenBounds.Y + screenBounds.Height) vy = -Math.Abs(vy);       
        }

        private IntPtr GetTopWindow(IntPtr[] skippedWindows)
        {
            IntPtr topWindow = Win32.GetTopWindow((IntPtr)null);
            if (topWindow != IntPtr.Zero)
            {
                while (!Win32.IsWindowVisible(topWindow) || skippedWindows.Contains(topWindow) || Win32.GetWindowTextLength(topWindow) < 1)
                {
                    topWindow = Win32.GetNextWindow(topWindow, Win32.GW_HWNDNEXT);
                }
            }

            return topWindow;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
