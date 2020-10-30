using System;
using System.Linq;
using System.Text.RegularExpressions;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Solving;
using static System.Text.RegularExpressions.RegexOptions;
using CT = Sudoku.Data.ConclusionType;

namespace Sudoku.Windows.Tooling
{
	partial class Parsing
	{
		private static unsafe Predicate<TechniqueInfo>? Parse_EliminationContainsCandidate(string s)
		{
			return $@"elimination\s+contains\s+({RegularExpressions.Candidate})" is var pattern
				&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
				&& AsCandidate(match.Groups[1].Value) is var candidate
				? info => info.Conclusions.Any(&internalChecking, candidate)
				: null;

			static bool internalChecking(Conclusion c, in int candidate) =>
				c is var (type, cell, digit) && (cell * 9 + digit, type) == (candidate, CT.Elimination);
		}

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

		private static unsafe Predicate<TechniqueInfo>? Parse_TechniqueUsesCandidate(string s)
		{
			return $@"technique\s+uses\s+({RegularExpressions.Candidate})" is var pattern
				&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
				&& AsCandidate(match.Groups[1].Value) is var candidate
				? i => i.Views.Any(&internalCheckingOuter, candidate)
				: null;

			static bool internalCheckingInner(DrawingInfo p, in int candidate) => p.Value == candidate;

			static unsafe bool internalCheckingOuter(View v, in int candidate) =>
				v.Candidates?.Any(&internalCheckingInner, candidate) ?? false;
		}

		private static unsafe Predicate<TechniqueInfo>? Parse_TechniqueUsesCell(string s)
		{
			return $@"technique\s+uses\s+({RegularExpressions.Cell})" is var pattern
				&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
				&& AsCell(match.Groups[1].Value) is var cell
				? i => i.Views.Any(&internalCheckingOuter, cell)
				: null;

			static bool internalCheckingInner(DrawingInfo p, in int cell) => p.Value == cell;

			static unsafe bool internalCheckingOuter(View v, in int cell) =>
				v.Cells?.Any(&internalCheckingInner, cell) ?? false;
		}

		private static unsafe Predicate<TechniqueInfo>? Parse_TechniqueUsesRegion(string s)
		{
			return $@"technique\s+uses\s+({RegularExpressions.Region})" is var pattern
				&& Regex.Match(s, pattern, IgnoreCase) is { Success: true } match
				&& AsRegion(match.Groups[1].Value) is var region
				? i => i.Views.Any(&internalCheckingOuter, region)
				: null;

			static bool internalCheckingInner(DrawingInfo p, in int region) => p.Value == region;

			static unsafe bool internalCheckingOuter(View v, in int region) =>
				v.Regions?.Any(&internalCheckingInner, region) ?? false;
		}
	}
}
