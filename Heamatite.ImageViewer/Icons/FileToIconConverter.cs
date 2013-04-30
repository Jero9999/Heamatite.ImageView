using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Heamatite.Icons
{
	public class FileToIconConverter 
	{
		public FileToIconConverter()
		{

		}

		public ImageSource GetImage(string filename)
		{

			string extension = Path.GetExtension(filename).ToLower();
			string lookup = Directory.Exists(filename) ? filename : "aaa" + extension;
			return toBitmapSource(loadJumbo(lookup));

		}



		#region Win32api
		[System.Runtime.InteropServices.DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);
		#endregion

		#region Static Tools

		private static Bitmap resizeImage(Bitmap imgToResize, System.Drawing.Size size, int spacing)
		{
			int sourceWidth = imgToResize.Width;
			int sourceHeight = imgToResize.Height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)size.Width / (float)sourceWidth);
			nPercentH = ((float)size.Height / (float)sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;

			int destWidth = (int)((sourceWidth * nPercent) - spacing * 4);
			int destHeight = (int)((sourceHeight * nPercent) - spacing * 4);

			int leftOffset = (int)((size.Width - destWidth) / 2);
			int topOffset = (int)((size.Height - destHeight) / 2);


			Bitmap b = new Bitmap(size.Width, size.Height);
			Graphics g = Graphics.FromImage((Image)b);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
			g.DrawLines(System.Drawing.Pens.Silver, new PointF[] {
								new PointF(leftOffset - spacing, topOffset + destHeight + spacing), //BottomLeft
								new PointF(leftOffset - spacing, topOffset -spacing),                 //TopLeft
								new PointF(leftOffset + destWidth + spacing, topOffset - spacing)});//TopRight

			g.DrawLines(System.Drawing.Pens.Gray, new PointF[] {
								new PointF(leftOffset + destWidth + spacing, topOffset - spacing),  //TopRight
								new PointF(leftOffset + destWidth + spacing, topOffset + destHeight + spacing), //BottomRight
								new PointF(leftOffset - spacing, topOffset + destHeight + spacing),}); //BottomLeft

			g.DrawImage(imgToResize, leftOffset, topOffset, destWidth, destHeight);
			g.Dispose();

			return b;
		}

		private static Bitmap resizeJumbo(Bitmap imgToResize, System.Drawing.Size size, int spacing)
		{
			int sourceWidth = imgToResize.Width;
			int sourceHeight = imgToResize.Height;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)size.Width / (float)sourceWidth);
			nPercentH = ((float)size.Height / (float)sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;

			int destWidth = 80;
			int destHeight = 80;

			int leftOffset = (int)((size.Width - destWidth) / 2);
			int topOffset = (int)((size.Height - destHeight) / 2);


			Bitmap b = new Bitmap(size.Width, size.Height);
			Graphics g = Graphics.FromImage((Image)b);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
			g.DrawLines(System.Drawing.Pens.Silver, new PointF[] {
								new PointF(0 + spacing, size.Height - spacing), //BottomLeft
								new PointF(0 + spacing, 0 + spacing),                 //TopLeft
								new PointF(size.Width - spacing, 0 + spacing)});//TopRight

			g.DrawLines(System.Drawing.Pens.Gray, new PointF[] {
								new PointF(size.Width - spacing, 0 + spacing),  //TopRight
								new PointF(size.Width - spacing, size.Height - spacing), //BottomRight
								new PointF(0 + spacing, size.Height - spacing)}); //BottomLeft

			g.DrawImage(imgToResize, leftOffset, topOffset, destWidth, destHeight);
			g.Dispose();

			return b;
		}


		private static BitmapSource toBitmapSource(Bitmap source)
		{
			IntPtr hBitmap = source.GetHbitmap();
			//Memory Leak fixes, for more info : http://social.msdn.microsoft.com/forums/en-US/wpf/thread/edcf2482-b931-4939-9415-15b3515ddac6/
			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
					 BitmapSizeOptions.FromEmptyOptions());
			}
			finally
			{
				DeleteObject(hBitmap);
			}

		}

		private static bool isFolder(string path)
		{
			return Directory.Exists(path);
		}

		
		#endregion

		private static SysImageList _imgList = new SysImageList(SysImageListSize.jumbo);

		private Bitmap loadJumbo(string lookup)
		{
			_imgList.ImageListSize = isVistaUp() ? SysImageListSize.jumbo : SysImageListSize.extraLargeIcons;
			Icon icon = _imgList.Icon(_imgList.IconIndex(lookup, isFolder(lookup)));
			Bitmap bitmap = icon.ToBitmap();
			icon.Dispose();

			System.Drawing.Color empty = System.Drawing.Color.FromArgb(0, 0, 0, 0);

			if (bitmap.Width < 256)
				bitmap = resizeImage(bitmap, new System.Drawing.Size(256, 256), 0);
			else
				if (bitmap.GetPixel(100, 100) == empty && bitmap.GetPixel(200, 200) == empty && bitmap.GetPixel(200, 200) == empty)
				{
					_imgList.ImageListSize = SysImageListSize.largeIcons;
					bitmap = resizeJumbo(_imgList.Icon(_imgList.IconIndex(lookup)).ToBitmap(), new System.Drawing.Size(200, 200), 5);
				}

			return bitmap;
		}

		
		#region Instance Tools

		public static bool isVistaUp()
		{
			return (Environment.OSVersion.Version.Major >= 6);
		}
		#endregion
	}
}
