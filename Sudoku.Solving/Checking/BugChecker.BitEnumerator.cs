using System.Collections;
using System.Collections.Generic;

namespace Sudoku.Solving.Checking
{
	partial class BugChecker
	{
		private struct BitEnumerator : IEnumerable<short>
		{
			private readonly short _mask;

			private short _value;

			private bool _isLast;


			public BitEnumerator(int bitCount, int oneCount)
			{
				(BitCount, OneCount) = (bitCount, oneCount);
				(_value, _mask, _isLast) = (
					(short)((1 << oneCount) - 1),
					(short)((1 << (bitCount - oneCount)) - 1),
					bitCount == 0);
			}


			public readonly int BitCount { get; }

			public readonly int OneCount { get; }


			public IEnumerator<short> GetEnumerator()
			{
				while (HasNext())
				{
					short result = _value;
					if (!_isLast)
					{
						short smallest = (short)(_value & -_value);
						short ripple = (short)(_value + smallest);
						short ones = (short)(_value ^ ripple);
						ones = (short)((ones >> 2) / smallest);
						_value = (short)(ripple | ones);
					}

					yield return result;
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public bool HasNext()
			{
				bool result = !_isLast;
				_isLast = (_value & -_value & _mask) == 0;
				return result;
			}
		}
	}
}
