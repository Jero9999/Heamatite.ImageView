﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Heamatite.ViewInterfaces
{
	public interface IImageView
	{
		double Width { set; }
		double Height { set; }
		bool IsFullScreen { get; set; }

		Action FirstImage { get; set; }
		Action LastImage { get; set; }
		Action PreviousImage { get; set; }
		Action NextImage { get; set; }

		object DataContext { get; set; }
	}

}
