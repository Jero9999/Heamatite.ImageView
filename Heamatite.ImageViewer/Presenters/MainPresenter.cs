using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Heamatite.View;
//using Heamatite.IO;

namespace Heamatite.View.Presenters
{
	using Heamatite.ViewInterfaces;
	using Heamatite.IoSystem;
	using Heamatite.IO;

	public class MainPresenter
	{
		private IMainView _View;
		IConfiguration _Config;
		IIoRepository _Repo;
		IDirectoryObject _CurrentDirectory;

		public MainPresenter(IMainView view, IConfiguration config, IIoRepository repository)
		{
			_View = view;
			_Config = config;
			_Repo = repository;

			string currentDirectory = _Config.StartupDirectory??@"C:\";
			_CurrentDirectory = _Repo.GetDirectory(currentDirectory);
			SetupView(currentDirectory);
		}

		private void SetupView(string currentDirectory)
		{
			_View.SizeChanged += ViewSizeChanged;
			_View.DataContext = new MainWindowDirectoryWrapper(_CurrentDirectory);
			_View.Width = _Config.MainViewWidth;
			_View.Height = _Config.MainViewHeight;
			_View.UpdateFileList = UpdateFileList;
			_View.GotoSelectedItem = (selectedItem) => GotoSelectedItem(selectedItem);
			_View.MoveRight = MoveRight;
			_View.MoveLeft = MoveLeft;
			_View.MoveToParent = MoveToParent;
			_View.CurrentDirectoryEnter = CurrentDirectoryEnter;
		}

		void ViewSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			_Config.MainViewHeight = (int)e.NewSize.Height;
			_Config.MainViewWidth = (int)e.NewSize.Width;
		}

		private void UpdateFileList()
		{
			_View.DataContext = new MainWindowDirectoryWrapper(_CurrentDirectory);
			_Config.StartupDirectory = _CurrentDirectory.FullName;
		}

		private void GotoSelectedItem(object selectedItem)
		{
			IFileSystemObject selected = selectedItem as IFileSystemObject;
			if (selected == null) return;
			if (selected is IDirectoryObject)
			{
				_CurrentDirectory = selected as IDirectoryObject;
				UpdateFileList();
			}
			else
			{
				IFileObject file = selected as IFileObject;
				IDirectoryObject dir = file.ParentDirectory;
				WindowManager.ShowImageWindow(new ImageDirectory(dir), file.Name);
			}
		}

		private void MoveRight(int currentIndex, int NumberOfItems, Action<int> setSelectedIndex)
		{
			int index = currentIndex;
			setSelectedIndex(System.Math.Min(NumberOfItems - 1, index + 1));
		}

		private void MoveLeft(int currentIndex, Action<int> setSelectedIndex)
		{
			setSelectedIndex(System.Math.Max(0, currentIndex - 1));
		}

		private void MoveToParent()
		{
			_CurrentDirectory = _CurrentDirectory.ParentDirectory;
			UpdateFileList();
		}

		private void CurrentDirectoryEnter()
		{
			_CurrentDirectory = _Repo.GetDirectory( _View.CurrentDirectory);
			UpdateFileList();
		}
	}

	class MainWindowDirectoryWrapper
	{
		IDirectoryObject _DirectoryObject;
		public MainWindowDirectoryWrapper(IDirectoryObject directoryObject)
		{
			_DirectoryObject = directoryObject;
		}

		public IList<IFileSystemObject> Contents
		{
			get
			{
				return _DirectoryObject.GetContents();
			}
		}

		public string FullName
		{
			get { return _DirectoryObject.FullName; }
			set { }
		}
	}
}
