using Heamatite.Icons;
using Heamatite.IoSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Heamatite.IO
{
	class MainWindowDirectoryWrapper : INotifyPropertyChanged, IDisposable
	{
		IDirectoryObject _DirectoryObject;
		private Dispatcher UiDispatcher;
		private IconCacheQueue IconCache;

		public MainWindowDirectoryWrapper(IDirectoryObject directoryObject, Dispatcher uiDispatcher, IconCacheQueue iconCacheQueue)
		{
			IconCache = iconCacheQueue;
			_DirectoryObject = directoryObject;
			UiDispatcher = uiDispatcher;
		}

		~MainWindowDirectoryWrapper()
		{
			Dispose();
		}

		private bool IsDisposed = false;
		public void Dispose()
		{
			if (!IsDisposed)
			{
				IsDisposed = true;
				IconCache.ClearQueue();
			}
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

		private ObservableCollection<MainWindowFile> _ContentsInternal = null;
		private ObservableCollection<MainWindowFile> ContentsInternal
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

		public ObservableCollection<MainWindowFile> ContentsAsync
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
			ContentsInternal = new ObservableCollection<MainWindowFile>();
			//TODO need to work out how to cancel this if 'this' is disposed / released before the task completes
			SetContentsTask = Task.Run(() =>
			{
				StartLongOperation();
				try
				{
					IEnumerable<IFileSystemObject> directoryContents = _DirectoryObject.GetContentsEnumerable();
					var aLot = directoryContents.Take(10);

					while (aLot.Count() > 0)
					{
						UiDispatcher.Invoke(() =>
							{
								aLot.Select(CreateNewFile).ToList().ForEach(c => _ContentsInternal.Add(c));
							}
						);
						//var firstItem = ContentsInternal.FirstOrDefault();
						//if (firstItem != null) { SelectedFile = firstItem; }
						directoryContents = directoryContents.Skip(10);
						aLot = directoryContents.Take(10);
					}
				}
				finally
				{
					EndLongOperation();
				}
			});

		}

		private MainWindowFile CreateNewFile(IFileSystemObject fileSystemObject)
		{
			MainWindowFile file = new MainWindowFile(fileSystemObject, IconCache);
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

}
