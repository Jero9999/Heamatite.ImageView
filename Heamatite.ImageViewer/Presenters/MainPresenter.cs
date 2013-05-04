
namespace Heamatite.View.Presenters
{
	using System;
	using Heamatite.IO;
	using Heamatite.IoSystem;
	using Heamatite.ViewInterfaces;
	using System.IO;
	using System.Threading.Tasks;
	using System.Linq;

	public interface IMainPresenter
	{
		IMainView View { get; }
	}

	public class MainPresenter : IMainPresenter
	{
		private IMainView _View;
		private IConfiguration _Config;
		private IIoRepository _Repo;
		private IDirectoryObject _CurrentDirectory;
		private IWindowManager _WindowManager;

		public MainPresenter(
			IWindowManager windowManager,
			IMainView view,
			IConfiguration config,
			IIoRepository repository)
		{
			_WindowManager = windowManager;
			_View = view;
			_Config = config;
			_Repo = repository;

			string currentDirectory = _Config.StartupDirectory ?? Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			_CurrentDirectory = _Repo.GetDirectory(currentDirectory);
			SetupView(currentDirectory);
		}

		public IMainView View { get { return _View; } }

		//need this so that code running a thread other than the UI thread has access to the window's DataContext.
		MainWindowDirectoryWrapper _DataContext;
		MainWindowDirectoryWrapper DataContext
		{
			get
			{
				return _DataContext;
			}
			set
			{
				_View.DataContext = _DataContext = value;
			}
		}

		private void SetupView(string currentDirectory)
		{
			_View.SizeChanged += ViewSizeChanged;
			_View.Width = _Config.MainViewWidth;
			_View.Height = _Config.MainViewHeight;
			_View.UpdateFileList = UpdateFileList;
			_View.GotoSelectedItem = (selectedItem) => GotoSelectedItem(selectedItem);
			_View.MoveRight = MoveRight;
			_View.MoveLeft = MoveLeft;
			_View.MoveToParent = MoveToParent;
			_View.CurrentDirectoryEnter = CurrentDirectoryEnter;
			_View.KeyboardSearch = KeyboardSearch;

			DataContext = new MainWindowDirectoryWrapper(_CurrentDirectory);

		}

		private static TimeSpan ResetSearchStringTime = TimeSpan.FromMilliseconds(500);
		private DateTime LastSearchKeyPressed = DateTime.Now;
		private string SearchString = string.Empty;
		private void KeyboardSearch(string key)
		{
			if (DateTime.Now - LastSearchKeyPressed > ResetSearchStringTime)
			{
				SearchString = string.Empty;
			}
			LastSearchKeyPressed = DateTime.Now;
			char charKey = key[0];
			SearchString += charKey;
			var itemToSelect = DataContext.Contents.FirstOrDefault(c => c.SystemObject.Name.StartsWith(SearchString, StringComparison.OrdinalIgnoreCase));
			if (itemToSelect != null)
			{
				DataContext.SetSelected(itemToSelect);
			}

		}

		void ViewSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			_Config.MainViewHeight = (int)e.NewSize.Height;
			_Config.MainViewWidth = (int)e.NewSize.Width;
		}

		private void UpdateFileList()
		{
			DataContext = new MainWindowDirectoryWrapper(_CurrentDirectory);
			_Config.StartupDirectory = _CurrentDirectory.FullName;
		}

		private void SetSelectedItemInList(IFileSystemObject fileSystemObject)
		{
			var itemToSelect = DataContext.Contents.FirstOrDefault(c => c.SystemObject.Name == fileSystemObject.Name);
			if (itemToSelect == null) return;
			DataContext.SetSelected(itemToSelect);
		}

		private void GotoSelectedItem(object selectedItem)
		{
			MainWindowFile selected = selectedItem as MainWindowFile;
			if (selected == null) return;
			IFileSystemObject selectedSystemObject = selected.SystemObject;
			if (selectedSystemObject is IDirectoryObject)
			{
				_CurrentDirectory = selectedSystemObject as IDirectoryObject;
				UpdateFileList();
			}
			else
			{
				IFileObject file = selectedSystemObject as IFileObject;
				IDirectoryObject dir = file.ParentDirectory;
				_WindowManager.ShowImageWindow(new ImageDirectory(dir), file.Name);
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
			var currentDirectory = _CurrentDirectory.ParentDirectory;
			if (currentDirectory != null)
			{
				var prevDirectory = _CurrentDirectory;
				_CurrentDirectory = currentDirectory;
				UpdateFileList();
				SetSelectedItemInList(prevDirectory);
			}
		}

		private void CurrentDirectoryEnter()
		{
			_CurrentDirectory = _Repo.GetDirectory(_View.CurrentDirectory);
			UpdateFileList();
		}
	}

}
