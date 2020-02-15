using System;
using Sudoku.Data.Meta;

namespace Sudoku.Data
{
	public class CellTypeChangingEventArgs : EventArgs
	{
		public CellTypeChangingEventArgs(CellType cellType) => CellType = cellType;


		public bool Cancel { get; set; }

		public CellType CellType { get; }
	}
}
