#define xDISPLAY_DEBUG 
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
			if (_cache[fileSystemObject.FullName] != null)
			{
#if DISPLAY_DEBUG
				System.Diagnostics.Debug.WriteLine("Cache HIT: " + fileSystemObject.FullName);
#endif
				return _cache[fileSystemObject.FullName] as ImageSource;
			}

			ImageSource bitmap = ReadFromDisk(fileSystemObject);

			AddToCache(fileSystemObject, bitmap);

			return bitmap;
		}

		private void AddToCache(IFileSystemObject fileSystemObject, ImageSource bitmap)
		{
#if DISPLAY_DEBUG
			System.Diagnostics.Debug.WriteLine("Cache add: " + fileSystemObject.FullName);
#endif
			_cache.Add(new CacheItem(fileSystemObject.FullName, bitmap),
				new CacheItemPolicy() { SlidingExpiration = TimeSpan.FromMinutes(10) });
		}

		static object locker = new object();
		private ImageSource ReadFromDisk(IFileSystemObject fileSystemObject)
		{
			ImageSource bitmap = null;
			//lock (locker)
			{
#if DISPLAY_DEBUG
				System.Diagnostics.Debug.WriteLine("Cache read from disk " + fileSystemObject.Name);
#endif
				if (fileSystemObject is IDirectoryObject)
				{
					bitmap = _Converter.GetImage(fileSystemObject.FullName);
				}
				else if (ImageDirectory.IsImage(fileSystemObject as IFileObject))
				{
					using (var stream = (fileSystemObject as IFileObject).OpenRead())
					{
						bitmap = GetBitmap(stream);
					}
				}
				else
				{
					bitmap = _Converter.GetImage(fileSystemObject.FullName);
				}
				bitmap.Freeze();
			}
			return bitmap;
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

	public class IconCacheQueue
	{
		Queue<ActionQueueItem> ActionQueue = new Queue<ActionQueueItem>(1000);

		static Object lockObject = new Object();
		bool _QueueIsRunning = false;
		bool QueueIsRunning
		{
			get { lock (lockObject) { return _QueueIsRunning; } }
			set { lock (lockObject) { _QueueIsRunning = value; } }
		}

		public void ClearQueue()
		{
			lock (lockObject)
			{
				ActionQueue.Clear();
			}
		}

		private void RunQueue()
		{
			if (QueueIsRunning) { return; }
			QueueIsRunning = true;

			Task.Run(() =>
			{
				while (ActionQueue.Count > 0)
				{
					ActionQueueItem item;
					lock (lockObject)
					{
						item = ActionQueue.Dequeue();
					}
					ProcessQueueItem(item);
				}
				QueueIsRunning = false;
			});
		}

		public void GetIconImageAsync(IFileSystemObject fileSystemObject, Action<ImageSource> returnAction)
		{
			lock (lockObject)
			{
				ActionQueue.Enqueue(new ActionQueueItem()
				{
					FileSystemObject = fileSystemObject,
					ReturnAction = returnAction
				}
				);
			}
			RunQueue();
		}

		private void ProcessQueueItem(ActionQueueItem item)
		{
			ImageSource image = IconCache.Instance.GetIconImage(item.FileSystemObject);
			item.ReturnAction(image);
		}

		struct ActionQueueItem
		{
			public IFileSystemObject FileSystemObject;
			public Action<ImageSource> ReturnAction;
		}
	}

}
