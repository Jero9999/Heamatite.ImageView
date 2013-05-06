using Heamatite.Icons;
using Heamatite.IoSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Heamatite.IO
{
	class MainWindowFile : INotifyPropertyChanged
	{

		public MainWindowFile(IFileSystemObject fileSystemObject, IconCacheQueue iconCacheQueue)
		{
			_FileSystemObject = fileSystemObject;
			_IconCache = iconCacheQueue;
		}

		IconCacheQueue _IconCache;
		IFileSystemObject _FileSystemObject;
		public IFileSystemObject SystemObject { get { return _FileSystemObject; } }

		public string Name
		{
			get { return _FileSystemObject.Name; }
		}

		private bool _GettingIcon = false;
		public ImageSource Icon
		{
			get
			{
				if (_CachedIcon == null && !_GettingIcon)
				{
					_GettingIcon = true;
					Task.Run(() => GetIcon());
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
		private void GetIcon()
		{
			_IconCache.GetIconImageAsync(_FileSystemObject, c =>
			{
				_CachedIcon = c;
				NotifyPropertyChanged("Icon");
				_GettingIcon = false;
			});
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
