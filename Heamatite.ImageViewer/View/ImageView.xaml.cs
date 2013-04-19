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
			}
		}

		void ImageView_KeyDown(object sender, KeyEventArgs e)
		{
			//TODO
			switch (e.Key)
			{
				case Key.Space:
				case Key.Right:
				case Key.Down:
				case Key.PageDown:
					NextImage();
					break;
				case Key.Back:
				case Key.Up:
				case Key.Left:
				case Key.PageUp:
					PreviousImage();
					break;
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


	}
}
