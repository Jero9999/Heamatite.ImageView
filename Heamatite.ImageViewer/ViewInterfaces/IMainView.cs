using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Heamatite.ViewInterfaces
{
	public interface IMainView
	{
		double Width { set; }
		double Height { set; }
		object DataContext { get; set; }
		string CurrentDirectory { get; set; }

		event SizeChangedEventHandler SizeChanged;
		event KeyEventHandler KeyDown;

		Action UpdateFileList { get; set; }
		Action<object> GotoSelectedItem { get; set; }
		Action<int, int, Action<int>> MoveRight { get; set; }
		Action<int, Action<int>> MoveLeft { get; set; }
		Action MoveToParent { get; set; }
		Action CurrentDirectoryEnter { get; set; }
		Action<string> KeyboardSearch { get; set; }
	}

}
