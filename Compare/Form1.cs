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
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO;

namespace Compare
{
    public partial class Form1 : Form
    {
        private Bitmap img1;
        private Bitmap img2;
        Stopwatch stopwatch = new Stopwatch();
        public string fpath;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                if (openDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                this.DisposeImg1();

                //Slika bez modifikacija
                //this.img1 = new Bitmap(openDialog.FileName);

                //Sporije
                //Prebaci u grayscale zatim resize
                //this.img1 = ResizeImage(MakeGrayscale(new Bitmap(openDialog.FileName)), 16, 16);
                //this.img1 = ResizeImage2(MakeGrayscale(new Bitmap(openDialog.FileName)), new Size(16, 16));

                //Brze
                //Resize zatim prebaci u grayscale
                img1 = MakeGrayscale(ResizeImage2(new Bitmap(openDialog.FileName), new Size(16, 16)));

            }

            this.pictureBox1.Image = this.img1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                if (openDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                DisposeImg2();
                stopwatch.Start();

                img2 = MakeGrayscale(ResizeImage2(new Bitmap(openDialog.FileName), new Size(16, 16)));

                stopwatch.Stop();
                long t1 = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();
                //MessageBox.Show("Trajalo je " + t1.ToString() + " milisekundi");

            }

            this.pictureBox2.Image = this.img2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //stopwatch.Start();
            //stopwatch.Stop();
            //long t1 = stopwatch.ElapsedMilliseconds;
            //long t2 = stopwatch.ElapsedMilliseconds;
            //stopwatch.Reset();
            //MessageBox.Show(t1.ToString() + " " + t2.ToString());


            //stopwatch.Start();
            //float ne = PercentageDifference(img1, img2);
            //stopwatch.Stop();
            //long t1 = stopwatch.ElapsedMilliseconds;
            //stopwatch.Reset();
            //MessageBox.Show("Trajalo je :" + t1.ToString() + " milisekundi");

            //MessageBox.Show("Postotaka razlike je :" + ne.ToString() + "%" );


        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var openDialog = new FolderBrowserDialog())
            {
                if (openDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                fpath = openDialog.SelectedPath;
                RemoveDuplicates(fpath);
            }
        }

        public static List<ListItem> LoadAllImagesInFolder(string folderPath)
        {
            List<ListItem> imageList = new List<ListItem>();
            List<string> images = new List<string>(Directory.EnumerateFiles(folderPath, "*.*").Where(s => s.EndsWith(".jpg") || s.EndsWith(".png") || s.EndsWith(".gif")));
            
            foreach(var img in images)
            {
                
                ListItem listItem = new ListItem();
                listItem.GrayscaleValue = InsertImageValue(img);
                listItem.OriginalImagePath = img;
                imageList.Add(listItem);
            }

            images.Clear();
            
            return imageList;
        }
        
        public static Byte[,] InsertImageValue(string imagePath)
        {
            Byte[,] grayValue = new Byte[16, 16];
            using (var b = Image.FromFile(imagePath))
            {
                grayValue = GetValue(MakeGrayscale(ResizeImage2(b, new Size(16, 16))));

                return grayValue;
            }
                

        }

        public static void RemoveDuplicates(string folderPath)
        {
            
            string destinationPath = folderPath + "\\_Duplicate\\";
             var allImages = LoadAllImagesInFolder(folderPath);

            if (!Directory.Exists(destinationPath))
                    Directory.CreateDirectory(destinationPath);

            for(int i = 0; i < allImages.Count; i++)
            {
                for(int j=i+1; j < allImages.Count;)
                {
                    if (IsDuplicate(allImages[i].GrayscaleValue, allImages[j].GrayscaleValue))
                    {
                        File.Move(folderPath + "\\" + Path.GetFileName(allImages[j].OriginalImagePath), destinationPath + Path.GetFileName(allImages[j].OriginalImagePath));
                        allImages.RemoveAt(j);
                    }
                    else
                        j++;
                }
            }
        }

    
        public static bool IsDuplicate(Byte[,] bmpLeftValue, Byte[,] bmpRightValue)
        {

           return (PercentageDifference(bmpLeftValue, bmpRightValue) < 5) ? true : false;
        }

        public static Bitmap MakeGrayscale(Bitmap original)
        {

            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            using (Graphics g = Graphics.FromImage(newBitmap)) {

                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                        {
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                        });

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            }
            return newBitmap;
        }

        public static Bitmap ResizeImage2(Image image, Size size)
        {
            return new Bitmap(image, size);         
        }

        //public static void PrintGrid(byte[,] toPrint)
        //{
        //    int row = toPrint.GetLength(0);
        //    int col = toPrint.GetLength(1);

        //    for (int x = 0; x < row; x++)
        //    {
        //        Console.WriteLine();
        //        for (int y = 0; y < col; y++)
        //        {
        //            {
        //                Console.Write("{0,3} ", toPrint[x,y]);
        //            }
        //        }
        //    }
            
            
        //}

        public static float PercentageDifference(Byte[,] left, Byte[,] right, int threshold = 2)
        {
            byte[,] difference = Compare(left, right);
            int diffPixels = 0;

            //PrintGrid(difference);

            foreach (byte b in difference)
            {
                if (b > threshold)
                    diffPixels++;
            }
            return diffPixels/256f * 100 ;
        }

        public static byte[,] Compare(byte[,] left, byte[,] right)
        {
            byte[,] diff = new byte[16, 16];
            
            for (int height = 0; height < 16; height++)
            {
                for (int width = 0; width < 16; width++)
                {
                    diff[height, width] = (byte)Math.Abs(left[height, width] - right[height, width]);
                }
            }

            return diff;
        }

        public static byte[,] GetValue(Bitmap bmp)
        {
            byte[,] grayValue = new byte[16, 16];

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    grayValue[x, y] = (byte)Math.Abs(bmp.GetPixel(x, y).R);
                }
            }

            return grayValue;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.DisposeImg2();
            this.DisposeImg1();
        }

        private void DisposeImg1()
        {
            if (this.img1 == null)
            {
                return;
            }

            this.pictureBox1.Image = null;
            this.img1.Dispose();
            this.img1 = null;
        }

        private void DisposeImg2()
        {
            if (this.img2 == null)
            {
                return;
            }

            this.pictureBox2.Image = null;
            this.img2.Dispose();
            this.img2 = null;
        }


    }
}
