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
    public partial class Paddle : Form
    {
        Form1 ball;
        public RECT paddleRect;

        public Paddle(Form1 ball)
        {
            this.ball = ball;
            InitializeComponent();
        }

        private async void Paddle_Shown(object sender, EventArgs e)
        {
            Win32.GetWindowRect(new HandleRef(this, Handle), out paddleRect);
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            int difference = paddleRect.Right - paddleRect.Left;
            paddleRect.Right = screenBounds.Width - 150;
            paddleRect.Left = paddleRect.Right - difference + 80;

            while (true)
            {
                await Task.Delay(1);

                int center = (paddleRect.Bottom + paddleRect.Top) / 2;
                if (ball == null) Close();
                int ballCenter = (ball.FormRect.Bottom + ball.FormRect.Top) / 2;

                int vy;
                if (center > ballCenter) vy = -5;
                else vy = 5;

                Win32.SetWindowPos(Handle, (IntPtr)0, paddleRect.Left, paddleRect.Top + vy, paddleRect.Right - paddleRect.Left, paddleRect.Bottom - paddleRect.Top, SetWindowPosFlags.SWP_DRAWFRAME);

                Win32.GetWindowRect(new HandleRef(this, Handle), out paddleRect);
            }
        }
    }
}
