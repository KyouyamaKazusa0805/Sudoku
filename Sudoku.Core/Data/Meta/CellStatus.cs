using System;

namespace Sudoku.Data.Meta
{
	[Flags]
	public enum CellStatus : byte
	{
		Empty = 1,
		Modifiable = 2,
		Given = 4,
		All = Empty | Modifiable | Given
	}
}
