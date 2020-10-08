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
	}
}
