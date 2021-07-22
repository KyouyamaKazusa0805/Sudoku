using System.Buffers;
using System.Runtime.CompilerServices;
#if DEBUG
using System.Diagnostics;
#else
using System;
#endif

namespace System.Text
{
	partial struct ValueStringBuilder
	{
		/// <summary>
		/// To ensure the capacity in order to append characters into this collection.
		/// </summary>
		/// <param name="capacity">The capacity value to ensure.</param>
		public void EnsureCapacity(int capacity)
		{
#if DEBUG
			// This is not expected to be called this with negative capacity
			Debug.Assert(capacity >= 0);
#else
			if (capacity < 0)
			{
				throw new ArgumentOutOfRange(nameof(capacity));
			}
#endif

			// If the caller has a bug and calls this with negative capacity,
			// make sure to call Grow to throw an exception.
			if (capacity > Capacity)
			{
				Grow(capacity - Length);
			}
		}

		/// <summary>
		/// Resize the internal buffer either by doubling current buffer size or
		/// by adding <paramref name="additionalCapacityBeyondPos"/> to
		/// <see cref="Length"/> whichever is greater.
		/// </summary>
		/// <param name="additionalCapacityBeyondPos">Number of chars requested beyond current position.</param>
		/// <seealso cref="Length"/>
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow(int additionalCapacityBeyondPos)
		{
#if DEBUG
			Debug.Assert(additionalCapacityBeyondPos > 0);
			Debug.Assert(
				Length > _chars.Length - additionalCapacityBeyondPos,
				"Grow called incorrectly, no resize is needed."
			);
#endif

			// Make sure to let Rent throw an exception
			// if the caller has a bug and the desired capacity is negative.
			char[] poolArray = ArrayPool<char>.Shared.Rent(
				(int)Math.Max((uint)(Length + additionalCapacityBeyondPos),
				(uint)_chars.Length * 2)
			);

			// If lack of space to store extra characters, just creates a new one,
			// and copy them into the new collection.
			_chars[..Length].CopyTo(poolArray);

			char[]? toReturn = _chunk;
			_chars = _chunk = poolArray;
			if (toReturn is not null)
			{
				ArrayPool<char>.Shared.Return(toReturn);
			}
		}
	}
}
