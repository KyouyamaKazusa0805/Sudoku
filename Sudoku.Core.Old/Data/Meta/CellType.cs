using System;

namespace Sudoku.Data.Meta
{
	[Flags]
	public enum CellType
	{
		Empty = 0,
		Given = 1,
		Modifiable = 2,
		Value = Given | Modifiable
	}
}
