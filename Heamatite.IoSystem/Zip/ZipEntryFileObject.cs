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

	internal class ZipEntryFileObject : ZipEntryObject, IFileObject
	{
		public ZipEntryFileObject(ZipEntry zipEntry)
			: base(zipEntry)
		{

		}

		public string Extension
		{
			get { throw new NotImplementedException(); }
		}

		public System.IO.Stream OpenRead()
		{
			throw new NotImplementedException();
		}

		public override IDirectoryObject ParentDirectory
		{
			get { throw new NotImplementedException(); }
		}
	}

}


