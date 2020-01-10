namespace Sudoku.Data.Literals
{
	public static class GridFormats
	{
		/// <summary>
		/// Normal format (susser) with dots
		/// '<c>.</c>' for empty cells.
		/// </summary>
		public const string SusserDot = "g.";

		/// <summary>
		/// Normal format (Susser) with zeros
		/// '<c>0</c>' for empty cells.
		/// </summary>
		public const string SusserZero = "g0";

		/// <summary>
		/// Normal format (Susser) with modifiable values
		/// and dots '<c>.</c>' for empty cells.
		/// </summary>
		public const string SusserPlusDot = "g+.";

		/// <summary>
		/// Normal format (Susser) with modifiable values
		/// and zeros '<c>0</c>' for empty cells.
		/// </summary>
		public const string SusserPlusZero = "g+0";

		/// <summary>
		/// Normal format (Susser) with modifiable values,
		/// dots '<c>.</c>' for empty cells and eliminations after.
		/// </summary>
		public const string SusserPlusDotWithElims = "g+.:";

		/// <summary>
		/// Normal format (Susser) with modifiable values,
		/// zeros '<c>0</c>' for empty cells and eliminations after.
		/// </summary>
		public const string SusserPlusZeroWithElims = "g+0:";

		/// <summary>
		/// Candidate format (Pencil-marked grid format).
		/// </summary>
		public const string PmGrid = "c";
	}
}
