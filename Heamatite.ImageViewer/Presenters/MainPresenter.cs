using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Heamatite.View;
using Heamatite.IO;

namespace Heamatite.View.Presenters
{
	using Heamatite.ViewInterfaces;

	public class MainPresenter
	{
		private IMainView _View;
		IConfiguration _Config;
		FileRepository _Repo;

		public MainPresenter(IMainView view, IConfiguration config, FileRepository repository)
		{
			_View = view;
			_Config = config;
			_Repo = repository;


			string currentDirectory = _Config.StartupDirectory??@"C:\";
			_Repo.Location = currentDirectory;
			
			SetupView(currentDirectory);
		}

		private void SetupView(string currentDirectory)
		{
			_View.SizeChanged += ViewSizeChanged;
			_View.DataContext = _Repo.GetContainerList();
			_View.CurrentDirectory = currentDirectory;
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
			_View.DataContext = _Repo.GetContainerList();
			_Config.StartupDirectory = _Repo.Location;
		}

		private void GotoSelectedItem(object selectedItem)
		{
			Io selected = selectedItem as Io;
			if (selected == null) return;
			if (selected.IoType.HasFlag(IoType.Directory))
			{
				_Repo.Location = _View.CurrentDirectory = selected.FullName;
				UpdateFileList();
			}
			else if (selected.IoType.HasFlag(IoType.File) && selected.IoType.HasFlag(IoType.Image))
			{
				var dir = System.IO.Path.GetDirectoryName(selected.FullName);
				WindowManager.ShowImageWindow(new ImageDirectory(dir), System.IO.Path.GetFileName(selected.FullName));
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
			_Repo.MoveToParent();
			UpdateFileList();
		}

		private void CurrentDirectoryEnter()
		{
			_Repo.Location = _View.CurrentDirectory;
			UpdateFileList();
		}
	}
}
