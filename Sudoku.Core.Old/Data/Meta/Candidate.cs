using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;

namespace Sudoku.Data.Meta
{
	public readonly struct Candidate : IEquatable<Candidate>, IComparable<Candidate>
	{
		public Candidate(int row, int column, int digit) : this(new(row, column), digit)
		{
		}

		public Candidate(Cell cell, int digit) => (Cell, Digit) = (cell, digit);


		public int Digit { get; }

		public int Row => Cell.Row;

		public int Column => Cell.Column;

		public int Block => Cell.Block;

		public int GlobalOffset => Row * 81 + Column * 9 + Digit;

		public Cell Cell { get; }

		public ISet<Candidate> Peers
		{
			get
			{
				var result = new HashSet<Candidate>();
				var currentCell = Cell;
				int currentDigit = Digit;
				result.AddRange(from i in Values.DigitRange select new Candidate(currentCell, i));
				result.AddRange(from cell in Cell.Peers select new Candidate(cell, currentDigit));
				result.Remove(this);
				return result;
			}
		}

		public ISet<Region> Regions => Cell.Regions;

		internal Region RegionRow => Cell.RegionRow;

		internal Region RegionColumn => Cell.RegionColumn;

		internal Region RegionBlock => Cell.RegionBlock;


		public void Deconstruct(out int row, out int column, out int digit) => (row, column, digit) = (Row, Column, Digit);

		public void Deconstruct(out Cell cell, out int digit) => (cell, digit) = (Cell, Digit);

		public bool Equals(Candidate other) => GlobalOffset == other.GlobalOffset;

		public override bool Equals(object? obj) => obj is Candidate comparer && Equals(comparer);

		public int CompareTo(Candidate other) => GlobalOffset.CompareTo(other.GlobalOffset);

		public override int GetHashCode() => GlobalOffset;

		public override string ToString() => $"{Digit + 1}{Cell}";

		public ISet<Candidate> IntersectWith(Candidate other)
		{
			var result = new HashSet<Candidate>();
			if (Cell.IsSameRegionWith(other.Cell))
			{
				if (Digit == other.Digit) // Same digit...
				{
					var cell = Cell;
					result.AddRange(from i in Values.DigitRange select new Candidate(cell, i));
					result.Remove(this);
					result.Remove(other);
				}
				else // Different digit...
				{
					result.Add(new(Cell, other.Digit));
					result.Add(new(other.Cell, Digit));
				}
			}
			else if (Digit == other.Digit)
			{
				int digit = Digit;
				result.AddRange(from cell in Cell & other.Cell select new Candidate(cell, digit));
			}

			return result;
		}


		public static bool TryParse(string str, [NotNullWhen(returnValue: true)] out Candidate? result)
		{
			try
			{
				result = Parse(str);
				return true;
			}
			catch
			{
				result = default;
				return false;
			}
		}

		public static string ToString(params Candidate[] candidates) => ToString(", ", candidates);

		public static string ToString(string separator, params Candidate[] candidates) =>
			ToString(separator, (IEnumerable<Candidate>)candidates);

		public static string ToString(IEnumerable<Candidate> candidates) => ToString(", ", candidates);

		public static string ToString(string separator, IEnumerable<Candidate> candidates)
		{
			var sb = new StringBuilder();
			foreach (var digitGroup in from cand in candidates group cand by cand.Digit)
			{
				foreach (var rowGroup in from cand in digitGroup group cand by cand.Row)
				{
					int digit = rowGroup.Key;
					sb.Append(Cell.ToString(from cand in rowGroup select cand.Cell));
					sb.Append($"({digit}){separator}");
				}
			}

			return sb.RemoveFromLast(separator.Length).ToString();
		}

		public static Candidate Parse(string str)
		{
			string? match = str.Match(Regexes.Candidate);
			return match is null
				? throw new FormatException()
				: new Candidate(match[2] - '1', match[4] - '1', match[0] - '1');
		}

		public static Candidate GetCandidate(Region region, int relativePosition, int digit) =>
			new(Cell.GetCell(region, relativePosition), digit);


		public static bool operator ==(Candidate left, Candidate right) => left.Equals(right);

		public static bool operator !=(Candidate left, Candidate right) => !(left == right);

		public static bool operator >(Candidate left, Candidate right) => left.CompareTo(right) > 0;

		public static bool operator <(Candidate left, Candidate right) => left.CompareTo(right) < 0;

		public static bool operator >=(Candidate left, Candidate right) => left.CompareTo(right) >= 0;

		public static bool operator <=(Candidate left, Candidate right) => left.CompareTo(right) <= 0;

		public static ISet<Candidate> operator &(Candidate left, Candidate right) => left.IntersectWith(right);
	}
}
