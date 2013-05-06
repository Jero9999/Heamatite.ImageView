using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heamatite.IoSystem
{
	public interface IFileSystemObject
	{
		string FullName { get; }
		string Name { get; }
		IDirectoryObject ParentDirectory { get; }
	}

	public interface IDirectoryObject : IFileSystemObject
	{
		IEnumerable<IFileSystemObject> GetContentsEnumerable();
		IList<IFileSystemObject> GetContents();
		IList<IDirectoryObject> GetDirectories();
		IList<IFileObject> GetFiles();
	}

	public interface IFileObject : IFileSystemObject
	{
		string Extension { get; }

		System.IO.Stream OpenRead();

	}
}
