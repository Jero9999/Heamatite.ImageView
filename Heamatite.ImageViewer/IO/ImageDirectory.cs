using Heamatite.IoSystem;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Heamatite.IO
{
	public interface IImageDirectory
	{
		IImageFile Next();
		IImageFile Previous();
		IImageFile Current();
		IImageFile First();
		IImageFile Last();

		void SetCurrent(string fileName);

		string FullName { get; set; }
		IList<object> Contents { get; set; }
	}

	public interface IImageFile
	{
		System.IO.Stream GetStream();

		string Name { get; }

		BitmapImage Bitmap { get; }
	}

	public class ImageDirectory : IImageDirectory
	{
		IDirectoryObject Directory;
		IFileObject[] Files;
		private int FileIndex;

		public ImageDirectory(IDirectoryObject directory)
		{
			Directory = directory;
			Files = directory.GetFiles().Where(c => IsImage(c)).Cast<IFileObject>().ToArray();
			FileIndex = 0;
		}

		private static string[] _ImageFileExtensions = new string[]
		{
			".jpg",
			".gif",
			".png"
		};

		private static bool IsImage(IFileObject file)
		{
			return _ImageFileExtensions.Contains(file.Extension);
		}

		public IImageFile Next()
		{
			FileIndex++;
			FileIndex = FileIndex % (Files.Length);
			return Current();
		}

		public IImageFile Previous()
		{
			FileIndex--;
			FileIndex = FileIndex < 0 ? 0 : FileIndex;
			return Current();
		}

		public IImageFile Current()
		{
			IFileObject file = Files[FileIndex];
			return new ImageFile(file);
		}

		public IImageFile First()
		{
			FileIndex = 0;
			return Current();
		}

		public IImageFile Last()
		{
			FileIndex = Files.Length - 1;
			return Current();
		}

		private void MoveToNextValidImage(int lastGoodIndex)
		{
			bool foundGoodImage = false;
			for (int i = FileIndex; i < Files.Length; i++)
			{
				IFileObject currentFile = Files[FileIndex];
				if (!IsImage(currentFile)) { continue; }
				foundGoodImage = true;
				FileIndex = i;
			}
			if (!foundGoodImage) FileIndex = lastGoodIndex;
		}

		private void MoveToPreviousValidImage(int lastGoodIndex)
		{
			bool foundGoodImage = false;
			for (int i = FileIndex; i >= 0; i++)
			{
				IFileObject currentFile = Files[FileIndex];
				if (!IsImage(currentFile)) { continue; }
				foundGoodImage = true;
				FileIndex = i;
			}
			if (!foundGoodImage) FileIndex = lastGoodIndex;
		}

		public void SetCurrent(string fileName)
		{
			for (int i = 0; i < Files.Length; i++)
			{
				if (Files[i].Name == fileName)
				{
					FileIndex = i;
					return;
				}
			}
		}

		public string FullName
		{
			get { return this.Directory.FullName; }
			set { }
		}

		public IList<object> Contents
		{
			get { return this.Directory.GetContents().Cast<object>().ToList(); }
			set { }
		}
	}

	class ImageFile : IImageFile
	{
		private IFileObject File;

		public ImageFile(IFileObject file)
		{
			File = file;
		}

		public System.IO.Stream GetStream()
		{
			return File.OpenRead();
		}

		public string Name
		{
			get { return File.Name; }
		}

		public BitmapImage Bitmap
		{
			get
			{
				using (var stream = GetStream())
				{
					return GetBitmap(stream);
				}
			}
		}

		private static BitmapImage GetBitmap(System.IO.Stream fileSteam)
		{
			BitmapImage myBitmapImage = new BitmapImage();

			// BitmapImage.UriSource must be in a BeginInit/EndInit block
			myBitmapImage.BeginInit();
			myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			myBitmapImage.StreamSource = fileSteam;

			myBitmapImage.EndInit();
			return myBitmapImage;
		}
	}
}
