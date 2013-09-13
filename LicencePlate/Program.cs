using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using YLScsDrawing.Imaging;

namespace LicencePlate
{
    class Program
    {
        static Color[,] _Px = new Color[720,1280];

        static bool IsYellow(Color Pix)
        {
            if (Pix.B > 60)
            {
                return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            string[] filePaths = Directory.GetFiles(@"C:\Users\Ben\Dropbox\Mandel\Mandel\bin\New folder", "*.jpg");
            
            foreach (string filep in filePaths)
            {

                // We need to find the points of the licence plate...
                int[] PointX = new int[4];
                int[] PointY = new int[4];
                bool SeenFirstBitOfYellow = false;
                Console.WriteLine("Processing {0}", filep);
                Bitmap Orig = (Bitmap)Bitmap.FromFile(filep);

                Console.WriteLine(Orig.GetPixel(689, 255).GetHue());
                //Process the image to Yellow Only
                for (int y = 0; y < Orig.Height; y++)
                {
                    for (int x = 0; x < Orig.Width; x++)
                    {
                        _Px[y, x] = Orig.GetPixel(x, y);
                        Color Pix = Orig.GetPixel(x,y);
                        int BWVal = 0;
                        if (!(((Pix.R + Pix.G) / 2) < Pix.B))
                        {
                            BWVal = ((Pix.R + Pix.G) / 2) - Pix.B;
                            if (BWVal < 50)
                            {
                                BWVal = 0;
                            }
                        }
                        //if (Pix.GetHue() < 65 && Pix.GetHue() > 35)
                        //{
                        //    BWVal = ((Pix.R + Pix.G) / 2);
                        //}
                        //BWVal = (int)Pix.GetHue();
                        //BWVal = BWVal / 4;
                        Orig.SetPixel(x,y,Color.FromArgb(BWVal,BWVal,BWVal));
                    }
                }
                Orig.Save(filep.Replace(".jpg", ".png").Replace("imagee","bwout"));
               
                for (int y = 300; y < Orig.Height; y++)
                {
                    for (int x = 0; x < Orig.Width; x++)
                    {
                        if (y > 300)
                        {
                            Color Pix = Orig.GetPixel(x, y);
                            if (!SeenFirstBitOfYellow)
                            {
                                if (IsYellow(Pix))
                                {

                                    // we need to check if the area is big enough for a plate
                                    // Also the brightness.
                                    try{
                                        if (Pix.R > 120)
                                        {
                                            Orig.SetPixel(x, y, Color.Red);
                                            // Ok so we just found a bright spot.
                                            // Lets go explore this.
                                            int xx = x;
                                            int yy = y;
                                            while (Orig.GetPixel(xx, yy - 1).B != 0)
                                            {
                                                yy--;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                                //Console.WriteLine("Going Higher to {0} Coming Next is {1}", yy, Orig.GetPixel(xx, yy + 1).B);
                                            }
                                            PointY[0] = yy;
                                            xx = x;
                                            yy = y;
                                            while (Orig.GetPixel(xx - 1, yy + 1).G != 0)
                                            {
                                                xx--;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                            }
                                            PointX[0] = xx;

                                            xx = x;
                                            yy = y;

                                            while (Orig.GetPixel(xx, yy + 1).B != 0)
                                            {
                                                yy++;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                                //Console.WriteLine("Going Higher to {0} Coming Next is {1}", yy, Orig.GetPixel(xx, yy + 1).B);
                                            }
                                            PointY[1] = yy;
                                            int tempx = xx;
                                            yy = yy - 10;
                                            while (Orig.GetPixel(xx - 1, yy + 1).G != 0)
                                            {
                                                xx--;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                            }
                                            PointX[1] = xx;
                                            xx = tempx;
                                            while (Orig.GetPixel(xx + 1, yy + 1).G != 0)
                                            {
                                                xx++;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                            }
                                            PointX[2] = xx;
                                            xx = xx - 10;
                                            int tempy = yy;
                                            while (Orig.GetPixel(xx, yy + 1).B != 0)
                                            {
                                                yy++;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                                //Console.WriteLine("Going Higher to {0} Coming Next is {1}", yy, Orig.GetPixel(xx, yy + 1).B);
                                            }
                                            PointY[2] = yy;
                                            yy = tempy;
                                            while (Orig.GetPixel(xx, yy - 2).B != 0)
                                            {
                                                yy--;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                                //Console.WriteLine("Going Higher to {0} Coming Next is {1}", yy, Orig.GetPixel(xx, yy + 1).B);
                                            }
                                            PointY[3] = yy;
                                            yy = yy + 10;
                                            while (Orig.GetPixel(xx + 1, yy + 1).G != 0)
                                            {
                                                xx++;
                                                Orig.SetPixel(xx, yy, Color.Red);
                                            }
                                            PointX[3] = xx;


                                            Orig.SetPixel(PointX[0], PointY[0], Color.Red);
                                            Orig.SetPixel(PointX[1], PointY[1], Color.Red);
                                            Orig.SetPixel(PointX[2], PointY[2], Color.Red);
                                            Orig.SetPixel(PointX[3], PointY[3], Color.Red);
                                            SeenFirstBitOfYellow = true;
                                            Console.WriteLine("Found Point 1 at {0} {1}", PointX[0], PointY[0]);
                                        
                                    }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                    }

                }

                Orig.Save(filep.Replace(".jpg", ".png").Replace("imagee", "ProcessedOut"),System.Drawing.Imaging.ImageFormat.Png);
                if (PointX[3] != 0)
                {
                    RescaleImage(new PointF(PointX[0], PointY[0]), new PointF(PointX[2], PointY[2]), new PointF(PointX[1], PointY[1]), new PointF(PointX[3], PointY[3]), 600, 90, filep.Replace("imagee", "LP"));
                }
                //Console.Read();

                // And now to Combine all the images together
                Bitmap Final = new Bitmap(1280, 720);

                using (Graphics g = Graphics.FromImage(Final))
                {
                    Image newImage = Image.FromFile(filep);
                    g.DrawImage(newImage, 0, 0);
                    if (PointX[3] != 0)
                    {
                        Image newImage2 = Image.FromFile(filep.Replace("imagee", "LP"));
                        g.DrawImage(newImage2, 0, 0);
                        g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[0], PointY[0]), new Point(PointX[1], PointY[1]));
                        g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[1], PointY[1]), new Point(PointX[2], PointY[2]));
                        g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[2], PointY[2]), new Point(PointX[3], PointY[3]));
                        g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[3], PointY[3]), new Point(PointX[0], PointY[0]));
                    }
                    g.DrawImage(ScaleImage(Image.FromFile(filep.Replace(".jpg", ".png").Replace("imagee", "ProcessedOut")), 1280 / 4, 720/4), new Point(960, 0));
                }
                Final.Save(filep.Replace("imagee", "final"));
                Final.Dispose();
                
            }
        }


        static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        static void RescaleImage(PointF TL, PointF TR, PointF LL, PointF LR, int sx, int sy,string filename)
        {
            var bmpOut = new Bitmap(sx, sy);

            for (int x = 0; x < sx; x++)
            {
                for (int y = 0; y < sy; y++)
                {
                    /*
                     * relative position
                     */
                    double rx = (double)x / sx;
                    double ry = (double)y / sy;

                    /*
                     * get top and bottom position
                     */
                    double topX = TL.X + rx * (TR.X - TL.X);
                    double topY = TL.Y + rx * (TR.Y - TL.Y);
                    double bottomX = LL.X + rx * (LR.X - LL.X);
                    double bottomY = LL.Y + rx * (LR.Y - LL.Y);

                    /*
                     * select center between top and bottom point
                     */
                    double centerX = topX + ry * (bottomX - topX);
                    double centerY = topY + ry * (bottomY - topY);

                    /*
                     * store result
                     */
                    var c = PolyColor(centerX, centerY);
                    bmpOut.SetPixel(x, y, c);
                }
            }

            bmpOut.Save(filename);
        }

        static Color PolyColor(double x, double y)
        {
            // get fractions
            double xf = x - (int)x;
            double yf = y - (int)y;

            // 4 colors - we're flipping sides so we can use the distance instead of inverting it later
            Color cTL = _Px[(int)y + 1, (int)x + 1];
            Color cTR = _Px[(int)y + 1, (int)x + 0];
            Color cLL = _Px[(int)y + 0, (int)x + 1];
            Color cLR = _Px[(int)y + 0, (int)x + 0];

            // 4 distances
            double dTL = Math.Sqrt(xf * xf + yf * yf);
            double dTR = Math.Sqrt((1 - xf) * (1 - xf) + yf * yf);
            double dLL = Math.Sqrt(xf * xf + (1 - yf) * (1 - yf));
            double dLR = Math.Sqrt((1 - xf) * (1 - xf) + (1 - yf) * (1 - yf));

            // 4 parts
            double factor = 1.0 / (dTL + dTR + dLL + dLR);
            dTL *= factor;
            dTR *= factor;
            dLL *= factor;
            dLR *= factor;

            // accumulate parts
            double r = dTL * cTL.R + dTR * cTR.R + dLL * cLL.R + dLR * cLR.R;
            double g = dTL * cTL.G + dTR * cTR.G + dLL * cLL.G + dLR * cLR.G;
            double b = dTL * cTL.B + dTR * cTR.B + dLL * cLL.B + dLR * cLR.B;

            Color c = Color.FromArgb((int)(r + 0.5), (int)(g + 0.5), (int)(b + 0.5));

            return c;
        }
    
    }
}
