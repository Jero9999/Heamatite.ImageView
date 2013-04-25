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

	static class ZipHelpers
	{
		public static IEnumerable<ZipFileVirtualDirectoryObject> GetVirtualDirectoriesForPath(
			this ZipFile zipFile, string zipFileFullName, string basePath)
		{
			basePath = basePath.Replace('/', '\\').TrimStart('\\');
			var virtualDirectories = zipFile.Entries.Select(c => c.FileName)
				.Select(c=> c.Replace('/', '\\').TrimStart('\\'));
			
			if(basePath.Length > 0){
				virtualDirectories
					.Where(c => c.StartsWith(basePath))
					.Select(c => c.Substring(basePath.Length));
			}
				
			virtualDirectories= virtualDirectories.Where(c=> c.StartsWith(basePath))
				.Select(c => c.Split('\\'))
				.Where(c=> c.Count() > 1)
				.Select(c=>c.First())
				.Distinct();
			
			return virtualDirectories.Select(c => new ZipFileVirtualDirectoryObject(zipFile, zipFileFullName, c));
		}
	}
}


