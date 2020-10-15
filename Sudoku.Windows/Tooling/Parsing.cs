using System;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Solving;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Provides a parsing way.
	/// </summary>
	internal static partial class Parsing
	{
		/// <summary>
		/// Parse the string as a condition.
		/// </summary>
		/// <param name="s">The string.</param>
		/// <returns>The predicate.</returns>
		public static Predicate<TechniqueInfo>? ToCondition(string? s) =>
			string.IsNullOrWhiteSpace(s)
				? _ => true
				: Parse_EliminationContainsCandidate(s)
					?? Parse_AssignmentIsCandidates(s)
					?? Parse_EliminationIsCandidate(s)
					?? Parse_TechniqueUsesRegion(s)
					?? Parse_TechniqueUsesCandidate(s)
					?? Parse_TechniqueUsesCell(s);

		/// <summary>
		/// Parse a string as a coordinate.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The coordinate value (between 0 and 80).</returns>
		private static int AsCell(string? @this) =>
			string.IsNullOrWhiteSpace(@this)
				? -1
				: byte.TryParse(@this, out byte value) && value is >= 0 and < 81
					? value
					: Regex.Match(@this.Trim(), RegularExpressions.Cell) is { Success: true } match
						? match.Groups is { Count: 3 } groups
							? (int.Parse(groups[1].Value) - 1) * 9 + int.Parse(groups[2].Value) - 1
							: -1
						: -1;

		/// <summary>
		/// Parse a string as a candidate.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The coordinate value (between 0 and 728).</returns>
		private static int AsCandidate(string? @this) =>
			string.IsNullOrWhiteSpace(@this)
				? -1
				: short.TryParse(@this, out short value) && value is >= 0 and < 729
					? value
					: Regex.Match(@this.Trim(), RegularExpressions.Candidate) is { Success: true } match
						? match.Groups is { Count: 4 } groups
							? (int.Parse(groups[1].Value) - 1) * 81
								+ (int.Parse(groups[2].Value) - 1) * 9
								+ int.Parse(groups[3].Value) - 1
							: -1
						: -1;

		/// <summary>
		/// Parse a string as a region.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The region value (between 0 and 26).</returns>
		private static int AsRegion(string @this) =>
			string.IsNullOrWhiteSpace(@this)
				? -1
				: byte.TryParse(@this, out byte value) && value is >= 0 and < 27
					? value
					: Regex.Match(@this.Trim(), RegularExpressions.Region) is { Success: true } match
						? match.Value is { Length: 2 } str && (str[0], str[1]) is (var l, var r)
							? l switch { 'b' => r - '1', 'r' => 9 + r - '1', 'c' => 18 + r - '1', _ => throw Throwings.ImpossibleCase }
							: -1
						: -1;
	}
}
