using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IoSystem.Zip
{
	using System.Text.RegularExpressions;
	using path = System.IO.Path;

	internal class ZipFileObject : FileObject, IDirectoryObject, IDisposable
	{
		internal ZipFile _ZipFile;

		public ZipFileObject(System.IO.FileInfo file, IIoRepository repository)
			: base(file, repository)
		{
			_ZipFile =  new ZipFile(file.FullName);
			
		}

		#region IDirectoryInfo implementation
		public IList<IFileSystemObject> GetContents()
		{
			return _ZipFile.Entries.Select(c => 
				c.IsDirectory ? 
				(IFileSystemObject)new ZipEntryDirectoryObject(c, _ZipFile) : 
				(IFileSystemObject)new ZipEntryFileObject(c))
				.ToList();
		}

		public IList<IDirectoryObject> GetDirectories()
		{
			var result = from c in GetContents()
									 where c is ZipEntryDirectoryObject
									 select c as IDirectoryObject;
			return result.ToList();
		}

		public IList<IFileObject> GetFiles()
		{
			var result = from c in GetContents()
									 where c is ZipEntryFileObject
									 select c as IFileObject;
			return result.ToList();
		}
		#endregion

		~ZipFileObject()
		{
			if (_ZipFile != null)
			{
				_ZipFile.Dispose();
				_ZipFile = null;
			}
		}

		public void Dispose()
		{
			_ZipFile.Dispose();
			_ZipFile = null;
		}
	}

	//public class ZipDirectory : Directory, IDisposable
	//{
	//	private ZipFile ZipFile;
	//	public ZipDirectory(ZipFile zipFile)
	//		: base()
	//	{
	//		ZipFile = zipFile;
	//		string tempDir = GetTempDirectory();
	//		foreach (var zipEntry in ZipFile)
	//		{
	//			zipEntry.Extract(tempDir);
	//		}
	//		base.internalDir = new DirectoryInfo(tempDir);
	//	}

	//	private static string GetTempDirectory()
	//	{
	//		string tempPath = Path.GetTempPath();
	//		var ourTempDir = new DirectoryInfo(Path.Combine(tempPath, "Heamatite.ImageViewer"));
	//		if (!ourTempDir.Exists) { ourTempDir.Create(); }

	//		string tempDir = Path.GetTempFileName();
	//		System.IO.File.Delete(tempDir);
	//		System.IO.Directory.CreateDirectory(tempDir);
	//		return ourTempDir.CreateSubdirectory(Path.GetFileName(tempDir)).FullName;
	//	}

	//	public ZipDirectory(FileInfo theFile)
	//		: this(new ZipFile(theFile.FullName))
	//	{
	//	}

	//	public override IoType IoType { get { return IoType.Directory | IO.IoType.File | IO.IoType.Zip; } }
	//	public override string Name
	//	{
	//		get
	//		{
	//			return Path.GetFileName(FullName);
	//		}
	//	}

	//	public override string FullName
	//	{
	//		get
	//		{
	//			return ZipFile.Name;
	//		}
	//	}

	//	public override Io[] GetContents()
	//	{
	//		return internalDir.GetFileSystemInfos().Select(c => Create(c)).ToArray();
	//	}

	//	public new Io Create(System.IO.FileSystemInfo fsi)
	//	{
	//		if (fsi.Attributes.HasFlag(System.IO.FileAttributes.Directory))
	//		{
	//			return new ZipContentDirectory(fsi as System.IO.DirectoryInfo, this);
	//		}
	//		else if (fsi.Extension == ".zip")
	//		{
	//			return new ZipDirectory(fsi as FileInfo);
	//		}
	//		else
	//		{
	//			return new ZipContentFile(fsi as System.IO.FileInfo, this);
	//		}
	//	}


	//	public void Dispose()
	//	{
	//		//if (internalDir.Exists) internalDir.Delete(true);
	//	}

	//	~ZipDirectory()
	//	{
	//		if (internalDir.Exists) internalDir.Delete(true);
	//	}
	//}
}


