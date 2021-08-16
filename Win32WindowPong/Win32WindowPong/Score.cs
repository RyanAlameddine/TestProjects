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
    public partial class Score : Form
    {
        Form1 ball;

        public Score(Form1 ball)
        {
            this.ball = ball;
            InitializeComponent();
        }

        private async void Paddle_Shown(object sender, EventArgs e)
        {
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

            RECT formRect;
            Win32.GetWindowRect(new HandleRef(this, Handle), out formRect);

            int midpoint = screenBounds.Width / 2;

            int formWidth = formRect.Right - formRect.Left;

            formRect.Left = midpoint - formWidth / 2;
            formRect.Right = midpoint + formWidth / 2;

            Win32.SetWindowPos(Handle, (IntPtr)0, formRect.Left, formRect.Top, formRect.Right - formRect.Left, formRect.Bottom - formRect.Top, SetWindowPosFlags.SWP_DRAWFRAME);



            while (true)
            {
                await Task.Delay(1);

                if (ball == null) Close();

                label1.Text = $"{ball.scoreL} | {ball.scoreR}";
            }
        }

        private void Score_Load(object sender, EventArgs e)
        {

        }
    }
}
