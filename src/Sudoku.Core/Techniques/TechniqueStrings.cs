namespace Sudoku.Techniques
{
	/// <summary>
	/// Provides the <see cref="string"/> values to be used.
	/// </summary>
	public static class TechniqueStrings
	{
		/// <summary>
		/// <para>The names of all subsets by their sizes.</para>
		/// <para>
		/// For example, if you want to get the name of the size 3, the code will be
		/// <code>
		/// string name = TechniqueStrings.SubsetNames[3];
		/// </code>
		/// Here the digit <c>3</c> is the size you want to get.
		/// </para>
		/// </summary>
		public static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};

		/// <summary>
		/// <para>The names of all regular wings by their sizes.</para>
		/// <para>
		/// For example, if you want to get the name of the size 3, the code will be
		/// <code>
		/// string name = TechniqueStrings.RegularWingNames[3];
		/// </code>
		/// Here the digit <c>3</c> is the size you want to get.
		/// </para>
		/// </summary>
		public static readonly string[] RegularWingNames =
		{
			string.Empty, string.Empty, string.Empty, string.Empty, "WXYZ-Wing", "VWXYZ-Wing",
			"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing", "RSTUVWXYZ-Wing"
		};
	}
}
