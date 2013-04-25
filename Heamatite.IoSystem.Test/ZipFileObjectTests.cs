using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Linq;

namespace Heamatite.IoSystem.Test
{
	using path = System.IO.Path;
	using System.Diagnostics;
	using Heamatite.IoSystem.Zip;

	[TestClass]
	public class ZipFileObjectTests
	{
		DirectoryInfo TestDir;
		ZipFile TestZip;

		[TestInitialize]
		public void Init()
		{
			TestDir = CreateTempTestDir();
			TestZip = CreateTestZip(TestDir);
		}

		[TestCleanup]
		public void Cleanup()
		{
			TestZip = null;
			TestDir.Delete(true);
		}

		[TestMethod]
		public void TestIfZipEntryHasFullPathInFilename()
		{
			IIoRepository repo = new IoRepository();

			Assert.AreEqual(1, TestDir.GetFiles("test.zip").Count(), "zip file not exist");

			var stream = System.IO.File.CreateText(path.Combine(TestDir.FullName, "test2.txt"));
				stream.Close();
				TestZip.AddFile(path.Combine(TestDir.FullName, "test2.txt"), "");
				TestZip.Save();
				foreach (var entry in TestZip.Entries)
				{
					Debug.WriteLine(entry.ToString());
					Debug.WriteLine(entry.FileName);
				}
		}

		[TestMethod]
		public void CanCreateZipFileObject()
		{
			IIoRepository repo = new IoRepository();
			FileInfo zipFileInfo = new FileInfo(path.Combine(TestDir.FullName, "test.zip"));
			var zipObject = new ZipFileObject(zipFileInfo, repo);
		}

		[TestMethod]
		public void ZipFileObjectRetrievesObjects()
		{
			IIoRepository repo = new IoRepository();
			FileInfo zipFileInfo = new FileInfo(path.Combine(TestDir.FullName, "test.zip"));
			using (var zipObject = new ZipFileObject(zipFileInfo, repo))
			{
				Assert.AreEqual(0, zipObject.GetContents().Count(), "should be no root objects");
			}
		}

		[TestMethod]
		public void ZipFileObjectDirectories()
		{
			IIoRepository repo = new IoRepository();
			FileInfo zipFileInfo = new FileInfo(path.Combine(TestDir.FullName, "test.zip"));

			System.IO.File.CreateText(path.Combine(TestDir.FullName, "test2.txt")).Close();
			TestZip.AddFile(path.Combine(TestDir.FullName, "test2.txt"), "projects");
			TestZip.Save();
			foreach (var entry in TestZip.Entries)
			{
				Debug.WriteLine("Zip entry name: " + entry.FileName);
			}
			Debug.WriteLine("");
			using (var zipObject = new ZipFileObject(zipFileInfo, repo))
			{
				foreach (var item in zipObject.GetContents())
				{
					Debug.WriteLine(item.FullName);
				}
				Assert.AreEqual(2, zipObject.GetContents().Count(), "should be 2 root objects");
				Assert.AreEqual(1, zipObject.GetDirectories().Count(), "should be 1 root dir");
				Assert.AreEqual(1, zipObject.GetFiles().Count(), "should be 1 root file");

			}
		}




		[TestMethod]
		public void MyTestMethod()
		{
			string[] testStrs = new string[]
			{
				@"c:\test.txt",
				@"c:/test.txt",
				@"\test.txt",
				@"/test.txt",
				@"/test.txt/",
				@"/",
				@"test.txt/",
			};

			foreach (var testStr in testStrs)
			{
				Debug.WriteLine(string.Format("{0} getFileName {1}", testStr, path.GetFileName(testStr)));
				Debug.WriteLine(string.Format("{0} GetFullPath {1}", testStr, path.GetFullPath(testStr)));
				Debug.WriteLine(string.Format("{0} GetDirectoryName {1}", testStr, path.GetDirectoryName(testStr)));
				Debug.WriteLine(string.Format("{0} GetPathRoot {1}", testStr, path.GetPathRoot(testStr)));
				Debug.WriteLine("----");
			}
		}





		private static ZipFile CreateTestZip(DirectoryInfo testDir)
		{
			using (ZipFile testZip = new ZipFile(path.Combine(testDir.FullName, "test.zip")))
			{
				testZip.AddFile(path.Combine(testDir.FullName, "test.txt"), "");
				testZip.Save();
				return testZip;
			}
		}

		private static DirectoryInfo CreateTempTestDir()
		{
			var testDirName = Guid.NewGuid().ToString();
			DirectoryInfo CurrentDirectory = new DirectoryInfo(".");
			Assert.IsTrue(CurrentDirectory.Exists, "current (" + CurrentDirectory.FullName + ") dir not exist");
			DirectoryInfo testDir = CurrentDirectory.CreateSubdirectory(testDirName);
			FileInfo newFile = new FileInfo(Path.Combine(testDir.FullName, "test.txt"));
			var stream = newFile.Create();
			stream.Close();
			return testDir;
		}

	}
}
