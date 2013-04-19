using Heamatite.IO;
using Heamatite.View.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.View
{
	public class WindowManager
	{
		private static WindowManager _Instance;
		private static WindowManager Instance
		{
			get
			{
				if (_Instance == null) { _Instance = new WindowManager(); }
				return _Instance;
			}
		}

		private ImageView _ImageWindow { get; set; }
		private ImagePresenter _ImagePresenter { get; set; }
		private ImageView ImageWindow
		{
			get
			{
				if (_ImageWindow == null) { 
					_ImageWindow = new ImageView();
					_ImagePresenter = new ImagePresenter(_ImageWindow, Config, null);
				}
				return _ImageWindow;
			}
		}

		private IConfiguration Config;
		private FileRepository FileRepository;
		public WindowManager()
		{
			Config = new Configuration1();
			FileRepository = new FileRepository();
		}

		public static void ShowImageWindow(ImageDirectory currentDirectory = null, string initialFilename = null)
		{
			var dummy = Instance.ImageWindow;
			Instance._ImagePresenter.ResetImage(currentDirectory, initialFilename);
			Instance.ImageWindow.Show();
			Instance.MainWindow.Hide();
			
		}

		private MainWindow _MainWindow { get; set; }
		private MainPresenter _MainPresenter { get; set; } 
		private MainWindow MainWindow
		{
			get
			{
				if (_MainWindow == null) { 
					
					_MainWindow = new MainWindow();
					_MainPresenter = new MainPresenter(_MainWindow, Config, FileRepository);
				}
				return _MainWindow;
			}
		}

		public static void ShowMainWindow()
		{
			Instance.MainWindow.Show();
			Instance.ImageWindow.Close();
			Instance._ImageWindow = null;
			Instance._ImagePresenter = null;
		}
	}
}
