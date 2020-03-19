using System;
using System.Drawing;

namespace Sudoku.Forms
{
	public class SizeChangedEventArgs : EventArgs
	{
		public SizeChangedEventArgs(Size size) => Size = size;


		public Size Size { get; }
	}
}
