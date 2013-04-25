using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IoSystem
{
	class FileSystemObject : IFileSystemObject
	{
		protected FileSystemInfo FileSystemInfo;
		protected IIoRepository _Repository;

		public FileSystemObject(FileSystemInfo fileSystemInfo, IIoRepository repository)
		{
			FileSystemInfo = fileSystemInfo;
			_Repository = repository;
		}

		public string FullName
		{
			get { return FileSystemInfo.FullName; }
		}

		public string Name
		{
			get { return FileSystemInfo.Name; }
		}


		public IDirectoryObject ParentDirectory
		{
			get
			{
				string parentDirectory = System.IO.Path.GetDirectoryName(this.FullName);
				return _Repository.GetDirectory(parentDirectory);
			}
		}
	}

	class DirectoryObject : FileSystemObject, IDirectoryObject
	{

		public DirectoryObject(DirectoryInfo directory, IIoRepository repository)
			: base(directory, repository)
		{
		}

		private DirectoryInfo DirectoryInfo { get { return FileSystemInfo as DirectoryInfo; } }

		public IList<IFileSystemObject> GetContents()
		{
			return _Repository.GetContents(this.FullName).ToList();
		}

		public IList<IDirectoryObject> GetDirectories()
		{
			return _Repository.GetContents(this.FullName).Where(c => c is IDirectoryObject).Cast<IDirectoryObject>().ToList();
		}
		
		public IList<IFileObject> GetFiles()
		{
			return _Repository.GetContents(this.FullName).Where(c => c is IFileObject).Cast<IFileObject>().ToList();
		}

	}

	class FileObject : FileSystemObject, IFileObject
	{
		public FileObject(FileInfo file, IIoRepository repository)
			: base(file, repository)
		{
		}

		private FileInfo FileInfo { get { return FileSystemInfo as FileInfo; } }

		public string Extension
		{
			get { return this.FileInfo.Extension; }
		}

		public Stream OpenRead()
		{
			return FileInfo.OpenRead();
		}
	}
}
