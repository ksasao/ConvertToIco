using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertToIco
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string srcfile = args[0];
                string destfile = Path.Combine(
                    Path.GetDirectoryName(srcfile),
                    Path.GetFileNameWithoutExtension(srcfile) + ".ico");
                ConvertToIco(srcfile, destfile);
            }
            catch(Exception ex)
            {
                Console.WriteLine("usage: converttoico [png file]");
                Console.WriteLine(ex.Message);
            }
        }

        private static void ConvertToIco(string srcfile, string destfile)
        {
            int[] sizelist = new[] { 256, 192, 128, 96, 64, 48, 40, 32, 24, 16 };
            Bitmap[] bitmap = new Bitmap[sizelist.Length];
            Bitmap src = new Bitmap(srcfile);

            for (int i = 0; i < sizelist.Length; i++)
            {
                bitmap[i] = new Bitmap(sizelist[i], sizelist[i], System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bitmap[i]))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(src,
                        new Rectangle(0, 0, bitmap[i].Width, bitmap[i].Height),
                        new Rectangle(0, 0, src.Width, src.Height), GraphicsUnit.Pixel);
                }

                // Convert to PNG Image object
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap[i].Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    bitmap[i].Dispose();
                    ms.Seek(0, SeekOrigin.Begin);
                    bitmap[i] = new Bitmap(ms);
                }
            }
            using (var stream = new FileStream(destfile, FileMode.Create))
            {
                IconFactory.SavePngsAsIcon(bitmap, stream);
            }

            // Dispose Resorces
            for (int i = 0; i < sizelist.Length; i++)
            {
                bitmap[i].Dispose();
            }
            src.Dispose();
        }
    }
}
