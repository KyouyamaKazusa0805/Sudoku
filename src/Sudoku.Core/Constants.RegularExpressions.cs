namespace Sudoku
{
	partial class Constants
	{
		/// <summary>
		/// The class that stores all regular expressions used in this solution.
		/// </summary>
		internal static class RegularExpressions
		{
			/// <summary>
			/// Indicates a comment line.
			/// </summary>
			[Regex]
			public const string CommentLine = @"(\s//.+|/\*.+|.+\*/)";

			/// <summary>
			/// Indicates each candidates group in the PM grid.
			/// </summary>
			[Regex]
			public const string PmGridCandidates = @"[1-9]{1,9}";

			/// <summary>
			/// Indicates the eliminations in the extended susser format.
			/// </summary>
			[Regex]
			public const string ExtendedSusserEliminations = @"(?<=\:)(\d{3}\s+)*\d{3}";

			/// <summary>
			/// Indicates the normal sudoku grid table (with only blank cells and numbers).
			/// </summary>
			[Regex]
			public const string SimpleTable = @"([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}";

			/// <summary>
			/// Indicates the susser format.
			/// </summary>
			[Regex]
			public const string Susser = @"[\d\.\+]{80,}(\:(\d{3}\s+)*\d{3})?";

			/// <summary>
			/// Indicates the open sudoku format.
			/// </summary>
			[Regex]
			public const string OpenSudoku = @"\d(\|\d){242}";

			/// <summary>
			/// Indicates the regular expression of a digit or empty cell placeholder.
			/// </summary>
			[Regex]
			public const string DigitOrEmptyCell = @"(\+?\d|\.)";

			/// <summary>
			/// Indicates the unit in the basic PM grid.
			/// </summary>
			[Regex, Obsolete("We suggest you don't use this field, and use '" + nameof(PmGridUnit) + "' instead.")]
			public const string PmGridUnit_Old = @"(\<\d\>|\*\d\*|\d{1,9})";

			/// <summary>
			/// Indicates the unit in the extended PM grid.
			/// </summary>
			[Regex]
			public const string PmGridUnit = @"(\<\d\>|\*\d\*|\d*[\+\-]?\d+)";

			/// <summary>
			/// Indicates the candidate unit that used in the elimination list.
			/// </summary>
			[Regex]
			public const string ThreeDigitsCandidate = @"\d{3}";

			/// <summary>
			/// Indicates the candidate list unit in the PM grid.
			/// </summary>
			[Regex]
			public const string PmGridCandidatesUnit = @"\d*[\-\+]?\d+";

			/// <summary>
			/// Indicates a cell string.
			/// </summary>
			[Regex]
			public const string Cell = @"[Rr]([1-9])[Cc]([1-9])";

			/// <summary>
			/// Indicates the cell list string.
			/// </summary>
			[Regex]
			public const string CellOrCellList = @"(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})";

			/// <summary>
			/// Indicates the region string.
			/// </summary>
			[Regex]
			public const string Region = @"([Rr][1-9]|[Cc][1-9]|[Bb][1-9])";

			/// <summary>
			/// Indicates a candidate string.
			/// </summary>
			[Regex]
			public const string Candidate = @"[Rr]([1-9])[Cc]([1-9])\(([1-9])\)";

			/// <summary>
			/// Indicates the candidate list string that matches the triplet form candidate list.
			/// </summary>
			[Regex]
			public const string CandidateListShortForm = @"[1-9]{3}";

			/// <summary>
			/// Indicates the candidate list string that matches the prepositional form candidate list.
			/// </summary>
			[Regex]
			public const string CandidateListPrepositionalForm = @"[1-9]{1,9}(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}|\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}),\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})*\s*\})";

			/// <summary>
			/// Indicates the candidate list string that matches the postpositional form candidate list.
			/// </summary>
			[Regex]
			public const string CandidateListPostpositionalForm = @"\{\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9}),\s*(R[1-9]{1,9}C[1-9]{1,9}|r[1-9]{1,9}c[1-9]{1,9})*\s*\}\([1-9]{1,9}\)";

			/// <summary>
			/// Indicates a candidate list string.
			/// </summary>
			/// <remarks>
			/// The regular expression can match the following formats:
			/// <list type="table">
			/// <item>
			/// <term><c>312</c></term>
			/// <description>The digit 3 in the cell row 1 column 2 (i.e. <c>r1c2</c>).</description>
			/// </item>
			/// <item>
			/// <term><c>{r1c1, r9c4}(13)</c></term>
			/// <description>The digit 1 and 3 in the cell <c>r1c1</c> and <c>r9c4</c>.</description>
			/// </item>
			/// <item>
			/// <term><c>13{r1c1, r9c3}</c></term>
			/// <description>The digit 1 and 3 in the cell <c>r1c1</c> and <c>r9c3</c>.</description>
			/// </item>
			/// </list>
			/// </remarks>
			[Regex]
			public const string CandidateOrCandidateList = "(" + CandidateListShortForm + "|" + CandidateListPrepositionalForm + "|" + CandidateListPostpositionalForm + ")";

			/// <summary>
			/// Indicates the regular expression to match a conclusion.
			/// </summary>
			[Regex]
			public const string Conclusion = @"(R[1-9]C[1-9]|r[1-9]c[1-9])\s*(=|!=|<>)\s*([1-9])";

			/// <summary>
			/// Indicates the regular expression to match a conjugate pair.
			/// </summary>
			[Regex]
			public const string ConjugatePair = @"(R[1-9]C[1-9]|r[1-9]c[1-9])\s*==\s*(R[1-9]C[1-9]|r[1-9]c[1-9])\(([1-9])\)";

			/// <summary>
			/// Indicates the regular expression to match a digit.
			/// </summary>
			[Regex]
			public const string Digit = @"\d";

			/// <summary>
			/// Indicates the regular expression to match a tab character.
			/// </summary>
			[Regex]
			public const string Tab = @"\t";
		}
	}
}
