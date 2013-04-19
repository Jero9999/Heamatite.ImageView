using System;
using System.Collections.Generic;
using System.IO;
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
	}

	public interface IImageFile
	{
		Stream GetStream();

		string Name { get; }

		bool IsImage { get; }

		BitmapImage Bitmap { get; }
	}

	public class ImageDirectory : IImageDirectory
	{
		Directory Directory;
		File[] Files;
		private int FileIndex;

		public ImageDirectory(string directoryPath)
			: this(new Directory(directoryPath))
		{
		}

		public ImageDirectory(Directory directory)
		{
			Directory = directory;
			IoType imageFileType = IoType.File | IoType.Image;
			Files = directory.GetFiles().Where(c=>(c.IoType & imageFileType) == imageFileType ).Cast<File>().ToArray();
			FileIndex = 0;
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
			File file = Files[FileIndex];
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
				IImageFile currentFile = Current();
				if (!currentFile.IsImage)
				{
					continue;
				}
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
				IImageFile currentFile = Current();
				if (!currentFile.IsImage)
				{
					continue;
				}
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
	}

	class ImageFile : IImageFile
	{
		private File File;

		public ImageFile(File file)
		{
			File = file;
		}

		public Stream GetStream()
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

		private static BitmapImage GetBitmap(Stream fileSteam)
		{
			BitmapImage myBitmapImage = new BitmapImage();

			// BitmapImage.UriSource must be in a BeginInit/EndInit block
			myBitmapImage.BeginInit();
			myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
			myBitmapImage.StreamSource = fileSteam;

			myBitmapImage.EndInit();
			return myBitmapImage;
		}

		public bool IsImage
		{
			get
			{
				return File.IoType.HasFlag(IoType.Image);
			}
		}
	}
}
