using Autofac;
using Heamatite.IO;
using Heamatite.IoSystem;
using Heamatite.View.Presenters;
using Heamatite.ViewInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heamatite.View
{
	public interface IWindowManager
	{
		void ShowImageWindow(IImageDirectory currentDirectory = null, string initialFilename = null);
		void ShowMainWindow();
	}

	public class WindowManager: IWindowManager
	{

		private IImageView _ImageWindow { get; set; }
		private IImagePresenter _ImagePresenter { get; set; }
		private IImageView ImageWindow
		{
			get
			{
				if (_ImageWindow == null) {
					_ImagePresenter = IocContainer.Resolve<IImagePresenter>();
					_ImageWindow = _ImagePresenter.View;
				}
				return _ImageWindow;
			}
		}

		private IConfiguration Config;
		private IIoRepository FileRepository;
		private IContainer IocContainer;

		public WindowManager(
			IConfiguration configuration, 
			IIoRepository repository)
		{
			IocContainer = Heamatite.ImageViewer.App.Container;
			Config = configuration;
			FileRepository = repository;
		}

		public void ShowImageWindow(IImageDirectory currentDirectory = null, string initialFilename = null)
		{
			ImageWindow.Show();
			_ImagePresenter.ResetImage(currentDirectory, initialFilename);
			MainWindow.Close();
			_MainWindow = null;
			_MainPresenter = null;
		}

		private IMainView _MainWindow { get; set; }
		private IMainPresenter _MainPresenter { get; set; }
		private IMainView MainWindow
		{
			get
			{
				if (_MainWindow == null) {
					_MainPresenter = IocContainer.Resolve<IMainPresenter>();
					_MainWindow = _MainPresenter.View;
				}
				return _MainWindow;
			}
		}

		public void ShowMainWindow()
		{
			MainWindow.Show();
			ImageWindow.Close();
			_ImageWindow = null;
			_ImagePresenter = null;
		}
	}
}
