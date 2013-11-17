using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace LicencePlate
{
    class Program
    {
        static Color[,] _Px = new Color[720,1280];

        static bool IsYellow(Color Pix)
        {
            if (Pix.B >100 && Pix.B == Pix.G)
            {
                return true;
            }
            return false;
        }
        static int[] PointXX = new int[4];
        static int[] PointYY = new int[4];
        static int BiggestQ = 0;
        static string filepG = "";
        static string CurrentPxArrayFile = "";
        static int HighestX = 0;
        static int LowestX = 0;
        static int HighestY = 0;
        static int LowestY = 0;


        static void Main(string[] args)
        {
            //string[] filePaths = Directory.GetFiles(@"D:\video\", "*.jpg");
            int Number_of_frames = 518;
            string RootDir = @"D:\Vid\Vid\";
            string FilePrefix = "imagee";
            //foreach (string filep in filePaths)
            for (int n = 3; n < Number_of_frames; n=n+3)
            {
                string filep = RootDir + FilePrefix + n + ".jpg";
                filepG = filep;
                // We need to find the points of the licence plate...
                int[] PointX = new int[4];
                int[] PointY = new int[4];
                bool SeenFirstBitOfYellow = false;
                Console.WriteLine("Processing {0}", filep);
                Bitmap Orig = (Bitmap)ScaleImage(Image.FromFile(filep), 640, 360);
                //Bitmap Orig = (Bitmap)Bitmap.FromFile(filep);
                int imageoffset = 0;


                //Process the image to Yellow Only
                for (int y = 0; y < Orig.Height; y++)
                {
                    for (int x = 0; x < Orig.Width; x++)
                    {
                        Color Pix = Orig.GetPixel(x,y);
                        int BWVal = 0;
                        if (!(((Pix.R + Pix.G) / 2) < Pix.B) && (Pix.GetHue() < 65 && Pix.GetHue() > 35) )
                        {
                            BWVal = ((Pix.R + Pix.G) / 2) - Pix.B;
                            if (BWVal < 50)
                            {
                                BWVal = 0;
                            }
                        }

                        Orig.SetPixel(x,y,Color.FromArgb(BWVal,BWVal,BWVal));
                    }
                }


                Bitmap Final = new Bitmap(1280, 720); // This is the final image that will be saved.
                bool FirstProcess = true;
                for (int y = 0; y < Orig.Height; y++)
                {
                    for (int x = 0; x < Orig.Width; x++)
                    {
                        if (!SeenFirstBitOfYellow)
                        {
                            Color Pix = Orig.GetPixel(x, y);
                            if (IsYellow(Pix))
                            {
                                HighestX = 0;
                                LowestX = Orig.Width;
                                HighestY = 0;
                                LowestY = Orig.Height;

                                BiggestQ = 0;
                                Orig = FloodFill(Orig, new Point(x, y), Color.Black, Color.Orange);
                                if (BiggestQ > 10)
                                {
                                    for (int b = 0; b < 4; b++) { PointX[b] = 0; PointY[b] = 0; } // Clear the old cords.
                                    Console.WriteLine("{0}-{1}    {2}-{3}", HighestX, LowestX, HighestY, LowestY);

                                    int xx = (HighestX + LowestX) / 2;
                                    int yy = (HighestY + LowestY) / 2;
                                    int maxy = 0;

                                    try
                                    {

                                        while (true)
                                        {

                                            Color ppp = Orig.GetPixel(xx - 1, yy);
                                            if (ppp.R == 255 && ppp.G == 165)
                                            {

                                                PointX[0] = xx;
                                                // Ok so this is a good X. Lets Find the higest bit.
                                                for (int xy = yy; xy > 0; xy--)
                                                {
                                                    if (Orig.GetPixel(xx, xy).R == 255 && Orig.GetPixel(xx, xy).G == 165 && xy > maxy)
                                                        maxy = xy;

                                                }
                                            }
                                            //Console.WriteLine(maxy);
                                            xx--;
                                            PointY[0] = maxy;
                                            if (0 >= xx)
                                                break;
                                            //Orig.SetPixel(xx, yy, Color.Olive);
                                        }
                                        int xt = PointX[0] + 10;
                                        maxy = 0;
                                        for (int xy = yy; xy >LowestY; xy--)
                                        {
                                            if (Orig.GetPixel(xt, xy).R != Orig.GetPixel(xt, xy).B)
                                                maxy = xy;
                                            //Orig.SetPixel(xt, xy, Color.PaleGreen);
                                        }
                                        PointY[0] = maxy;

                                        PointX[1] = PointX[0];

                                        maxy = 0;
                                        for (int xy = yy; xy < HighestY; xy++)
                                        {
                                            if (Orig.GetPixel(xt, xy).R != Orig.GetPixel(xt, xy).B)
                                                maxy = xy;
                                            //Orig.SetPixel(xt, xy, Color.PaleGreen);
                                        }
                                        PointY[1] = maxy;

                                        xx = (HighestX + LowestX) / 2;
                                        maxy = 0;
                                        yy = (HighestY + LowestY) / 2;
                                        while (true)
                                        {

                                            Color ppp = Orig.GetPixel(xx + 1, yy);
                                            if (ppp.R == 255 && ppp.G == 165)
                                            {
                                                PointX[2] = xx;
                                                PointX[3] = xx;
                                                // Ok so this is a good X. Lets Find the higest bit.
                                                Orig.SetPixel(xx, yy, Color.Olive);
                                            }
                                            //Console.WriteLine(xx);
                                            xx++;
                                            if (xx >= HighestX) 
                                                break;
                                        }

                                        xt = PointX[2] - 10;
                                        maxy = 0;
                                        for (int xy = yy; xy > LowestY; xy--)
                                        {
                                            if (Orig.GetPixel(xt, xy).R != Orig.GetPixel(xt, xy).B)
                                                maxy = xy;
                                            //Orig.SetPixel(xt, xy, Color.PaleGreen);
                                        }
                                        PointY[2] = maxy;

                                        xt = PointX[2] - 10;
                                        maxy = 0;
                                        for (int xy = yy; xy < HighestY; xy++)
                                        {
                                            if (Orig.GetPixel(xt, xy).R != Orig.GetPixel(xt, xy).B)
                                                maxy = xy;
                                            //Orig.SetPixel(xt, xy, Color.PaleGreen);
                                        }
                                        PointY[3] = maxy;

                                        Console.WriteLine("X{0} Y{1}", PointX[0], PointY[0]);
                                        Orig.SetPixel(PointX[0], PointY[0], Color.Red);
                                        Orig.SetPixel(PointX[1], PointY[1], Color.Red);
                                        Orig.SetPixel(PointX[2], PointY[2], Color.Red);
                                        Orig.SetPixel(PointX[3], PointY[3], Color.Red);
                                        //SeenFirstBitOfYellow = true; //Comment in to only select 1st find.
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Failed LP.");
                                    }
                                    //Console.ReadLine();
                                    for (int b = 0; b < 4; b++) { PointX[b] = PointX[b] * 2; PointY[b] = PointY[b] * 2; } // Clear the old cords.
                                        
                                    if (PointX[3] != 0)
                                        RescaleImage(new PointF(PointX[0], PointY[0]), new PointF(PointX[2], PointY[2]), new PointF(PointX[1], PointY[1]), new PointF(PointX[3], PointY[3]), 600, 90, filep.Replace("imagee", "LP"));
                                    
                                    using (Graphics g = Graphics.FromImage(Final))
                                    {
                                        if (FirstProcess)
                                        {
                                            Image newImage = Image.FromFile(filep);
                                            g.DrawImage(newImage, 0, 0);
                                            FirstProcess = false;
                                        }

                                        if (AreAllSet(PointX,PointY))
                                        {
                                            try
                                            {
                                                Image newImage2 = Image.FromFile(filep.Replace("imagee", "LP"));
                                                g.DrawImage(newImage2, 0, 90 * imageoffset);
                                                newImage2.Dispose();
                                                File.Delete(filep.Replace("imagee", "LP"));
                                                imageoffset++;
                                            }
                                            catch
                                            {
                                                // I need to find out more on what to do about this.
                                            }
                                        }

                                        //g.DrawImage(ScaleImage(Image.FromFile(filep.Replace(".jpg", ".png").Replace("imagee", "ProcessedOut")), 1280 / 4, 720 / 4), new Point(960, 0));
                                    }

                                }
                                else
                                {
                                    Orig = FloodFill(Orig, new Point(x, y), Color.Orange, Color.Red);
                                }
                            }
                        }
                    }
                }


                Orig.Save(filep.Replace(".jpg", ".png").Replace("imagee", "ProcessedOut"),System.Drawing.Imaging.ImageFormat.Png);
                using (Graphics g = Graphics.FromImage(Final))
                {
                    if (FirstProcess)
                    {
                        Image newImage = Image.FromFile(filep);
                        g.DrawImage(newImage, 0, 0);
                        FirstProcess = false;
                    }
                    g.DrawImage(ScaleImage(Image.FromFile(filep.Replace(".jpg", ".png").Replace("imagee", "ProcessedOut")), 1280 / 4, 720 / 4), new Point(960, 0));
                }
                //Console.Read();

                // And now to Combine all the images together


                Console.WriteLine("Saving Final Image...");
                Orig.Dispose();
                Final.Save(filep.Replace("imagee", "final"));
                Final.Dispose();
                
            }
        }

        static void DrawBox(int[] PointX, int[] PointY, Graphics g)
        {
            g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[0], PointY[0]), new Point(PointX[1], PointY[1]));
            g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[1], PointY[1]), new Point(PointX[2], PointY[2]));
            g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[2], PointY[2]), new Point(PointX[3], PointY[3]));
            g.DrawLine(new Pen(Color.Red, 5), new Point(PointX[3], PointY[3]), new Point(PointX[0], PointY[0]));
        }

        static bool AreAllSet(int[] ar1, int[] ar2)
        {
            foreach (int test in ar1)
            {
                if (test == 0)
                {
                    return false;
                }
            }
            foreach (int test in ar2)
            {
                if (test == 0)
                {
                    return false;
                }
            }
            return true;
        }


        /*
         * Holy hell, this one really is a cluster fuck!
         * I have no idea how this works and this needs to be looked at since its a rather critical part to the
         * program's operation.
         */
        static Bitmap FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Queue<Point> q = new Queue<Point>();
            int LargestQ = 0;
            q.Enqueue(pt);
            while (q.Count > 0)
            {
                Point n = q.Dequeue();
                if (!(bmp.GetPixel(n.X, n.Y) != replacementColor || bmp.GetPixel(n.X, n.Y).B != 0)) //!(bmp.GetPixel(n.X, n.Y) != Color.Black) && !
                    continue;
                //Console.WriteLine("Going to fill {0} points", q.Count());
                if (LargestQ < q.Count) { LargestQ = q.Count; }
                if (HighestX < n.X) {HighestX = n.X;} 
                else if (LowestX > n.X) {LowestX = n.X;}
                if(HighestY < n.Y){ HighestY = n.Y; }
                else if (LowestY > n.Y) { LowestY = n.Y; }

                Point w = n, e = new Point(n.X + 1, n.Y);
                while ((w.X > 0) && ((bmp.GetPixel(w.X, w.Y).B != 0)) && ((bmp.GetPixel(w.X, w.Y) != replacementColor)))
                {
                    bmp.SetPixel(w.X, w.Y, replacementColor);
                    if ((w.Y > 0) && (bmp.GetPixel(w.X, w.Y - 1).B != 0) && (bmp.GetPixel(w.X, w.Y - 1) != replacementColor))
                        q.Enqueue(new Point(w.X, w.Y - 1));
                    if ((w.Y < bmp.Height - 1) && ((bmp.GetPixel(w.X, w.Y + 1).B != 0)) && (bmp.GetPixel(w.X, w.Y + 1) != replacementColor))
                        q.Enqueue(new Point(w.X, w.Y + 1));
                    w.X--;
                }
                while ((e.X < bmp.Width - 1) && ((bmp.GetPixel(e.X, e.Y).B != 0) && (bmp.GetPixel(e.X, e.Y) != replacementColor)))
                {
                    bmp.SetPixel(e.X, e.Y, replacementColor);
                    if ((e.Y > 0) && ((bmp.GetPixel(e.X, e.Y - 1).B != 0)) && (bmp.GetPixel(e.X, e.Y - 1) != replacementColor))
                        q.Enqueue(new Point(e.X, e.Y - 1));
                    if ((e.Y < bmp.Height - 1) && ((bmp.GetPixel(e.X, e.Y + 1).B != 0)) && (bmp.GetPixel(e.X, e.Y + 1) != replacementColor))
                        q.Enqueue(new Point(e.X, e.Y + 1));
                    e.X++;
                }
            }
            Console.WriteLine("Largest Q = " +LargestQ);
            BiggestQ = LargestQ;
            return bmp;
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
            //static Color[,] _Px = new Color[720,1280];
            Bitmap import = (Bitmap)Image.FromFile(filepG);
            if (CurrentPxArrayFile != filepG)
            {
                Console.WriteLine("Exporting into array...");
                for (int x = 0; x < import.Width; x++)
                {
                    for (int y = 0; y < import.Height; y++)
                    {
                        _Px[y, x] = import.GetPixel(x, y);
                    }
                }
                CurrentPxArrayFile = filepG;
            }
            import.Dispose();
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
            // fractions
            double xf = x - (int)x;
            double yf = y - (int)y;

            // 4 colors - we're flipping sides
            // use the distance instead of inverting
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

            // smashing parts
            double r = dTL * cTL.R + dTR * cTR.R + dLL * cLL.R + dLR * cLR.R;
            double g = dTL * cTL.G + dTR * cTR.G + dLL * cLL.G + dLR * cLR.G;
            double b = dTL * cTL.B + dTR * cTR.B + dLL * cLL.B + dLR * cLR.B;

            Color c = Color.FromArgb((int)(r + 0.5), (int)(g + 0.5), (int)(b + 0.5));

            return c;
        }
    
    }
}
