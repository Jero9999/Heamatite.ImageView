using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IoSystem.Zip
{
	using System.IO;
	using System.Text.RegularExpressions;
	using path = System.IO.Path;

	internal class ZipEntryFileObject : ZipEntryObject, IFileObject
	{
		private ZipFileObject _Owner;
		public ZipEntryFileObject(ZipEntry zipEntry, ZipFileObject owner)
			: base(zipEntry)
		{
			_Owner = owner;
		}

		public string Extension
		{
			get { return path.GetExtension(_ZipEntry.FileName); }
		}

		byte[] CachedBuffer = null;

		public System.IO.Stream OpenRead()
		{
			//multiple entry into this can cause problems due to the fact that you can't really have multiple
			//streams open on a single zip file at the same time.
			//Here we just make sure that if there are multiple streams open, they aren't into the zip stream.
			if (CachedBuffer == null)
			{
				lock (_Owner)
				{
					using (Stream zipStream = _ZipEntry.OpenReader())
					{
						CachedBuffer = new byte[zipStream.Length];
						zipStream.Read(CachedBuffer, 0, (int)zipStream.Length);
					}
				}
			}
			MemoryStream memoryStream = new MemoryStream(CachedBuffer);
			return memoryStream;
		}

		public override IDirectoryObject ParentDirectory
		{
			get { return _Owner; }
		}
	}

}


