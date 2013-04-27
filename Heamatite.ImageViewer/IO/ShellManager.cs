using Heamatite.IoSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using System.Windows.Interop;

namespace Heamatite.IO
{
	static class Interop
	{
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFileInfo psfi, uint cbFileInfo, uint uFlags);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyIcon(IntPtr hIcon);

		public const uint SHGFI_ICON = 0x000000100;
		public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
		public const uint SHGFI_OPENICON = 0x000000002;
		public const uint SHGFI_SMALLICON = 0x000000001;
		public const uint SHGFI_LARGEICON = 0x000000000;
		public const uint FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
		public const uint FILE_ATTRIBUTE_FILE = 0x00000100;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	struct SHFileInfo
	{
		public IntPtr hIcon;

		public int iIcon;

		public uint dwAttributes;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	}

	class ShellManager
	{
		public static Icon GetIcon(string path, ItemType type, IconSize size, ItemState state)
		{
			var flags = (uint)(Interop.SHGFI_ICON | Interop.SHGFI_USEFILEATTRIBUTES);
			var attribute = (uint)(object.Equals(type, ItemType.Folder) ? Interop.FILE_ATTRIBUTE_DIRECTORY : Interop.FILE_ATTRIBUTE_FILE);
			if (object.Equals(type, ItemType.Folder) && object.Equals(state, ItemState.Open))
			{
				flags += Interop.SHGFI_OPENICON;
			}
			if (object.Equals(size, IconSize.Small))
			{
				flags += Interop.SHGFI_SMALLICON;
			}
			else
			{
				flags += Interop.SHGFI_LARGEICON;
			}
			var shfi = new SHFileInfo();
			var res = Interop.SHGetFileInfo(path, attribute, out shfi, (uint)Marshal.SizeOf(shfi), flags);
			if (object.Equals(res, IntPtr.Zero)) throw Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
			try
			{
				Icon.FromHandle(shfi.hIcon);
				return (Icon)Icon.FromHandle(shfi.hIcon).Clone();
			}
			finally
			{
				Interop.DestroyIcon(shfi.hIcon);
			}
		}
	}

	enum IconSize : short
	{
		Small,
		Large
	}

	enum ItemType : short
	{
		Folder,
		File
	}

	public enum ItemState : short
	{
		Undefined,
		Open,
		Close
	}

	public static class FolderManager
	{
		public static ImageSource GetImageSource(string directory, ItemState folderType, int height = 16)
		{
			using (var icon = ShellManager.GetIcon(directory, ItemType.Folder, IconSize.Large, folderType))
			{
				var bitmap = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty, 
					BitmapSizeOptions.FromWidthAndHeight(height, height));
				bitmap.Freeze();
				return bitmap;
			}
		}
	}

	public static class FileManager
	{
		public static ImageSource GetImageSource(string filename, int height = 16)
		{
			using (var icon = ShellManager.GetIcon(Path.GetExtension(filename), ItemType.File, IconSize.Large, ItemState.Undefined))
			{
				var bitmap =Imaging.CreateBitmapSourceFromHIcon(icon.Handle, System.Windows.Int32Rect.Empty,
					BitmapSizeOptions.FromWidthAndHeight(height, height));
				bitmap.Freeze();
				return bitmap;
			}
		}
	}
}
