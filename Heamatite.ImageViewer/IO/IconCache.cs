using Heamatite.IoSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using System.Windows.Interop;

namespace Heamatite.IO
{
	public static class IconCache
	{
		static ImageSource DirectoryImage
		{
			get
			{
				return (BitmapImage)Heamatite.ImageViewer.App.Current.FindResource("DirectoryImage");
			}
		}

		public static ImageSource GetIconImage(IFileSystemObject fileSystemObject)
		{
			if (fileSystemObject is IDirectoryObject)
			{
				return DirectoryImage;
			}

			IFileObject fileObject = fileSystemObject as IFileObject;
			if (fileObject == null)
			{
				//WTF? not a directory and not a file???
				return null;
			}

			if (ImageDirectory.IsImage(fileObject))
			{
				return GenerateBitmap(fileObject);
			}
			else
			{
				return FileManager.GetImageSource(fileObject.FullName, 100);
			}
		}

		private static BitmapImage GenerateBitmap(IFileObject fileObject)
		{
			using (var stream = fileObject.OpenRead())
			{
				return GetBitmap(stream);
			}
		}
		private static BitmapImage GetBitmap(System.IO.Stream fileSteam)
		{
			BitmapImage myBitmapImage = new BitmapImage();

			// BitmapImage.UriSource must be in a BeginInit/EndInit block
			myBitmapImage.BeginInit();
			myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			myBitmapImage.StreamSource = fileSteam;
			myBitmapImage.DecodePixelHeight = 100;
			myBitmapImage.DecodePixelWidth = 100;
			myBitmapImage.EndInit();
			myBitmapImage.Freeze();
			return myBitmapImage;
		}
	}

}
