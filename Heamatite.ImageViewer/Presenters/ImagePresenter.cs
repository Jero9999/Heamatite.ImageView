using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Heamatite.View;
using Heamatite.IO;
using Heamatite.ViewInterfaces;
using System.Windows.Media.Imaging;
using System.IO;

namespace Heamatite.View.Presenters
{

	public class ImagePresenter
	{
		private IImageView _View;
		IConfiguration _Config;
		private IImageDirectory _ImageDirectory;

		public ImagePresenter(IImageView view, IConfiguration config, IImageDirectory imageDirectory)
		{
			_View = view;
			_Config = config;
			_ImageDirectory = imageDirectory;

			SetupView();
		}

		private void SetupView()
		{
			_View.SizeChanged += ViewSizeChanged;
			_View.FullScreenChanged += ViewFullscreenChanged;
			_View.IsFullScreen = _Config.ImageViewFullScreen;
			_View.Width = _Config.ImageViewWidth;
			_View.Height = _Config.ImageViewHeight;
			_View.NextImage = NextImage;
			_View.PreviousImage = PreviousImage;
			_View.FirstImage = FirstImage;
			_View.LastImage = LastImage;
			_View.DataContext = null;
			_View.EnterCommand = () => WindowManager.ShowMainWindow();
			_View.FullScreenCommand = () => _View.IsFullScreen = !_View.IsFullScreen;
		}

		private void ViewFullscreenChanged(object sender, EventArgs e)
		{
			_Config.ImageViewFullScreen = _View.IsFullScreen;
		}

		void ViewSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
		{
			_Config.ImageViewHeight = (int)e.NewSize.Height;
			_Config.ImageViewWidth = (int)e.NewSize.Width;
		}

		private void FirstImage()
		{
			IImageFile imageFile = _ImageDirectory.First();
			ShowImage(imageFile);
		}

		private void LastImage()
		{
			IImageFile imageFile = _ImageDirectory.Last();
			ShowImage(imageFile);
		}

		private void PreviousImage()
		{
			IImageFile imageFile = _ImageDirectory.Previous();
			ShowImage(imageFile);
		}

		private void NextImage()
		{
			IImageFile imageFile = _ImageDirectory.Next();
			ShowImage(imageFile);
		}

		private void ShowImage(IImageFile imageFile)
		{
			_View.DataContext = imageFile;
		}

		internal void ResetImage(ImageDirectory currentDirectory, string initialFilename)
		{
			_ImageDirectory = currentDirectory;
			_ImageDirectory.SetCurrent(initialFilename);
			ShowImage(_ImageDirectory.Current());
		}

	}
}
