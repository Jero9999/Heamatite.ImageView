﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.IoSystem
{
	public interface IIoRepository
	{
		IList<IFileSystemObject> GetContents(string path);

		IDirectoryObject GetDirectory(string currentDirectory);

	}
}
