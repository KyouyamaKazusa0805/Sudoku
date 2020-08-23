#pragma warning disable CS8767

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;

namespace Sudoku.Data.Meta
{
	public sealed class CandidateField : IComparable<CandidateField>, ICloneable<CandidateField>, IEquatable<CandidateField>, IEnumerable<bool>
	{
		private readonly BitArray _candidates = new(9, true);


		public CandidateField()
		{
		}

		public CandidateField(bool initializeBy) => _candidates = new BitArray(9, initializeBy);

		public CandidateField(int digit) : this(new[] { digit })
		{
		}

		public CandidateField(params int[] digits) : this((IEnumerable<int>)digits)
		{
		}

		public CandidateField(BitArray array) => _candidates = array;

		public CandidateField(IEnumerable<int> digits)
		{
			foreach (int digit in digits)
			{
				_candidates[digit] = false;
			}

			ReverseAll(); // Reverse all bits.
		}


		public int Cardinality => Trues.Count();

		public int EigenValue
		{
			get
			{
				int result = 0;
				foreach (int digit in Values.DigitRange)
				{
					// `value ? 1 : 0` is the same meaning with `value.GetHashCode()`.
					result += _candidates[digit].GetHashCode();
					if (digit != 8)
					{
						result <<= 1;
					}
				}

				return result;
			}
		}

		public IEnumerable<int> Trues =>
			from digit in Values.DigitRange
			where _candidates[digit]
			select digit;

		public IEnumerable<int> Falses =>
			from digit in Values.DigitRange
			where !_candidates[digit]
			select digit;


		[IndexerName("Index")]
		public bool this[Index index] { get => _candidates[index]; set => _candidates[index] = value; }


		public override bool Equals(object? obj) =>
			obj is CandidateField comparer && Equals(comparer);

		public bool Equals(CandidateField other) =>
			GetHashCode() == other.GetHashCode();

		public override int GetHashCode() => EigenValue;

		public int CompareTo(CandidateField other) =>
			EigenValue.CompareTo(other.EigenValue);

		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (int digit in Values.DigitRange)
			{
				if (this[digit])
				{
					sb.Append(digit + 1);
				}
			}

			return sb.ToString();
		}

		public CandidateField BitAndWith(CandidateField other)
		{
			foreach (int digit in Values.DigitRange)
			{
				this[digit] &= other[digit];
			}

			return this;
		}

		public CandidateField BitOrWith(CandidateField other)
		{
			foreach (int digit in Values.DigitRange)
			{
				this[digit] |= other[digit];
			}

			return this;
		}

		public CandidateField BitXorWith(CandidateField other)
		{
			foreach (int digit in Values.DigitRange)
			{
				this[digit] ^= other[digit];
			}

			return this;
		}

		public CandidateField BitNot()
		{
			foreach (int digit in Values.DigitRange)
			{
				this[digit] ^= true;
			}

			return this;
		}

		public CandidateField Clone() => new((BitArray)_candidates.Clone());

		public IEnumerator<bool> GetEnumerator() => (from i in Values.DigitRange select _candidates[i]).GetEnumerator();

		object ICloneable.Clone() => Clone();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		internal void ReverseAll()
		{
			foreach (int digit in Values.DigitRange)
			{
				_candidates[digit] = !_candidates[digit];
			}
		}

		public static bool TryParse(string str, [NotNullWhen(true)] out CandidateField? result)
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

		public static CandidateField Parse(string str)
		{
			string? match = str.Match(Regexes.CandidateField);
			return match is null
				? throw new FormatException()
				: new CandidateField(from ch in match.ToCharArray() select ch - '1');
		}


		public static bool operator ==(CandidateField left, CandidateField right) => left.Equals(right);

		public static bool operator !=(CandidateField left, CandidateField right) => !(left == right);

		public static bool operator >(CandidateField left, CandidateField right) => left.CompareTo(right) > 0;

		public static bool operator <(CandidateField left, CandidateField right) => left.CompareTo(right) < 0;

		public static bool operator >=(CandidateField left, CandidateField right) => left.CompareTo(right) >= 0;

		public static bool operator <=(CandidateField left, CandidateField right) => left.CompareTo(right) <= 0;

		public static CandidateField operator ~(CandidateField candidateField) => candidateField.Clone().BitNot();

		public static CandidateField operator &(CandidateField left, CandidateField right) => left.Clone().BitAndWith(right);

		public static CandidateField operator |(CandidateField left, CandidateField right) => left.Clone().BitOrWith(right);

		public static CandidateField operator ^(CandidateField left, CandidateField right) => left.Clone().BitXorWith(right);
	}
}
