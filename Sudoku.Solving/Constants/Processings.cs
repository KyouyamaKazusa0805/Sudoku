using Sudoku.Data;
using Sudoku.Extensions;

namespace Sudoku.Solving.Constants
{
	/// <summary>
	/// Provides the constants and read-only values in the current project.
	/// Of course, the class will also provides you with some method to get the results
	/// such as the chain length rating calculation.
	/// </summary>
	public static class Processings
	{
		/// <summary>
		/// The names of all subsets by their sizes.
		/// </summary>
		public static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};

		/// <summary>
		/// The names of all fishes by their sizes.
		/// </summary>
		public static readonly string[] FishNames =
		{
			string.Empty, "Cyclopsfish", "X-Wing", "Swordfish", "Jellyfish",
			"Squirmbag", "Whale", "Leviathan", "Octopus", "Dragon"
		};

		/// <summary>
		/// The names of all regular wings by their sizes.
		/// </summary>
		public static readonly string[] RegularWingNames =
		{
			string.Empty, string.Empty, string.Empty, string.Empty, "WXYZ-Wing", "VWXYZ-Wing",
			"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing", "RSTUVWXYZ-Wing"
		};


		/// <summary>
		/// Get the mask that is a result after the bitwise or operation processed all cells
		/// in the specified map.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="map">The map.</param>
		/// <returns>The result.</returns>
		public static short BitwiseOrMasks(IReadOnlyGrid grid, GridMap map)
		{
			short mask = 0;
			foreach (int cell in map)
			{
				mask |= grid.GetCandidateMask(cell);
			}

			return mask;
		}

		/// <summary>
		/// Get extra difficulty rating for a chain node sequence.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <returns>The difficulty.</returns>
		public static decimal GetExtraDifficultyByLength(int length)
		{
			decimal added = 0;
			int ceil = 4;
			for (bool isOdd = false; length > ceil; isOdd.Flip())
			{
				added += .1M;
				ceil = isOdd ? (ceil << 2) / 3 : ceil * 3 >> 1;
			}
			return added;

			#region Obsolete code
			// I have seen the code of Sudoku Explainer.
			// The calculation formula (older one) is:
			//int[] steps =
			//{
			//	4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128,
			//	192, 256, 384, 512, 768, 1024, 1536, 2048,
			//	3072, 4096, 6144, 8192
			//};
			//decimal added = 0;
			//for (int index = 0; index < steps.Length && length > steps[index]; index++)
			//{
			//	added += .1M;
			//}
			//return added;
			#endregion
		}
	}
}
