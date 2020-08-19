using System;

namespace Sudoku.Data.Meta
{
	[Flags]
	public enum GridEqualityOptions
	{
		None = 0,
		CheckGivens = 0b001,
		CheckModifiables = 0b010,
		CheckValues = 0b011,
		CheckCandidates = 0b100,
		All = 0b111,
	}
}
