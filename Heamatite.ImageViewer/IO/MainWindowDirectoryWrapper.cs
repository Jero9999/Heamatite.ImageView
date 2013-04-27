using Heamatite.IoSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Heamatite.IO
{
	class MainWindowDirectoryWrapper : INotifyPropertyChanged
	{
		IDirectoryObject _DirectoryObject;
		public MainWindowDirectoryWrapper(IDirectoryObject directoryObject)
		{
			_DirectoryObject = directoryObject;
		}

		private IList<MainWindowFile> _Contents = null;
		public IList<MainWindowFile> Contents
		{
			get
			{
				if (_Contents == null)
				{
					SetContents();
				}
				return _Contents;
			}
		}

		private Task SetContentsTask = null;
		private void SetContents()
		{
			_Contents = new List<MainWindowFile>();
			//TODO need to work out how to cancel this if 'this' is disposed / released before the task completes
			SetContentsTask = Task.Run(() =>
			{
				_Contents = _DirectoryObject.GetContents().Select(c => new MainWindowFile(c)).ToList();
				NotifyPropertyChanged("Contents");
			});

		}

		public string FullName
		{
			get { return _DirectoryObject.FullName; }
			set { }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName == null) return;

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	class MainWindowFile : INotifyPropertyChanged
	{

		public MainWindowFile(IFileSystemObject fileSystemObject)
		{
			_FileSystemObject = fileSystemObject;
		}

		IFileSystemObject _FileSystemObject;
		public IFileSystemObject SystemObject { get { return _FileSystemObject; } }

		public string Name { get { return _FileSystemObject.Name; } }

		public ImageSource Icon
		{
			get
			{
				if (_CachedIcon == null)
				{
					Task.Run(() =>
					{
						_CachedIcon = GetIcon();
						NotifyPropertyChanged("Icon");
					});
				}
				return _CachedIcon;
			}
		}

		private ImageSource _CachedIcon = null;
		private ImageSource GetIcon()
		{
			return IconCache.GetIconImage(_FileSystemObject);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName == null) return;

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
