namespace Sudoku.Constants
{
	/// <summary>
	/// The class that stores all regular expressions used in this solution.
	/// </summary>
	internal static class RegularExpressions
	{
		/// <summary>
		/// Indicates each candidates group in the PM grid.
		/// </summary>
		public const string PmGridCandidates = @"[1-9]{1,9}";

		/// <summary>
		/// Indicates the eliminations in the extended susser format.
		/// </summary>
		public const string ExtendedSusserEliminations = @"(?<=\:)(\d{3}\s+)*\d{3}";

		/// <summary>
		/// Indicates the normal sudoku grid table (with only blank cells and numbers).
		/// </summary>
		public const string SimpleTable = @"([\d\.\+]{9}(\n|\r)){8}[\d\.\+]{9}";

		/// <summary>
		/// Indicates the susser format.
		/// </summary>
		public const string Susser = @"[\d\.\+]{81,}(\:(\d{3}\s+)*\d{3})?";

		/// <summary>
		/// Indicates the regular expression of a digit or empty cell placeholder.
		/// </summary>
		public const string DigitOrEmptyCell = @"(\+?\d|\.)";

		/// <summary>
		/// Indicates the unit in the basic PM grid.
		/// </summary>
		public const string PmGridUnit_Old = @"(\<\d\>|\*\d\*|\d{1,9})";

		/// <summary>
		/// Indicates the unit in the extended PM grid.
		/// </summary>
		public const string PmGridUnit = @"(\<\d\>|\*\d\*|\d*[\+\-]?\d+)";

		/// <summary>
		/// Indicates the candidate unit that used in the elimination list.
		/// </summary>
		public const string ThreeDigitsCandidate = @"\d{3}";

		/// <summary>
		/// Indicates the candidate list unit in the PM grid.
		/// </summary>
		public const string PmGridCandidatesUnit = @"\d*[\-\+]?\d+";

		/// <summary>
		/// Indicates a cell string.
		/// </summary>
		public const string Cell = @"[Rr]([1-9])[Cc]([1-9])";

		/// <summary>
		/// Indicates the region string.
		/// </summary>
		public const string Region = @"([Rr][1-9]|[Cc][1-9]|[Bb][1-9])";

		/// <summary>
		/// Indicates a candidate string.
		/// </summary>
		public const string Candidate = @"[Rr]([1-9])[Cc]([1-9])\(([1-9])\)";

		/// <summary>
		/// Indicates the regular expression to match a digit.
		/// </summary>
		public const string Digit = @"\d";

		/// <summary>
		/// Indicates all null lines and header spaces in their lines.
		/// </summary>
		public const string NullLinesOrHeaderSpaces = @"(^\s*|(?<=\r\n)\s+)";
	}
}
