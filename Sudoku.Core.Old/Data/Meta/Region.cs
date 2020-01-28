using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;
using Sudoku.Diagnostics.CodeAnalysis;
using static Sudoku.Data.Meta.RegionType;

namespace Sudoku.Data.Meta
{
	public readonly struct Region : IEquatable<Region>, IComparable<Region>
	{
		public Region(RegionType regionType, int index)
		{
			Contract.Requires(index >= 0 && index < 9);

			(RegionType, Index) = (regionType, index);
		}


		public int Index { get; }

		public Region[] CrossRegions
		{
			get
			{
				Contract.Assume(RegionType >= 0 && RegionType <= (RegionType)2);
				Contract.Assume(Index >= 0 && Index < 9);

				return (RegionType, Index) switch
				{
					(Row, 0) => new[] { Parse("b1"), Parse("b2"), Parse("b3") },
					(Row, 1) => new[] { Parse("b1"), Parse("b2"), Parse("b3") },
					(Row, 2) => new[] { Parse("b1"), Parse("b2"), Parse("b3") },
					(Row, 3) => new[] { Parse("b4"), Parse("b5"), Parse("b6") },
					(Row, 4) => new[] { Parse("b4"), Parse("b5"), Parse("b6") },
					(Row, 5) => new[] { Parse("b4"), Parse("b5"), Parse("b6") },
					(Row, 6) => new[] { Parse("b7"), Parse("b8"), Parse("b9") },
					(Row, 7) => new[] { Parse("b7"), Parse("b8"), Parse("b9") },
					(Row, 8) => new[] { Parse("b7"), Parse("b8"), Parse("b9") },
					(Column, 0) => new[] { Parse("b1"), Parse("b4"), Parse("b7") },
					(Column, 1) => new[] { Parse("b1"), Parse("b4"), Parse("b7") },
					(Column, 2) => new[] { Parse("b1"), Parse("b4"), Parse("b7") },
					(Column, 3) => new[] { Parse("b2"), Parse("b5"), Parse("b8") },
					(Column, 4) => new[] { Parse("b2"), Parse("b5"), Parse("b8") },
					(Column, 5) => new[] { Parse("b2"), Parse("b5"), Parse("b8") },
					(Column, 6) => new[] { Parse("b3"), Parse("b6"), Parse("b9") },
					(Column, 7) => new[] { Parse("b3"), Parse("b6"), Parse("b9") },
					(Column, 8) => new[] { Parse("b3"), Parse("b6"), Parse("b9") },
					(Block, 0) => new[] { Parse("r1"), Parse("r2"), Parse("r3"), Parse("c1"), Parse("c2"), Parse("c3") },
					(Block, 1) => new[] { Parse("r1"), Parse("r2"), Parse("r3"), Parse("c4"), Parse("c5"), Parse("c6") },
					(Block, 2) => new[] { Parse("r1"), Parse("r2"), Parse("r3"), Parse("c7"), Parse("c8"), Parse("c9") },
					(Block, 3) => new[] { Parse("r4"), Parse("r5"), Parse("r6"), Parse("c1"), Parse("c2"), Parse("c3") },
					(Block, 4) => new[] { Parse("r4"), Parse("r5"), Parse("r6"), Parse("c4"), Parse("c5"), Parse("c6") },
					(Block, 5) => new[] { Parse("r4"), Parse("r5"), Parse("r6"), Parse("c7"), Parse("c8"), Parse("c9") },
					(Block, 6) => new[] { Parse("r7"), Parse("r8"), Parse("r9"), Parse("c1"), Parse("c2"), Parse("c3") },
					(Block, 7) => new[] { Parse("r7"), Parse("r8"), Parse("r9"), Parse("c4"), Parse("c5"), Parse("c6") },
					(Block, 8) => new[] { Parse("r7"), Parse("r8"), Parse("r9"), Parse("c7"), Parse("c8"), Parse("c9") },
					_ => throw new Exception($"Impossible case ({nameof(RegionType)} is out of range).")
				};
			}
		}

		public RegionType RegionType { get; }

		public IEnumerable<Cell> Cells
		{
			get
			{
				var region = this;
				return from i in Values.DigitRange
					   from j in Values.DigitRange
					   let cell = new Cell(i, j)
					   where cell.Regions.Contains(region)
					   select cell;
			}
		}

		private int HashCode => (int)RegionType * 9 + Index;


		[OnDeconstruction]
		public void Deconstruct(out RegionType regionType, out int index) =>
			(regionType, index) = (RegionType, Index);

		public override bool Equals(object? obj) =>
			obj is Region comparer && Equals(comparer);

		public bool Equals(Region other) => HashCode == other.HashCode;

		public override int GetHashCode() => HashCode;

		public int CompareTo(Region other) => HashCode.CompareTo(other.HashCode);

		public override string ToString()
		{
			return $@"{RegionType switch
			{
				Row => "r",
				Column => "c",
				_ => "b"
			}}{Index + 1 }";
		}


		public static bool TryParse(
			string str, [NotNullWhen(returnValue: true)] out Region? result)
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

		public static string ToString(params Region[] regions) => ToString(", ", regions);

		public static string ToString(string separator, params Region[] regions) =>
			ToString(separator, (IEnumerable<Region>)regions);

		public static string ToString(IEnumerable<Region> regions) => ToString(", ", regions);

		public static string ToString(string separator, IEnumerable<Region> regions)
		{
			var sb = new StringBuilder();
			foreach (var region in regions)
			{
				sb.Append($"{region}{separator}");
			}

			return sb.RemoveFromLast(separator.Length).ToString();
		}

		public static Region Parse(string str)
		{
			var match = str.Match(@"[RCBrcb][1-9]");
			return match is null
				? throw new FormatException()
				: new Region(match[0] switch
				{
					'R' => Row,
					'r' => Row,
					'C' => Column,
					'c' => Column,
					_ => Block
				}, match[1] - '1');
		}


		public static bool operator ==(Region left, Region right) => left.Equals(right);

		public static bool operator !=(Region left, Region right) => !left.Equals(right);

		public static bool operator >(Region left, Region right) => left.CompareTo(right) > 0;

		public static bool operator <(Region left, Region right) => left.CompareTo(right) < 0;

		public static bool operator >=(Region left, Region right) => left.CompareTo(right) >= 0;

		public static bool operator <=(Region left, Region right) => left.CompareTo(right) <= 0;
	}
}
