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
        Stopwatch stopwatch = new Stopwatch();


        public Form1()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var openDialog = new FolderBrowserDialog())
            {
                if (openDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                stopwatch.Start();
                RemoveDuplicates(openDialog.SelectedPath);
                stopwatch.Stop();
                MessageBox.Show("Time :" + stopwatch.ElapsedMilliseconds + " ms");
                stopwatch.Reset();
            }
        }

        public static List<ListItem> LoadAllImagesInFolder(String folderPath)
        {
            List<ListItem> imageList = new List<ListItem>();
            //LinkedList<ListItem> imageLList = new LinkedList<ListItem>();
            //List<string> images = new List<string>(Directory.EnumerateFiles(folderPath, "*.*").Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png") || s.EndsWith(".gif")));
            string[] images = Directory.EnumerateFiles(folderPath, "*.*").Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png") || s.EndsWith(".gif")).ToArray();

           
            foreach (var img in images)
            {
                imageList.Add(new ListItem(InsertImageValue(img), img));
                //imageLList.AddLast(new ListItem(InsertImageValue(img), img));
            }

            //images.Clear();
            return imageList;
        }
        
        public static Byte[] InsertImageValue(String imagePath)
        {
            using (Stream stream = File.OpenRead(imagePath))
            {
                using (Image sourceImage = Image.FromStream(stream, false, false))
                {
                    return GetValue(MakeGrayscale(ResizeImage(sourceImage, new Size(16, 16))));
                }
            }
        }

        public static void RemoveDuplicates(String folderPath)
        {
            
            //StringBuilder destinationPath = new StringBuilder(folderPath + "\\_Duplicate\\");
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

                    //Llist
                    //if (IsDuplicate(allImages.ElementAt(i).GrayscaleValue, allImages.ElementAt(j).GrayscaleValue))
                    //{
                    //    File.Move(folderPath + "\\" + Path.GetFileName(allImages.ElementAt(j).OriginalImagePath), destinationPath + Path.GetFileName(allImages.ElementAt(j).OriginalImagePath));
                    //    allImages.Remove(allImages.ElementAt(j));
                    //}
                    //else
                    //    j++;
                }
            }
        }
 
        public static bool IsDuplicate(Byte[] bmpLeftValue, Byte[] bmpRightValue)
        {

           return (PercentageDifference(bmpLeftValue, bmpRightValue) < 4) ? true : false;
        }

        public static Bitmap MakeGrayscale(Bitmap original)
        {

            Bitmap newBitmap = new Bitmap(original.Width, original.Height);


            using (Graphics g = Graphics.FromImage(newBitmap))
            {

                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                            new float[] {.33f, .33f, .33f, 0, 0},
                            new float[] {.33f, .33f, .33f, 0, 0},
                            new float[] {.33f, .33f, .33f, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {0, 0, 0, 0, 1}
                    });

                ImageAttributes attributes = new ImageAttributes();

                attributes.SetColorMatrix(colorMatrix);

                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            }
            return newBitmap;
        }

        public static Bitmap ResizeImage(Image image, Size size)
        {
            return new Bitmap(image, size);         
        }

        public static float PercentageDifference(Byte[] left, Byte[] right, int threshold = 1)
        {
            byte[] difference = Compare(left, right);
            int diffPixels = 0;

            foreach (byte b in difference)
            {
                if (b > threshold)
                    diffPixels++;
            }
            return diffPixels/256f * 100 ;
        }

        public static byte[] Compare(Byte[] left, Byte[] right)
        {
            byte[] diff = new byte[256];
            
            for (int i = 0; i < 256; i++)
            {
                
                    diff[i] = (byte)Math.Abs(left[i] - right[i]);
                
            }

            return diff;
        }

        public static byte[] GetValue(Bitmap bmp)
        {
            ////v2 popravi
            //BitmapData bmpData = null;

            //try
            //{
            //    bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            //    //int numBytes = bmpData.Stride * bmp.Height;
            //    int numBytes = bmpData.Width * bmp.Height;
            //    byte[] byteData = new byte[numBytes];
            //    IntPtr ptr = bmpData.Scan0;

            //    Marshal.Copy(ptr, byteData, 0, numBytes);

            //    return byteData;
            //}
            //finally
            //{
            //    if (bmpData != null)
            //        bmp.UnlockBits(bmpData);
            //}

            //v1
            byte[] grayValue = new byte[256];

            int i = 0;

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    grayValue[i] = (byte)Math.Abs(bmp.GetPixel(x, y).R);
                    i++;
                }
            }
            return grayValue;
        }

    }
}
