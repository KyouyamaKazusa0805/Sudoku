using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;

namespace Sudoku.Data.Meta
{
	public readonly struct Cell : IEquatable<Cell>, IComparable<Cell>
	{
		public Cell(int row, int column)
		{
			Contract.Requires(row is >= 0 and < 9);
			Contract.Requires(column is >= 0 and < 9);

			(Row, Column) = (row, column);
		}


		public int Row { get; }

		public int Column { get; }

		public int Block => Row / 3 * 3 + Column / 3;

		public int GlobalOffset => Row * 9 + Column;

		public ISet<Cell> Peers
		{
			get
			{
				var result = new HashSet<Cell>();
				int r = Row, c = Column, b = Block, kr = r / 3 * 3, kc = c / 3 * 3;
				result.AddRange(from i in Values.DigitRange select new Cell(i, c));
				result.AddRange(from i in Values.DigitRange select new Cell(r, i));
				result.AddRange(from i in Enumerable.Range(kr, 3) from j in Enumerable.Range(kc, 3) select new Cell(i, j));
				result.Remove(this);
				return result;
			}
		}

		public ISet<Region> Regions => new HashSet<Region> { RegionRow, RegionColumn, RegionBlock };

		internal Region RegionRow => new(RegionType.Row, Row);

		internal Region RegionColumn => new(RegionType.Column, Column);

		internal Region RegionBlock => new(RegionType.Block, Block);


		public void Deconstruct(out int row, out int column) => (row, column) = (Row, Column);

		public override bool Equals(object? obj) => obj is Cell comparer && Equals(comparer);

		public bool Equals(Cell other) => GlobalOffset == other.GlobalOffset;

		public bool IsIntersectionWith(Cell other) => IntersectWith(other).Count != 0;

		public bool IsSameRegionWith(Cell other)
		{
			var result = new HashSet<Region>(Regions);
			result.IntersectWith(other.Regions);
			return result.Count != 0;
		}

		public override int GetHashCode() => GlobalOffset;

		public int CompareTo(Cell other) => GlobalOffset.CompareTo(other.GlobalOffset);

		public override string ToString() => $"r{Row + 1}c{Column + 1}";

		public ISet<Cell> IntersectWith(Cell other)
		{
			var result = new HashSet<Cell>(Peers);
			result.IntersectWith(other.Peers);
			return result;
		}


		public static bool TryParse(string str, [NotNullWhen(true)] out Cell? result)
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

		public static string ToString(params Cell[] cells) => ToString(", ", cells);

		public static string ToString(string separator, params Cell[] cells) =>
			ToString(separator, (IEnumerable<Cell>)cells);

		public static string ToString(IEnumerable<Cell> cells) => ToString(", ", cells);

		public static string ToString(string separator, IEnumerable<Cell> cells)
		{
			var sb = new StringBuilder();
			foreach (var groupByRow in from cell in cells orderby cell group cell by cell.Row)
			{
				bool isFirstCell = true;

				sb.Append("r");
				foreach (var cell in groupByRow)
				{
					if (isFirstCell)
					{
						sb.Append($"{cell.Row + 1}c");
						isFirstCell = !isFirstCell;
					}
					sb.Append(cell.Column + 1);
				}

				sb.Append(separator);
			}

			return sb.RemoveFromLast(separator.Length).ToString();
		}

		public static Cell Parse(string str)
		{
			string? match = str.Match(Regexes.Cell);
			return match is null
				? throw new FormatException()
				: new Cell(match[1] - '1', match[3] - '1');
		}

		public static Cell GetCell(Region region, int relativePosition)
		{
			Contract.Requires(relativePosition is >= 0 and < 9);

			return region.RegionType switch
			{
				RegionType.Row => new(region.Index, relativePosition),
				RegionType.Column => new(relativePosition, region.Index),
				_ => new(region.Index / 3 * 3 + relativePosition / 3, region.Index % 3 * 3 + relativePosition % 3)
			};
		}


		public static bool operator ==(Cell left, Cell right) => left.Equals(right);

		public static bool operator !=(Cell left, Cell right) => !(left == right);

		public static bool operator >(Cell left, Cell right) => left.CompareTo(right) > 0;

		public static bool operator <(Cell left, Cell right) => left.CompareTo(right) < 0;

		public static bool operator >=(Cell left, Cell right) => left.CompareTo(right) >= 0;

		public static bool operator <=(Cell left, Cell right) => left.CompareTo(right) <= 0;

		public static ISet<Cell> operator &(Cell left, Cell right) => left.IntersectWith(right);
	}
}
