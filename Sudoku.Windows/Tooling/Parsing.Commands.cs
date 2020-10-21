using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Solving;
using static System.Text.RegularExpressions.RegexOptions;
using CT = Sudoku.Data.ConclusionType;

namespace Sudoku.Windows.Tooling
{
	partial class Parsing
	{
		private static Predicate<TechniqueInfo>? Parse_EliminationContainsCandidate(string s) =>
			$@"elimination\s+contains\s+({RegularExpressions.Candidate})" is var pattern
			&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
			&& AsCandidate(match.Groups[1].Value) is var candidate
				? info =>
					info.Conclusions.Any(
						c => c is var (type, cell, digit)
						&& (cell * 9 + digit, type) == (candidate, CT.Elimination))
				: null;

		private static Predicate<TechniqueInfo>? Parse_AssignmentIsCandidates(string s) =>
			$@"assignment\s+is\s+({RegularExpressions.Candidate})" is var pattern
			&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
			&& AsCandidate(match.Groups[1].Value) is var candidate
				? info =>
					info.Conclusions is { Count: 1 } conc && conc[0] is var (type, cell, digit)
					&& (cell * 9 + digit, type) == (candidate, CT.Assignment)
				: null;

		private static Predicate<TechniqueInfo>? Parse_EliminationIsCandidate(string s) =>
			$@"elimination\s+is\s+({RegularExpressions.Candidate})" is var pattern
			&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
			&& AsCandidate(match.Groups[1].Value) is var candidate
				? info =>
					info.Conclusions is { Count: 1 } conc && conc[0] is var (type, cell, digit)
					&& (cell * 9 + digit, type) == (candidate, CT.Elimination)
				: null;

		private static Predicate<TechniqueInfo>? Parse_TechniqueUsesCandidate(string s) =>
			$@"technique\s+uses\s+({RegularExpressions.Candidate})" is var pattern
			&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
			&& AsCandidate(match.Groups[1].Value) is var candidate
				? i => i.Views.Any(v => v.Candidates?.Any(p => p.Value == candidate) ?? false)
				: null;

		private static Predicate<TechniqueInfo>? Parse_TechniqueUsesCell(string s) =>
			$@"technique\s+uses\s+({RegularExpressions.Cell})" is var pattern
			&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
			&& AsCell(match.Groups[1].Value) is var cell
				? i => i.Views.Any(v => v.Cells?.Any(p => p.Value == cell) ?? false)
				: null;

		private static Predicate<TechniqueInfo>? Parse_TechniqueUsesRegion(string s) =>
			$@"technique\s+uses\s+({RegularExpressions.Region})" is var pattern
			&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
			&& AsRegion(match.Groups[1].Value) is var region
				? i => i.Views.Any(v => v.Regions?.Any(p => p.Value == region) ?? false)
				: null;
	}
}
