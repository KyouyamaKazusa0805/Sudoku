namespace Sudoku.Data.Literals
{
	public static class Regexes
	{
		/// <summary>
		/// Sudoku susser format. Dot `<c>.</c>` or Zero `<c>0</c>`
		/// stands for an empty cell, and other digit will be
		/// given value in its own cell.
		/// </summary>
		public const string Susser = @"[\d\.\+]{81,}";

		/// <summary>
		/// Sudoku susser extended format, which supports custom eliminations.
		/// </summary>
		public const string ExtendedSusser = @"[\d\.\+]{81,}(\:([1-9]{3})+(\s+[1-9]{3})*)?";

		/// <summary>
		/// Sudoku susser format (but <b>only</b> custom eliminations).
		/// </summary>
		public const string SusserElims = @"(?<=\:)([1-9]{3})+(\s+[1-9]{3})*";

		/// <summary>
		/// An elimination group in susser format.
		/// </summary>
		public const string SusserElimsGroup = @"[1-9]{3}";

		/// <summary>
		/// A value group in pencil-marked grid.
		/// </summary>
		public const string PMGridValuesGroup = @"(\<[1-9]\>|[1-9]+)";

		/// <summary>
		/// Candidate field parser string.
		/// </summary>
		public const string CandidateField = @"[1-9]{,9}";

		/// <summary>
		/// A candidate.
		/// </summary>
		public const string Candidate = @"[1-9][Rr][1-9][Cc][1-9]";

		/// <summary>
		/// A cell.
		/// </summary>
		public const string Cell = @"[Rr][1-9][Cc][1-9]";
	}
}
