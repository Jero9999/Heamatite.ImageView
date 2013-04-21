using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace Heamatite.IoSystem.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void GetContentsFailsForBadPath()
		{
			IIoRepository repository = new IoRepository();

			IList<IFileSystemObject> dirList = repository.GetContents(path: "");
		}

		[TestMethod]
		public void GetContentsForCurrentDirectoryNotNull()
		{
			IIoRepository repository = new IoRepository();

			DirectoryInfo CurrentDirectory = new DirectoryInfo(".");
			DirectoryInfo testDir = null;
			var testDirName = Guid.NewGuid().ToString();
			try
			{
				Assert.IsTrue(CurrentDirectory.Exists, "current ("+CurrentDirectory.FullName+") dir not exist");
				testDir = CurrentDirectory.CreateSubdirectory(testDirName);
				IList<IFileSystemObject> dirList = repository.GetContents(testDir.FullName);
				Assert.IsNotNull(dirList);
			}
			finally
			{
				testDir.Delete();
			}
		}

		[TestMethod]
		public void GetContentsReturnsCorrectNumberOfFiles()
		{
			IIoRepository repository = new IoRepository();

			string testDirName = null;
			try
			{
				testDirName = CreateTempTestDir(testDirName);
				IList<IFileSystemObject> dirList = repository.GetContents(testDirName);
				Assert.AreEqual(1, dirList.Count, "GetContents not returning right number of objects");
			}
			finally
			{
				Directory.Delete(testDirName, true);
			}
			
		}

		private static string CreateTempTestDir(string testDirNameout)
		{
			var testDirName = Guid.NewGuid().ToString();
			DirectoryInfo CurrentDirectory = new DirectoryInfo(".");
			Assert.IsTrue(CurrentDirectory.Exists, "current (" + CurrentDirectory.FullName + ") dir not exist");
			DirectoryInfo testDir = CurrentDirectory.CreateSubdirectory(testDirName);
			FileInfo newFile = new FileInfo(Path.Combine(testDir.FullName, "test.txt"));
			var stream = newFile.Create();
			stream.Close();
			testDirNameout = testDir.FullName;
			return testDirNameout;
		}
	}
}
