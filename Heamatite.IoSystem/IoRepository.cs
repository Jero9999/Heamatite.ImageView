﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IoSystem
{
	public class IoRepository : IIoRepository
	{
		public IList<IFileSystemObject> GetContents(string path)
		{
			DirectoryInfo pathDir = new DirectoryInfo(path);
			if (!pathDir.Exists) { throw new ArgumentException(path + " does not exist"); }

			return pathDir.GetFileSystemInfos().Select(c => Create(c)).ToList();
		}

		private IFileSystemObject Create(FileSystemInfo systemObject)
		{
			if (systemObject.Attributes.HasFlag(FileAttributes.Directory))
			{
				return (IFileSystemObject)new DirectoryObject(systemObject as DirectoryInfo, this);
			}
			else
			{
				if (Ionic.Zip.ZipFile.IsZipFile(systemObject.FullName))
				{
					return (IFileSystemObject)new Zip.ZipFileObject(systemObject as FileInfo, this);
				}
				return (IFileSystemObject)new FileObject(systemObject as FileInfo, this);
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
