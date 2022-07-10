using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace OCRNetFrame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }

        private const int cCaption = 32;
        private const int cBorder = 8;

        private string OCR(Bitmap b)
        {
            string res = "";
            using (var engine = new TesseractEngine(@"tessdata", "vie", EngineMode.Default))
            {
                using (var page = engine.Process(b, PageSegMode.AutoOnly))
                    res = page.GetText();
            }

            res = res.Replace('º', 'o').Replace('\n', ' ').Trim();
            return res;
        }

        private Bitmap captureScreen() {

            var width = this.Width  - 2 * cBorder;
            var height = this.Height - cCaption - 2 * cBorder;
            var x = this.Location.X + cBorder;
            var y = this.Location.Y + cCaption;
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            return bmp;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            this.Hide();
            var image = captureScreen();
            var text = OCR(image);
            if (String.IsNullOrEmpty(text))
            {
                text = " ";
            }
            Clipboard.SetText(text);
            this.Text = text;
            this.Show();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.ClientRectangle.Inflate(-10, -10);
            ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.LawnGreen, ButtonBorderStyle.Solid);
        }
    }
}
