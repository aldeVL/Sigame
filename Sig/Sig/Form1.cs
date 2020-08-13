using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Sig
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDc, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        public Form1()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(700, 350);
        }
        bool _running = true;
        private void button1_Click(object sender, EventArgs e)
        {
            _running = true;
            label1.ForeColor = Color.Red;
            label1.Text = "АКТИВНО...";
            PollPixel();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _running = false;
            label1.ForeColor = Color.Black;
            label1.Text = "ОСТАНОВЛЕНО";
        }

        private void PollPixel()
        {
           
                while (_running == true)
                {
                     var c = GetColorAt(new System.Drawing.Point(268, 292));
                    pictureBox1.BackColor = c;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                if (c.R == 255 && c.G == 230 && c.B == 130)
                    {
                    answer();
                    label1.ForeColor = Color.Black;
                    label1.Text = "ОСТАНОВЛЕНО";
                    _running = false;
                    return;
                    }

                Application.DoEvents();
                Thread.Sleep(1);
                }
            
        }

        private void answer()
        {
            IntPtr progr = FindWindow(null, "SIGame");
            SetForegroundWindow(progr);
            Thread.Sleep(1);
            SendKeys.Send("L");
            return;
        }

        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        public Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            using (var bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height))
            {
                pictureBox1.DrawToBitmap(bmp, pictureBox1.ClientRectangle);
                var color = bmp.GetPixel(e.X, e.Y);
                var red = color.R;
                var green = color.G;
                var blue = color.B;
                label1.Text = Convert.ToString(red + " " + green + " " + blue + "");
            }
        }
    }


}
