using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for subset (hidden or naked) technique.
	/// </summary>
	public static class SubsetUtils
	{
		/// <summary>
		/// All subset names.
		/// </summary>
		private static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};


		/// <summary>
		/// Gets the name by its size.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <returns>The name.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetNameBy(int size) => SubsetNames[size];
	}
}
