using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OcrOverlay
{
    public partial class Reader : Form
    {

        Func<string, string> getText;
        Timer refreshTimer;

        public Reader(Func<string, string> getText)
        {
            InitializeComponent();
            this.getText = getText;
        }

        private void SelectArea_Shown(object sender, EventArgs e)
        {
            this.refreshTimer = new System.Windows.Forms.Timer();
            this.refreshTimer.Tick += new EventHandler(timer_Tick);
            this.refreshTimer.Interval = 100;
            this.refreshTimer.Start();
        }

        private void Reader_Closing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            string text = this.getText("");
            if (text != null && text != this.readerText.Text)
            {
                this.readerText.Text = text;
            }
        }

        private void readerText_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
