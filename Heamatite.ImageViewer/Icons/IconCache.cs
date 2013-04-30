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
using Heamatite.IO;
using System.Runtime.Caching;

namespace Heamatite.Icons
{
	public class IconCache
	{
		static IconCache _Instance;
		public static IconCache Instance
		{
			get
			{
				_Instance = _Instance == null ? new IconCache() : _Instance;
				return _Instance;
			}
		}
		static MemoryCache _cache = new MemoryCache("IconCache");
		FileToIconConverter _Converter = new FileToIconConverter();

		static ImageSource DirectoryImage
		{
			get
			{
				return (BitmapImage)Heamatite.ImageViewer.App.Current.FindResource("DirectoryImage");
			}
		}

		public ImageSource GetIconImage(IFileSystemObject fileSystemObject)
		{
			System.Diagnostics.Debug.WriteLine("Cache get icon image");

			if (_cache[fileSystemObject.FullName] != null)
			{
				System.Diagnostics.Debug.WriteLine("Cache HIT: " + fileSystemObject.FullName);
				return _cache[fileSystemObject.FullName] as ImageSource;
			}
			ImageSource bitmap = null;
			if (fileSystemObject is IDirectoryObject)
			{
				bitmap = _Converter.GetImage(fileSystemObject.FullName);
			}
			else if (ImageDirectory.IsImage(fileSystemObject as IFileObject))
			{
				bitmap = GenerateBitmap(fileSystemObject as IFileObject);
			}
			else
			{
				bitmap = _Converter.GetImage(fileSystemObject.FullName);
			}

			System.Diagnostics.Debug.WriteLine("Cache add: " + fileSystemObject.FullName);
			_cache.Add(new CacheItem(fileSystemObject.FullName, bitmap), 
				new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(10) });

			bitmap.Freeze();
			return bitmap;
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
