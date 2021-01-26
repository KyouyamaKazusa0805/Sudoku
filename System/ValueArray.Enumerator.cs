using System.Collections;
using System.Collections.Generic;

namespace System
{
	public readonly unsafe ref partial struct ValueArray<TUnmanaged> where TUnmanaged : unmanaged
	{
		/// <summary>
		/// Indicates the inner enumerator.
		/// </summary>
		public ref struct Enumerator
		{
			/// <summary>
			/// Indicates the length of that array.
			/// </summary>
			private readonly int _length;


			/// <summary>
			/// Indicates the curren index.
			/// </summary>
			private int _index;

			/// <summary>
			/// Indicates the current pointer.
			/// </summary>
			private TUnmanaged* _ptr;


			/// <summary>
			/// Initializes an instance with the specified pointer and the length of the array
			/// where that pointer points to.
			/// </summary>
			/// <param name="ptr">The pointer.</param>
			/// <param name="length">The length of that array.</param>
			[CLSCompliant(false)]
			public Enumerator(TUnmanaged* ptr, int length) : this()
			{
				_ptr = ptr - 1;
				_index = -1;
				_length = length;
			}


			/// <inheritdoc cref="IEnumerator{T}.Current"/>
			public readonly TUnmanaged Current => *_ptr;


			/// <inheritdoc cref="IEnumerator.MoveNext"/>
			public bool MoveNext()
			{
				if (++_index >= _length)
				{
					return false;
				}

				_ptr++;
				return true;
			}
		}
	}
}
