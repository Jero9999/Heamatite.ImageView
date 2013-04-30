using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Heamatite.Icons
{

	#region Public Enumerations
	/// <summary>
	/// Available system image list sizes
	/// </summary>
	public enum SysImageListSize : int
	{
		/// <summary>
		/// System Large Icon Size (typically 32x32)
		/// </summary>
		largeIcons = 0x0,
		/// <summary>
		/// System Small Icon Size (typically 16x16)
		/// </summary>
		smallIcons = 0x1,
		/// <summary>
		/// System Extra Large Icon Size (typically 48x48).
		/// Only available under XP; under other OS the
		/// Large Icon ImageList is returned.
		/// </summary>
		extraLargeIcons = 0x2,
		jumbo = 0x4
	}

	/// <summary>
	/// Flags controlling how the Image List item is 
	/// drawn
	/// </summary>
	[Flags]
	public enum ImageListDrawItemConstants : int
	{
		/// <summary>
		/// Draw item normally.
		/// </summary>
		ILD_NORMAL = 0x0,
		/// <summary>
		/// Draw item transparently.
		/// </summary>
		ILD_TRANSPARENT = 0x1,
		/// <summary>
		/// Draw item blended with 25% of the specified foreground colour
		/// or the Highlight colour if no foreground colour specified.
		/// </summary>
		ILD_BLEND25 = 0x2,
		/// <summary>
		/// Draw item blended with 50% of the specified foreground colour
		/// or the Highlight colour if no foreground colour specified.
		/// </summary>
		ILD_SELECTED = 0x4,
		/// <summary>
		/// Draw the icon's mask
		/// </summary>
		ILD_MASK = 0x10,
		/// <summary>
		/// Draw the icon image without using the mask
		/// </summary>
		ILD_IMAGE = 0x20,
		/// <summary>
		/// Draw the icon using the ROP specified.
		/// </summary>
		ILD_ROP = 0x40,
		/// <summary>
		/// Preserves the alpha channel in dest. XP only.
		/// </summary>
		ILD_PRESERVEALPHA = 0x1000,
		/// <summary>
		/// Scale the image to cx, cy instead of clipping it.  XP only.
		/// </summary>
		ILD_SCALE = 0x2000,
		/// <summary>
		/// Scale the image to the current DPI of the display. XP only.
		/// </summary>
		ILD_DPISCALE = 0x4000
	}

	/// <summary>
	/// Flags specifying the state of the icon to draw from the Shell
	/// </summary>
	[Flags]
	public enum ShellIconStateConstants
	{
		/// <summary>
		/// Get icon in normal state
		/// </summary>
		ShellIconStateNormal = 0,
		/// <summary>
		/// Put a link overlay on icon 
		/// </summary>
		ShellIconStateLinkOverlay = 0x8000,
		/// <summary>
		/// show icon in selected state 
		/// </summary>
		ShellIconStateSelected = 0x10000,
		/// <summary>
		/// get open icon 
		/// </summary>
		ShellIconStateOpen = 0x2,
		/// <summary>
		/// apply the appropriate overlays
		/// </summary>
		ShellIconAddOverlays = 0x000000020,
	}
	#endregion

	#region SysImageList
	/// <summary>
	/// Summary description for SysImageList.
	/// </summary>
	class SysImageList : IDisposable
	{
		#region UnmanagedCode
		private const int MAX_PATH = 260;

		[DllImport("shell32")]
		private static extern IntPtr SHGetFileInfo(
				string pszPath,
				int dwFileAttributes,
				ref SHFILEINFO psfi,
				uint cbFileInfo,
				uint uFlags);

		[DllImport("user32.dll")]
		private static extern int DestroyIcon(IntPtr hIcon);

		private const int FILE_ATTRIBUTE_NORMAL = 0x80;
		private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;

		private const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x100;
		private const int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
		private const int FORMAT_MESSAGE_FROM_HMODULE = 0x800;
		private const int FORMAT_MESSAGE_FROM_STRING = 0x400;
		private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
		private const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
		private const int FORMAT_MESSAGE_MAX_WIDTH_MASK = 0xFF;
		[DllImport("kernel32")]
		private extern static int FormatMessage(
				int dwFlags,
				IntPtr lpSource,
				int dwMessageId,
				int dwLanguageId,
				string lpBuffer,
				uint nSize,
				int argumentsLong);

		[DllImport("kernel32")]
		private extern static int GetLastError();

		[DllImport("comctl32")]
		private extern static int ImageList_Draw(
				IntPtr hIml,
				int i,
				IntPtr hdcDst,
				int x,
				int y,
				int fStyle);

		[DllImport("comctl32")]
		private extern static int ImageList_DrawIndirect(
				ref IMAGELISTDRAWPARAMS pimldp);

		[DllImport("comctl32")]
		private extern static int ImageList_GetIconSize(
				IntPtr himl,
				ref int cx,
				ref int cy);

		[DllImport("comctl32")]
		private extern static IntPtr ImageList_GetIcon(
				IntPtr himl,
				int i,
				int flags);

		/// <summary>
		/// SHGetImageList is not exported correctly in XP.  See KB316931
		/// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
		/// Apparently (and hopefully) ordinal 727 isn't going to change.
		/// </summary>
		[DllImport("shell32.dll", EntryPoint = "#727")]
		private extern static int SHGetImageList(
				int iImageList,
				ref Guid riid,
				ref IImageList ppv
				);

		[DllImport("shell32.dll", EntryPoint = "#727")]
		private extern static int SHGetImageListHandle(
				int iImageList,
				ref Guid riid,
				ref IntPtr handle
				);

		#endregion

		#region Private Enumerations
		[Flags]
		private enum SHGetFileInfoConstants : int
		{
			SHGFI_ICON = 0x100,                // get icon 
			SHGFI_DISPLAYNAME = 0x200,         // get display name 
			SHGFI_TYPENAME = 0x400,            // get type name 
			SHGFI_ATTRIBUTES = 0x800,          // get attributes 
			SHGFI_ICONLOCATION = 0x1000,       // get icon location 
			SHGFI_EXETYPE = 0x2000,            // return exe type 
			SHGFI_SYSICONINDEX = 0x4000,       // get system icon index 
			SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon 
			SHGFI_SELECTED = 0x10000,          // show icon in selected state 
			SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes 
			SHGFI_LARGEICON = 0x0,             // get large icon 
			SHGFI_SMALLICON = 0x1,             // get small icon 
			SHGFI_OPENICON = 0x2,              // get open icon 
			SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon 
			//SHGFI_PIDL = 0x8,                  // pszPath is a pidl 
			SHGFI_USEFILEATTRIBUTES = 0x10,     // use passed dwFileAttribute 
			SHGFI_ADDOVERLAYS = 0x000000020,     // apply the appropriate overlays
			SHGFI_OVERLAYINDEX = 0x000000040     // Get the index of the overlay
		}
		#endregion

		#region Private ImageList structures
		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			int left;
			int top;
			int right;
			int bottom;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct POINT
		{
			int x;
			int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct IMAGELISTDRAWPARAMS
		{
			public int cbSize;
			public IntPtr himl;
			public int i;
			public IntPtr hdcDst;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int xBitmap;        // x offest from the upperleft of bitmap
			public int yBitmap;        // y offset from the upperleft of bitmap
			public int rgbBk;
			public int rgbFg;
			public int fStyle;
			public int dwRop;
			public int fState;
			public int Frame;
			public int crEffect;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct IMAGEINFO
		{
			public IntPtr hbmImage;
			public IntPtr hbmMask;
			public int Unused1;
			public int Unused2;
			public RECT rcImage;
		}
		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int iIcon;
			public int dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}
		#endregion

		#region Private ImageList COM Interop (XP)
		[ComImportAttribute()]
		[GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
		[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
		//helpstring("Image List"),
		interface IImageList
		{
			[PreserveSig]
			int Add(
					IntPtr hbmImage,
					IntPtr hbmMask,
					ref int pi);

			[PreserveSig]
			int ReplaceIcon(
					int i,
					IntPtr hicon,
					ref int pi);

			[PreserveSig]
			int SetOverlayImage(
					int iImage,
					int iOverlay);

			[PreserveSig]
			int Replace(
					int i,
					IntPtr hbmImage,
					IntPtr hbmMask);

			[PreserveSig]
			int AddMasked(
					IntPtr hbmImage,
					int crMask,
					ref int pi);

			[PreserveSig]
			int Draw(
					ref IMAGELISTDRAWPARAMS pimldp);

			[PreserveSig]
			int Remove(
					int i);

			[PreserveSig]
			int GetIcon(
					int i,
					int flags,
					ref IntPtr picon);

			[PreserveSig]
			int GetImageInfo(
					int i,
					ref IMAGEINFO pImageInfo);

			[PreserveSig]
			int Copy(
					int iDst,
					IImageList punkSrc,
					int iSrc,
					int uFlags);

			[PreserveSig]
			int Merge(
					int i1,
					IImageList punk2,
					int i2,
					int dx,
					int dy,
					ref Guid riid,
					ref IntPtr ppv);

			[PreserveSig]
			int Clone(
					ref Guid riid,
					ref IntPtr ppv);

			[PreserveSig]
			int GetImageRect(
					int i,
					ref RECT prc);

			[PreserveSig]
			int GetIconSize(
					ref int cx,
					ref int cy);

			[PreserveSig]
			int SetIconSize(
					int cx,
					int cy);

			[PreserveSig]
			int GetImageCount(
					ref int pi);

			[PreserveSig]
			int SetImageCount(
					int uNewCount);

			[PreserveSig]
			int SetBkColor(
					int clrBk,
					ref int pclr);

			[PreserveSig]
			int GetBkColor(
					ref int pclr);

			[PreserveSig]
			int BeginDrag(
					int iTrack,
					int dxHotspot,
					int dyHotspot);

			[PreserveSig]
			int EndDrag();

			[PreserveSig]
			int DragEnter(
					IntPtr hwndLock,
					int x,
					int y);

			[PreserveSig]
			int DragLeave(
					IntPtr hwndLock);

			[PreserveSig]
			int DragMove(
					int x,
					int y);

			[PreserveSig]
			int SetDragCursorImage(
					ref IImageList punk,
					int iDrag,
					int dxHotspot,
					int dyHotspot);

			[PreserveSig]
			int DragShowNolock(
					int fShow);

			[PreserveSig]
			int GetDragImage(
					ref POINT ppt,
					ref POINT pptHotspot,
					ref Guid riid,
					ref IntPtr ppv);

			[PreserveSig]
			int GetItemFlags(
					int i,
					ref int dwFlags);

			[PreserveSig]
			int GetOverlayImage(
					int iOverlay,
					ref int piIndex);
		};
		#endregion

		#region Member Variables
		private IntPtr hIml = IntPtr.Zero;
		private IImageList iImageList = null;
		private SysImageListSize size = SysImageListSize.smallIcons;
		private bool disposed = false;
		#endregion


		#region Properties
		/// <summary>
		/// Gets/sets the size of System Image List to retrieve.
		/// </summary>
		public SysImageListSize ImageListSize
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				create();
			}

		}

		#endregion

		#region Methods
		/// <summary>
		/// Returns a GDI+ copy of the icon from the ImageList
		/// at the specified index.
		/// </summary>
		/// <param name="index">The index to get the icon for</param>
		/// <returns>The specified icon</returns>
		public Icon Icon(int index)
		{
			Icon icon = null;

			IntPtr hIcon = IntPtr.Zero;
			if (iImageList == null)
			{
				hIcon = ImageList_GetIcon(
						hIml,
						index,
						(int)ImageListDrawItemConstants.ILD_TRANSPARENT);

			}
			else
			{
				iImageList.GetIcon(
						index,
						(int)ImageListDrawItemConstants.ILD_TRANSPARENT,
						ref hIcon);
			}

			if (hIcon != IntPtr.Zero)
			{
				icon = System.Drawing.Icon.FromHandle(hIcon);
			}
			return icon;
		}

		/// <summary>
		/// Returns the index of the icon for the specified file
		/// </summary>
		/// <param name="fileName">Filename to get icon for</param>
		/// <param name="forceLoadFromDisk">If True, then hit the disk to get the icon,
		/// otherwise only hit the disk if no cached icon is available.</param>
		/// <param name="iconState">Flags specifying the state of the icon
		/// returned.</param>
		/// <returns>Index of the icon</returns>
		public int IconIndex(
				string fileName,
				bool forceLoadFromDisk = false,
				ShellIconStateConstants iconState = ShellIconStateConstants.ShellIconStateNormal
				)
		{
			SHGetFileInfoConstants dwFlags = SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
			int dwAttr = 0;
			if (size == SysImageListSize.smallIcons)
			{
				dwFlags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
			}

			// We can choose whether to access the disk or not. If you don't
			// hit the disk, you may get the wrong icon if the icon is
			// not cached. Also only works for files.
			if (!forceLoadFromDisk)
			{
				dwFlags |= SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES;
				dwAttr = FILE_ATTRIBUTE_NORMAL;
			}
			else
			{
				dwAttr = 0;
			}

			// sFileSpec can be any file. You can specify a
			// file that does not exist and still get the
			// icon, for example sFileSpec = "C:\PANTS.DOC"
			SHFILEINFO shfi = new SHFILEINFO();
			uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());
			IntPtr retVal = SHGetFileInfo(
					fileName, dwAttr, ref shfi, shfiSize,
					((uint)(dwFlags) | (uint)iconState));

			if (retVal.Equals(IntPtr.Zero))
			{
				System.Diagnostics.Debug.Assert((!retVal.Equals(IntPtr.Zero)), "Failed to get icon index");
				return 0;
			}
			else
			{
				return shfi.iIcon;
			}
		}

	

		/// <summary>
		/// Determines if the system is running Windows XP
		/// or above
		/// </summary>
		/// <returns>True if system is running XP or above, False otherwise</returns>
		private bool isXpOrAbove()
		{
			bool ret = false;
			if (Environment.OSVersion.Version.Major > 5)
			{
				ret = true;
			}
			else if ((Environment.OSVersion.Version.Major == 5) &&
					(Environment.OSVersion.Version.Minor >= 1))
			{
				ret = true;
			}
			return ret;
			//return false;
		}

		/// <summary>
		/// Creates the SystemImageList
		/// </summary>
		private void create()
		{
			// forget last image list if any:
			hIml = IntPtr.Zero;

			if (isXpOrAbove())
			{
				// Get the System IImageList object from the Shell:
				Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
				int ret = SHGetImageList(
						(int)size,
						ref iidImageList,
						ref iImageList
						);
				// the image list handle is the IUnknown pointer, but 
				// using Marshal.GetIUnknownForObject doesn't return
				// the right value.  It really doesn't hurt to make
				// a second call to get the handle:
				SHGetImageListHandle((int)size, ref iidImageList, ref hIml);
			}
			else
			{
				// Prepare flags:
				SHGetFileInfoConstants dwFlags = SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES | SHGetFileInfoConstants.SHGFI_SYSICONINDEX;
				if (size == SysImageListSize.smallIcons)
				{
					dwFlags |= SHGetFileInfoConstants.SHGFI_SMALLICON;
				}
				// Get image list
				SHFILEINFO shfi = new SHFILEINFO();
				uint shfiSize = (uint)Marshal.SizeOf(shfi.GetType());

				// Call SHGetFileInfo to get the image list handle
				// using an arbitrary file:
				hIml = SHGetFileInfo(
						".txt",
						FILE_ATTRIBUTE_NORMAL,
						ref shfi,
						shfiSize,
						(uint)dwFlags);
				System.Diagnostics.Debug.Assert((hIml != IntPtr.Zero), "Failed to create Image List");
			}
		}
		#endregion

		#region Constructor, Dispose, Destructor
		/// <summary>
		/// Creates a Small Icons SystemImageList 
		/// </summary>
		public SysImageList()
		{
			create();
		}
		/// <summary>
		/// Creates a SystemImageList with the specified size
		/// </summary>
		/// <param name="size">Size of System ImageList</param>
		public SysImageList(SysImageListSize size)
		{
			this.size = size;
			create();
		}

		/// <summary>
		/// Clears up any resources associated with the SystemImageList
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		/// <summary>
		/// Clears up any resources associated with the SystemImageList
		/// when disposing is true.
		/// </summary>
		/// <param name="disposing">Whether the object is being disposed</param>
		public virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (iImageList != null)
					{
						Marshal.ReleaseComObject(iImageList);
					}
					iImageList = null;
				}
			}
			disposed = true;
		}
		/// <summary>
		/// Finalise for SysImageList
		/// </summary>
		~SysImageList()
		{
			Dispose(false);
		}

		#endregion
	}

	#endregion

}
