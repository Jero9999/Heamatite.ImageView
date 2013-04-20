using Heamatite.IO;
using Heamatite.ViewInterfaces;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Heamatite.View
{

	/// <summary>
	/// Interaction logic for ImageView.xaml
	/// </summary>
	public partial class ImageView : Window, IImageView
	{
		public ImageView()
		{
			InitializeComponent();
			
			this.KeyDown += ImageView_KeyDown;
		}

		public event EventHandler FullScreenChanged;
		public bool IsFullScreen
		{
			get
			{
				return WindowStyle == System.Windows.WindowStyle.None && WindowState == System.Windows.WindowState.Maximized;
			}
			set
			{
				if (value)
				{
					this.WindowStyle = System.Windows.WindowStyle.None;
					this.WindowState = System.Windows.WindowState.Maximized;
				}
				else
				{
					this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
					this.WindowState = System.Windows.WindowState.Normal;
				}
				OnFullScreenChanged();
			}
		}

		protected void OnFullScreenChanged()
		{
			if (FullScreenChanged != null)
			{
				FullScreenChanged(this, new EventArgs());
			}
		}

		void ImageView_KeyDown(object sender, KeyEventArgs e)
		{
			//TODO
			switch (e.Key)
			{
				case Key.Home:
					FirstImage();
					break;
				case Key.End:
					LastImage();
					break;
				case Key.Enter:
					WindowManager.ShowMainWindow();
					return;
				case Key.F:
					IsFullScreen = !IsFullScreen;
					break;
			}
		}

		public Action FirstImage { get; set; }
		public Action LastImage { get; set; }
		public Action PreviousImage { get; set; }
		public Action NextImage { get; set; }

		private void BrowseForwardExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			NextImage();
		}

		private void BrowseForwardCanExecuted(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void BrowseBackExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			PreviousImage();
		}

		private void BrowseBackCanExecuted(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}


	}
}
