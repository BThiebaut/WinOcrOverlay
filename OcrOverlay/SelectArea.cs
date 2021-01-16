using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrOverlay
{
    public partial class SelectArea : Form
    {
       private const int
           HTLEFT = 10,
           HTRIGHT = 11,
           HTTOP = 12,
           HTTOPLEFT = 13,
           HTTOPRIGHT = 14,
           HTBOTTOM = 15,
           HTBOTTOMLEFT = 16,
           HTBOTTOMRIGHT = 17;

        const int borderWidth = 5; // you can rename this variable if you like

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, borderWidth); } }
        Rectangle Left { get { return new Rectangle(0, 0, borderWidth, this.ClientSize.Height); } }
        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - borderWidth, this.ClientSize.Width, borderWidth); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - borderWidth, 0, borderWidth, this.ClientSize.Height); } }
        Rectangle TopLeft { get { return new Rectangle(0, 0, borderWidth, borderWidth); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - borderWidth, 0, borderWidth, borderWidth); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - borderWidth, borderWidth, borderWidth); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - borderWidth, this.ClientSize.Height - borderWidth, borderWidth, borderWidth); } }

        Thread ThreadReader = null;
        Bitmap bmp;
        Reader reader;
        string currentText;
        bool check;
        System.Windows.Forms.Timer timer;
        

        public SelectArea()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.Sizable; // no borders
            this.Opacity = .1D; // make trasparent
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true); // this is to avoid visual artifacts
            check = true;
        }

        protected override void OnPaint(PaintEventArgs e) // you can safely omit this method if you want
        {
            e.Graphics.FillRectangle(Brushes.Gray, Top);
            e.Graphics.FillRectangle(Brushes.Gray, Left);
            e.Graphics.FillRectangle(Brushes.Gray, Right);
            e.Graphics.FillRectangle(Brushes.Gray, Bottom);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }

        private void SelectArea_Shown(object sender, EventArgs e)
        {
            this.reader = new Reader(DisplayText);
            this.ThreadReader = new Thread((ThreadStart)delegate { Application.Run(this.reader); });
            this.ThreadReader.Start();
            this.timer = new System.Windows.Forms.Timer();
            this.timer.Tick += new EventHandler(timer_Tick);
            this.timer.Interval = 500;
            this.timer.Start();
        }

        private void SelectArea_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ThreadReader = null;
            this.timer.Stop();
            Application.Exit();
        }

        private void CaptureScreen()
        {
            try
            {
                Rectangle rect = new Rectangle(this.Location.X, this.Location.Y, this.Width, this.Height);
                var nbmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                // Limit ocr scan on no changing zone
                if (bmp != null)
                {
                    var bmpbytes = ImageToByte(bmp);
                    var nbmpbytes = ImageToByte(nbmp);
                    if (Convert.ToBase64String(bmpbytes).Equals(Convert.ToBase64String(nbmpbytes)))
                    {
                        check = false;
                        return;
                    }

                }
                check = true;
                bmp = nbmp;
                Graphics g = Graphics.FromImage(bmp);
                g.CopyFromScreen(rect.Left, rect.Top, 0, 0, this.Size, CopyPixelOperation.SourceCopy);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.Dispose();

            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [DllImport("msvcrt.dll")]
        private static extern int memcmp(IntPtr b1, IntPtr b2, long count);

        private byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private string DisplayText(string arg)
        {
            return currentText;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Thread t = new Thread(ProcessOcr);
            t.Start();
        }

        private void ProcessOcr()
        {
            CaptureScreen();
            if (bmp != null && check)
            {
                currentText = new IronOcr.IronTesseract().Read(bmp).Text;
            }
        }
    }
}
