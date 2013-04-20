using Heamatite.IO;
using Heamatite.ViewInterfaces;
using System;
using System.Diagnostics;
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

		public Action FirstImage { get; set; }
		public Action LastImage { get; set; }
		public Action PreviousImage { get; set; }
		public Action NextImage { get; set; }
		public Action FullScreenCommand { get; set; }
		public Action EnterCommand { get; set; }

		private void BrowseCommandExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			RoutedUICommand command = e.Command as RoutedUICommand;
			if (command == null) return;
			switch (command.Text)
			{
				case "Forward":
					NextImage();
					break;
				case "Back":
					PreviousImage();
					break;
				case "First Page":
					FirstImage();
					break;
				case "Last Page":
					LastImage();
					break;
				default:
					break;
			}
		}

		private void SwitchWindow_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			EnterCommand();
		}

		private void ToggleFullScreen_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			FullScreenCommand();
		}
	}

}
