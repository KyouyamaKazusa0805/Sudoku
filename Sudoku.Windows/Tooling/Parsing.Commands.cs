using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Solving;
using static System.Text.RegularExpressions.RegexOptions;
using static Sudoku.Data.ConclusionType;

namespace Sudoku.Windows.Tooling
{
	partial class Parsing
	{
		private static Predicate<TechniqueInfo>? Parse_EliminationContainsCandidate(string s)
		{
			var match = Regex.Match(s, $@"elimination\s+contains\s+({RegularExpressions.Candidate})", IgnoreCase);
			if (match.Success)
			{
				int candidate = AsCandidate(match.Groups[1].Value);
				return info =>
					info.Conclusions.Any(
						c =>
						{
							var (type, cell, digit) = c;
							return cell * 9 + digit == candidate && type == Elimination;
						});
			}

			return null;
		}

		private static Predicate<TechniqueInfo>? Parse_AssignmentIsCandidats(string s)
		{
			var match = Regex.Match(s, $@"assignment\s+is\s+({RegularExpressions.Candidate})", IgnoreCase);
			if (match.Success)
			{
				int candidate = AsCandidate(match.Groups[1].Value);
				return info =>
				{
					if (info.Conclusions.Count == 1)
					{
						var (type, cell, digit) = info.Conclusions[0];
						return cell * 9 + digit == candidate && type == Data.ConclusionType.Assignment;
					}

					return false;
				};
			}

			return null;
		}

		private static Predicate<TechniqueInfo>? Parse_EliminationIsCandidate(string s)
		{
			var match = Regex.Match(s, $@"elimination\s+is\s+({RegularExpressions.Candidate})", IgnoreCase);
			if (match.Success)
			{
				int candidate = AsCandidate(match.Groups[1].Value);
				return info =>
				{
					if (info.Conclusions.Count == 1)
					{
						var (type, cell, digit) = info.Conclusions[0];
						return cell * 9 + digit == candidate && type == Elimination;
					}

					return false;
				};
			}

			return null;
		}

		private static Predicate<TechniqueInfo>? Parse_TechniqueUsesCandidate(string s)
		{
			var match = Regex.Match(s, $@"technique\s+uses\s+({RegularExpressions.Candidate})", IgnoreCase);
			if (match.Success)
			{
				int candidate = AsCandidate(match.Groups[1].Value);
				return i => i.Views.Any(v => v.Candidates?.Any(p => p.Value == candidate) ?? false);
			}

			return null;
		}

		private static Predicate<TechniqueInfo>? Parse_TechniqueUsesCell(string s)
		{
			var match = Regex.Match(s, $@"technique\s+uses\s+({RegularExpressions.Cell})", IgnoreCase);
			if (match.Success)
			{
				int cell = AsCell(match.Groups[1].Value);
				return i => i.Views.Any(v => v.Cells?.Any(p => p.Value == cell) ?? false);
			}

			return null;
		}

		private static Predicate<TechniqueInfo>? Parse_TechniqueUsesRegion(string s)
		{
			var match = Regex.Match(s, $@"technique\s+uses\s+({RegularExpressions.Region})", IgnoreCase);
			if (match.Success)
			{
				int region = AsRegion(match.Groups[1].Value);
				return i => i.Views.Any(v => v.Regions?.Any(p => p.Value == region) ?? false);
			}

			return null;
		}
	}
}
