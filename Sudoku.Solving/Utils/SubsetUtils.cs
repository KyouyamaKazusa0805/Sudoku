using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	public static class SubsetUtils
	{
		private static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetNameBy(int size) => SubsetNames[size];
	}
}
