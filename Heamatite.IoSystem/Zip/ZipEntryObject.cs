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

	internal abstract class ZipEntryObject : IFileSystemObject
	{
		ZipEntry _ZipEntry;
		public ZipEntryObject(ZipEntry zipEntry)
		{
			_ZipEntry = zipEntry;
		}

		public string FullName
		{
			get { return _ZipEntry.FileName; }
		}

		public string Name
		{
			get { return path.GetFileName(FullName); }
		}

		public abstract IDirectoryObject ParentDirectory { get; }

		~ZipEntryObject()
		{
			_ZipEntry = null;
		}
	}
}


