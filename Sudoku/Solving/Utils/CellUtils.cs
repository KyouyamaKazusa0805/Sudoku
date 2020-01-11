using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	public static class CellUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetOffset(int row, int column) => row * 9 + column;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToString(int cellOffset) => $"r{cellOffset / 9 + 1}c{cellOffset % 9 + 1}";

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToFullString(int cellOffset) =>
			$"r{cellOffset / 9 + 1}c{cellOffset % 9 + 1}b{cellOffset / 3 * 3 + cellOffset % 3 + 1}";
	}
}
