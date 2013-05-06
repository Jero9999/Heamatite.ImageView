using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IoSystem
{
	public class IoRepository : IIoRepository
	{
		public IEnumerable<IFileSystemObject> GetContentsEnumerable(string path)
		{
			DirectoryInfo pathDir = new DirectoryInfo(path);
			if (!pathDir.Exists) { throw new ArgumentException(path + " does not exist"); }

			return pathDir.EnumerateFileSystemInfos()
				.OrderBy(c =>
				{
					if (c.Attributes.HasFlag(FileAttributes.Directory)) { return 0; }
					else if (c.Attributes.HasFlag(FileAttributes.Archive)) { return 1; }
					else { return 2; }
				})
				.Select(c => Create(c));
		}

		public IList<IFileSystemObject> GetContents(string path)
		{
			return GetContentsEnumerable(path).ToList();
		}

		private IFileSystemObject Create(FileSystemInfo systemObject)
		{
			if (systemObject.Attributes.HasFlag(FileAttributes.Directory))
			{
				return new DirectoryObject(systemObject as DirectoryInfo, this);
			}
			else
			{
				string fileExtension = Path.GetExtension(systemObject.Name);
				if (string.Equals(fileExtension, ".zip", StringComparison.OrdinalIgnoreCase) &&
					Ionic.Zip.ZipFile.IsZipFile(systemObject.FullName))
				{
					return new Zip.ZipFileObject(systemObject as FileInfo, this);
				}
				return new FileObject(systemObject as FileInfo, this);
			}
		}

		public IDirectoryObject GetDirectory(string currentDirectory)
		{
			//TODO this is nasty. Create a directory. if we find it's not actually a directory create a file? bleh
			FileSystemInfo directoryInfo = new DirectoryInfo(currentDirectory);
			if (!directoryInfo.Attributes.HasFlag(FileAttributes.Directory))
			{
				directoryInfo = new FileInfo(currentDirectory);
			}
			return Create(directoryInfo) as IDirectoryObject;
		}
	}
}
