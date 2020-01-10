using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.Data.Meta
{
	public struct CellInfo : IEquatable<CellInfo>
	{
		private int _cellValue;

		private CellType _cellType;


		public CellInfo(Cell cell)
			: this(cell, default, CellType.Empty, new CandidateField())
		{
		}

		public CellInfo(Cell cell, int value)
			: this(cell, value, CellType.Given, new CandidateField(value))
		{
		}

		public CellInfo(Cell cell, int value, CellType cellType)
			: this(cell, value, cellType, cellType == CellType.Empty ? new CandidateField() : new CandidateField(value))
		{
		}

		public CellInfo(Cell cell, int value, CellType type, CandidateField candidates)
		{
			Contract.Requires(value >= 0 && value < 9);

			(Cell, Candidates, _cellValue, _cellType) = (cell, candidates, value, type);
			(ValueChanging, ValueChanged, CellTypeChanging, CellTypeChanged) = (null, null, null, null);
		}


		public bool IsValueCell => CellType != CellType.Empty;

		public bool IsNakedSingle => !IsValueCell && CandidateCount == 1;

		public readonly int CandidateCount => Candidates.Cardinality;

		public int TrendValue => IsNakedSingle ? Candidates.Trues.First() : -1;

		public int Value
		{
			get => _cellValue;
			set
			{
				ValueChanging?.Invoke(
					this, new ValueChangingEventArgs(new Candidate(Cell, value)));

				_cellValue = value;

				ValueChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public readonly int Row => Cell.Row;

		public readonly int Column => Cell.Column;

		public readonly int Block => Cell.Block;

		public readonly Cell Cell { get; }

		public CellType CellType
		{
			get => _cellType;
			set
			{
				CellTypeChanging?.Invoke(this, new CellTypeChangingEventArgs(value));

				_cellType = value;

				CellTypeChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public readonly CandidateField Candidates { get; }


		public bool this[int digit] { get => Get(digit); set => Set(digit, value); }


		public event ValueChangingEventHandler? ValueChanging;
		public event ValueChangedEventHandler? ValueChanged;
		public event CellTypeChangingEventHandler? CellTypeChanging;
		public event CellTypeChangedEventHandler? CellTypeChanged;


		public readonly void Set(int digit, bool value)
		{
			Contract.Requires(digit >= 0 && digit < 9);

			Candidates[digit] = value;
		}

		[OnDeconstruction]
		public void Deconstruct(
			out Cell cell, out int value, out CellType cellType, out CandidateField candidates)
		{
			(cell, value, cellType, candidates) = (Cell, Value, CellType, Candidates.Clone());
		}

		public readonly bool Get(int digit)
		{
			Contract.Requires(digit >= 0 && digit < 9);

			return Candidates[digit];
		}

		public override bool Equals(object? obj) =>
			obj is CellInfo comparer && Equals(comparer);

		public bool Equals(CellInfo other) => GetHashCode() == other.GetHashCode();

		public readonly bool Contains(int candidateDigit) =>
			Candidates[candidateDigit];

		public readonly bool ContainsAny(IEnumerable<int> digits)
		{
			var @this = this;
			return digits.Any(digit => @this.Contains(digit));
		}

		public override int GetHashCode()
		{
			int result = GetType().GetHashCode();
			result ^= Value;
			result ^= Cell.GetHashCode();
			result ^= (int)CellType;
			result ^= Candidates.GetHashCode();
			return result;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			if (IsValueCell)
			{
				sb.Append(Value + 1);
			}
			sb.Append(Cell);
			if (!IsValueCell && CandidateCount != 0)
			{
				sb.Append($"{{{Candidates}}}");
			}
			return sb.ToString();
		}


		public static bool operator ==(CellInfo left, CellInfo right) => left.Equals(right);

		public static bool operator !=(CellInfo left, CellInfo right) => !left.Equals(right);
	}
}
