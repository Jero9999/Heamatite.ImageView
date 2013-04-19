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
		private ImageView ImageWindow
		{
			get
			{
				if (_ImageWindow == null) { _ImageWindow = new ImageView(); }
				return _ImageWindow;
			}
		}

		public void ShowImageWindow()
		{
			ImageWindow.Show();
			MainWindow.Hide();
		}

		private MainWindow _MainWindow { get; set; }
		private MainWindow MainWindow
		{
			get
			{
				if (_MainWindow == null) { _MainWindow = new MainWindow(); }
				return _MainWindow;
			}
		}

		public void ShowMainWindow()
		{
			MainWindow.Show();
			ImageWindow.Hide();
		}
	}
}
