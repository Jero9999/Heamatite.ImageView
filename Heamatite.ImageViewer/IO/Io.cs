using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IO
{
	[Flags]
	public enum IoType
	{
		File = 1,
		Directory = 2,
		Image = 4,
		Zip = 8
	}

	public abstract class Io : INotifyPropertyChanged
	{
		public Io()
		{

		}

		public Io(System.IO.FileSystemInfo fsi)
		{
			_Name = fsi.Name;
			_FullName = fsi.FullName;
		}

		public static Io Create(System.IO.FileSystemInfo fsi)
		{
			if (fsi.Attributes.HasFlag(System.IO.FileAttributes.Directory))
			{
				return new Directory(fsi as System.IO.DirectoryInfo);
			}
			else if (fsi.Extension == ".zip")
			{
				return new ZipDirectory(fsi as FileInfo);
			}
			else
			{
				return new File(fsi as System.IO.FileInfo);
			}
		}

		public abstract IoType IoType { get; }

		private string _Name;
		public virtual string Name { get { return _Name; } set { _Name = value; NotifyChange("Name"); } }
		private string _IconImage;
		public string IconImage { get { return _IconImage; } set { _IconImage = value; NotifyChange("IconImage"); } }
		private string _FullName;
		public virtual string FullName { get { return _FullName; } set { _FullName = value; NotifyChange("FullName"); } }

		private void NotifyChange(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;


		public static Io Create(string location)
		{
			var directoryPath = Path.GetDirectoryName(location);
			if (directoryPath == null && Path.GetPathRoot(location) == location)
			{
				//umm could it be the root?
				directoryPath = location;
				return Create(new DirectoryInfo(directoryPath));
			}

			var fileName = Path.GetFileName(location);
			if (fileName == null)
			{
				fileName = directoryPath;
			}
			DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);
			FileSystemInfo fsi = dirInfo.GetFileSystemInfos(fileName).FirstOrDefault();
			return fsi == null ? null : Create(fsi);
		}
	}


	public class Directory : Io
	{
		protected DirectoryInfo internalDir;

		public Directory(System.IO.DirectoryInfo dir)
			: base(dir)
		{
			internalDir = dir;
		}

		public Directory(string directoryPath)
			: this(new DirectoryInfo(directoryPath))
		{
			
		}

		internal Directory()
		{
		}

		public override IoType IoType { get { return IoType.Directory; } }

		public virtual Io[] GetFiles()
		{
			return internalDir.GetFiles().Select(c => Io.Create(c)).ToArray();
		}

		public virtual Io[] GetContents()
		{
			return internalDir.GetFileSystemInfos().Select(c => Io.Create(c)).ToArray();
		}
	}

	public class ZipDirectory : Directory, IDisposable
	{
		private ZipFile ZipFile;
		public ZipDirectory(ZipFile zipFile)
			: base()
		{
			ZipFile = zipFile;
			string tempDir = GetTempDirectory();
			foreach (var zipEntry in ZipFile)
			{
				zipEntry.Extract(tempDir);
			}
			base.internalDir = new DirectoryInfo(tempDir);
		}

		private static string GetTempDirectory()
		{
			string tempPath = Path.GetTempPath();
			var ourTempDir = new DirectoryInfo(Path.Combine(tempPath, "Heamatite.ImageViewer"));
			if (!ourTempDir.Exists) { ourTempDir.Create(); }
			
			string tempDir = Path.GetTempFileName();
			System.IO.File.Delete(tempDir);
			System.IO.Directory.CreateDirectory(tempDir);
			return ourTempDir.CreateSubdirectory(Path.GetFileName(tempDir)).FullName;
		}

		public ZipDirectory(FileInfo theFile) : this(new ZipFile(theFile.FullName))
		{
		}

		public override IoType IoType { get { return IoType.Directory | IO.IoType.File | IO.IoType.Zip; } }
		public override string Name
		{
			get
			{
				return Path.GetFileName(FullName);
			}
		}

		public override string FullName
		{
			get
			{
				return ZipFile.Name;
			}
		}

		public override Io[] GetContents()
		{
			return internalDir.GetFileSystemInfos().Select(c => Create(c)).ToArray();
		}

		public new  Io Create(System.IO.FileSystemInfo fsi)
		{
			if (fsi.Attributes.HasFlag(System.IO.FileAttributes.Directory))
			{
				return new ZipContentDirectory(fsi as System.IO.DirectoryInfo, this);
			}
			else if (fsi.Extension == ".zip")
			{
				return new ZipDirectory(fsi as FileInfo);
			}
			else
			{
				return new ZipContentFile(fsi as System.IO.FileInfo, this);
			}
		}


		public void Dispose()
		{
			//if (internalDir.Exists) internalDir.Delete(true);
		}

		~ZipDirectory()
		{
			if (internalDir.Exists) internalDir.Delete(true);
		}
	}

	public class File : Io
	{
		FileInfo internalFile;

		public File() { }
		public File(System.IO.FileInfo file) : base(file) { internalFile = file; }
		public override IoType IoType
		{
			get { return IoType.File | (IsImage() ? IoType.Image : 0); }
		}


		static List<string> ImageExtensions = new List<string>()
		{
			".jpg",
			".gif",
			".png"
		};

		bool IsImage()
		{
			var extension = Path.GetExtension(this.FullName);
			return ImageExtensions.Contains(extension.ToLower());
		}


		internal Stream OpenRead()
		{
			return internalFile.OpenRead();
		}

	}

	public class ZipContentFile : File
	{
		public ZipContentFile(FileInfo file, ZipDirectory parent)
			: base(file)
		{
			ParentZip = parent;
		}

		protected ZipDirectory ParentZip;
	}

	public class ZipContentDirectory : Directory
	{
		public ZipContentDirectory(DirectoryInfo dir, ZipDirectory parent)
			: base(dir)
		{
			ParentZip = parent;
		}

		protected ZipDirectory ParentZip;
	}
}
