using Heamatite.Icons;
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

		System.Windows.Visibility _InProgress = System.Windows.Visibility.Hidden;
		public System.Windows.Visibility InProgress
		{
			get { return _InProgress; }
			set
			{
				if (_InProgress == value) { return; }
				_InProgress = value;
				NotifyPropertyChanged();
			}
		}

		public void StartLongOperation()
		{
			InProgress = System.Windows.Visibility.Visible;
		}

		public void EndLongOperation()
		{
			InProgress = System.Windows.Visibility.Hidden;
		}

		private object _ContentsLocker = new Object();

		private IList<MainWindowFile> _ContentsInternal = null;
		private IList<MainWindowFile> ContentsInternal
		{
			get
			{
				lock (_ContentsLocker) { return _ContentsInternal; }
			}
			set
			{
				lock (_ContentsLocker) { _ContentsInternal = value; }
			}
		}

		public IList<MainWindowFile> ContentsAsync
		{
			get
			{
				if (ContentsInternal == null) { SetContents(); }
				return ContentsInternal;
			}
		}

		public IList<MainWindowFile> Contents
		{
			get
			{
				var theContents = ContentsAsync;
				if (!SetContentsTask.IsCompleted) SetContentsTask.Wait();
				return ContentsInternal;
			}
		}

		private Task SetContentsTask = null;
		private void SetContents()
		{
			ContentsInternal = new List<MainWindowFile>();
			//TODO need to work out how to cancel this if 'this' is disposed / released before the task completes
			SetContentsTask = Task.Run(() =>
			{
				StartLongOperation();
				ContentsInternal = _DirectoryObject.GetContents().Select(CreateNewFile).ToList();
				var firstItem = ContentsInternal.FirstOrDefault();
				if (firstItem != null) { SelectedFile = firstItem; }
				NotifyPropertyChanged("ContentsAsync");
				NotifyPropertyChanged("Contents");
				EndLongOperation();
			});

		}

		private MainWindowFile CreateNewFile(IFileSystemObject fileSystemObject)
		{
			MainWindowFile file = new MainWindowFile(fileSystemObject);
			file.PropertyChanged += (sender, eventArgs) =>
			{
				if (eventArgs.PropertyName == "Selected")
				{
					_SelectedFile = sender as MainWindowFile;
					NotifyPropertyChanged("SelectedFile");
				}
			};
			return file;
		}

		public string FullName
		{
			get { return _DirectoryObject.FullName; }
			set { }
		}

		private MainWindowFile _SelectedFile;
		public MainWindowFile SelectedFile
		{
			get
			{
				return _SelectedFile;
			}
			private set
			{
				value.Selected = true;
				_SelectedFile = value;
				NotifyPropertyChanged("SelectedFile");
			}
		}


		internal void SetSelected(MainWindowFile itemToSelect)
		{
			SelectedFile = itemToSelect;
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

		public string Name
		{
			get { return _FileSystemObject.Name; }
		}

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

		private bool _Selected;
		public bool Selected
		{
			get
			{
				return _Selected;
			}
			set
			{
				_Selected = value;
				NotifyPropertyChanged("Selected");
			}
		}
		private ImageSource _CachedIcon = null;
		private ImageSource GetIcon()
		{
			return IconCache.Instance.GetIconImage(_FileSystemObject);
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
