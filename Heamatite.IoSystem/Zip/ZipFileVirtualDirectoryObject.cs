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

	internal class ZipFileVirtualDirectoryObject : IDirectoryObject
	{
		ZipFile _ZipFile;
		string _localZipPath;
		string _ZipFileFullName;
		public ZipFileVirtualDirectoryObject(ZipFile zipFile, string zipFileFullName, string path)
		{
			_ZipFile = zipFile;
			_localZipPath = path;
			_ZipFileFullName = zipFileFullName;
		}

		#region IDirectoryInfo implementation
		public IList<IFileSystemObject> GetContents()
		{
			throw new NotImplementedException();
		}

		public IList<IDirectoryObject> GetDirectories()
		{
			throw new NotImplementedException();
		}

		public IList<IFileObject> GetFiles()
		{
			throw new NotImplementedException();
		}
		#endregion


		public string FullName
		{
			get { return path.Combine(_ZipFileFullName, this._localZipPath); }
		}

		public string Name
		{
			get { return path.GetFileName(_localZipPath); }
		}

		public IDirectoryObject ParentDirectory
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerable<IFileSystemObject> GetContentsEnumerable()
		{
			throw new NotImplementedException();
		}
	}

}


