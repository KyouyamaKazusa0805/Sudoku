using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Sudoku.Solving.Tools
{
	/// <summary>
	/// Generator of binary permutations. This class will compute
	/// all binary numbers of specified length and 1's count ascendingly.
	/// </summary>
	/// <example>
	/// With <see cref="OneCount"/> = 3 and <see cref="BitCount"/> = 5
	/// the following binary numbers are generated:
	/// <list type="bullet">
	/// <item>00111</item>
	/// <item>01011</item>
	/// <item>01101</item>
	/// <item>01110</item>
	/// <item>10011</item>
	/// <item>10101</item>
	/// <item>10110</item>
	/// <item>11001</item>
	/// <item>11010</item>
	/// <item>11100</item>
	/// </list>
	/// </example>
	/// <remarks>
	/// The code adapted from "Hacker's Delight" by Henry S. Warren,
	/// ISBN 0-201-91465-4,
	/// and I should thank for Sudoku Explainer's author...
	/// </remarks>
	internal sealed class BinaryPermutation : IEnumerable<long>
	{
		private bool _isLast;

		private readonly long _mask;

		private long _value;


		public BinaryPermutation(int oneCount, int bitCount)
		{
			Contract.Requires(oneCount >= 0);
			Contract.Requires(bitCount >= 0);
			Contract.Requires(oneCount <= bitCount);
			Contract.Requires(bitCount <= 64);

			(BitCount, OneCount) = (bitCount, oneCount);
			(_value, _mask, _isLast) = ((1 << oneCount) - 1, (1L << bitCount - oneCount) - 1, bitCount == 0);
		}


		public int BitCount { get; }

		public int OneCount { get; }


		public IEnumerator<long> GetEnumerator()
		{
			while (HasNext())
			{
				yield return Next();
			}
		}

		public bool HasNext()
		{
			bool result = !_isLast;
			_isLast = (_value & -_value & _mask) == 0;
			return result;
		}

		public long Next()
		{
			long result = _value;
			if (!_isLast)
			{
				long smallest = _value & -_value;
				long ripple = _value + smallest;
				long ones = _value ^ ripple;
				ones = (ones >> 2) / smallest;
				_value = ripple | ones;
			}

			return result;
		}

		public int[] NextBitNumbers()
		{
			long mask = Next();
			var result = new int[OneCount];
			int dst = 0;
			for (int src = 0; src < BitCount; src++)
			{
				if ((mask & 1L << src) != 0)
				{
					result[dst++] = src; // Bit number `src` is set.
				}
			}

			return result;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
