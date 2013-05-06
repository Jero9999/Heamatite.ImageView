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

	internal class ZipEntryDirectoryObject : ZipEntryObject, IDirectoryObject
	{
		ZipFile _ZipFile;

		public ZipEntryDirectoryObject(ZipEntry zipEntry, ZipFile zipFile)
			: base(zipEntry)
		{
			_ZipFile = zipFile;
		}

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


		public override IDirectoryObject ParentDirectory
		{
			get { throw new NotImplementedException(); }
		}

		~ZipEntryDirectoryObject()
		{
			_ZipFile = null;
		}

		public IEnumerable<IFileSystemObject> GetContentsEnumerable()
		{
			throw new NotImplementedException();
		}
	}

}


