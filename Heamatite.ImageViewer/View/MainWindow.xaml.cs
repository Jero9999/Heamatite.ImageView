using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Heamatite.View
{
	using Heamatite.IO;
	using Heamatite.IoSystem;
	using Heamatite.ViewInterfaces;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IMainView
	{
		#region Window
		public MainWindow()
		{
			InitializeComponent();

			DirectoryList.Focus();
		}

		public string CurrentDirectory
		{
			get { return CurrentDirectoryControl.Text; }
			set { CurrentDirectoryControl.Text = value; }
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
		}

		#endregion

		public Action CurrentDirectoryEnter { get; set; }
		public Action MoveToParent { get; set; }

		public Action UpdateFileList { get; set; }
		public Action<object> GotoSelectedItem { get; set; }
		public Action<int, int, Action<int>> MoveRight { get; set; }
		public Action<int, Action<int>> MoveLeft { get; set; }

		#region CurrentDirectory
		private void CurrentDirectory_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.IsRepeat) return;
			switch (e.Key)
			{
				case Key.Enter:
					CurrentDirectoryEnter();
					break;
			}
		}


		#endregion

		#region DirectoryList
		private void DirectoryList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			GotoSelectedItem(((ListBox)sender).SelectedItem);
		}

		private void DirectoryList_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.IsRepeat) return;
			switch (e.Key)
			{
				case Key.Enter:
					GotoSelectedItem(((ListBox)sender).SelectedItem);
					break;
				case Key.Back:
					MoveToParent();
					break;
				case Key.Left:
					MoveLeft(DirectoryList.SelectedIndex, newIndex => DirectoryList.SelectedIndex = newIndex);
					break;
				case Key.Right:
					MoveRight(DirectoryList.SelectedIndex, DirectoryList.Items.Count, newIndex => DirectoryList.SelectedIndex = newIndex);
					break;
			}
		}

		private void DirectoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ResetDirectoryListFocus();
		}

		private void ResetDirectoryListFocus()
		{
			if (DirectoryList.SelectedItem == null) { return; }
			ListBoxItem listBoxItem = (ListBoxItem)DirectoryList.ItemContainerGenerator.ContainerFromItem(DirectoryList.SelectedItem);
			listBoxItem.Focus();
		}
		#endregion



	}



}
