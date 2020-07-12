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
					?? Parse_AssignmentIsCandidats(s)
					?? Parse_EliminationIsCandidate(s)
					?? Parse_TechniqueUsesCandidate(s)
					?? Parse_TechniqueUsesCell(s);

		/// <summary>
		/// Parse a string as a coordinate.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The coordinate value (between 0 and 80).</returns>
		private static int AsCell(string? @this)
		{
			// Null or empty or write space.
			if (string.IsNullOrWhiteSpace(@this))
			{
				return -1;
			}

			// Value 0..81.
			if (byte.TryParse(@this, out byte value) && value >= 0 && value < 81)
			{
				return value;
			}

			// RxCy or rxcy.
			var regex = new Regex(RegularExpressions.Cell);
			string s = @this.Trim();
			var match = regex.Match(s);
			if (match.Success)
			{
				var groups = match.Groups;
				if (groups.Count == 3)
				{
					return (int.Parse(groups[1].Value) - 1) * 9 + int.Parse(groups[2].Value) - 1;
				}

				return -1;
			}

			// Unknown case.
			return -1;
		}

		/// <summary>
		/// Parse a string as a candidate.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The coordinate value (between 0 and 728).</returns>
		private static int AsCandidate(string? @this)
		{
			// Null or empty or write space.
			if (string.IsNullOrWhiteSpace(@this))
			{
				return -1;
			}

			// Value 0..729.
			if (short.TryParse(@this, out short value) && value >= 0 && value < 729)
			{
				return value;
			}

			// RxCy or rxcy.
			var regex = new Regex(RegularExpressions.Candidate);
			string s = @this.Trim();
			var match = regex.Match(s);
			if (match.Success)
			{
				var groups = match.Groups;
				if (groups.Count == 4)
				{
					return (int.Parse(groups[1].Value) - 1) * 81 + (int.Parse(groups[2].Value) - 1) * 9 + int.Parse(groups[3].Value) - 1;
				}

				return -1;
			}

			// Unknown case.
			return -1;
		}
	}
}
