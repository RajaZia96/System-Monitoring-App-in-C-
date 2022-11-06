using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideFormProgram
{
    public partial class pictureShow : Form
    {
        private Image image;
        public pictureShow()
        {
            InitializeComponent();
        }

        public void setimage(Image img)
        {
            this.image = img;
        }

        private void pictureShow_Load(object sender, EventArgs e)
        {
            PictureBox pic = new PictureBox();
            Image.GetThumbnailImageAbort mycallback = new Image.GetThumbnailImageAbort(ThumbnailImage);
            Bitmap mybitmap = new Bitmap(image);
            Image mythumb = mybitmap.GetThumbnailImage(800, 550, mycallback, IntPtr.Zero);
            pic.Size = new System.Drawing.Size(800,550);
            pic.Image = mythumb;
            flowLayoutPanel1.Controls.Add(pic);
        }

        private bool ThumbnailImage()
        {
            return true;
            //throw new NotImplementedException();
        }
    }
}
