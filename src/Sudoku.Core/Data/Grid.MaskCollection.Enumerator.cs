namespace Sudoku.Data;

partial struct Grid
{
	partial struct MaskCollection
	{
		/// <summary>
		/// Defines the default enumerator that iterates the <see cref="Grid"/>
		/// through the masks in the current <see cref="Grid"/> instance.
		/// </summary>
		public unsafe ref partial struct Enumerator
		{
			/// <summary>
			/// The pointer to the start value.
			/// </summary>
			private readonly short* _start;


			/// <summary>
			/// The current pointer.
			/// </summary>
			private short* _currentPointer;

			/// <summary>
			/// The current index.
			/// </summary>
			private int _currentIndex;


			/// <summary>
			/// Initializes an instance with the specified pointer to an array to iterate.
			/// </summary>
			/// <param name="arr">The pointer to an array.</param>
			/// <remarks>
			/// Note here we should point at the one-unit-lengthed memory before the array start.
			/// </remarks>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(short* arr)
			{
				_currentPointer = _start = arr - 1;
				_currentIndex = -1;
			}


			/// <summary>
			/// Gets the element in the collection at the current position of the enumerator.
			/// </summary>
			public readonly ref short Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ref *_currentPointer;
			}


			/// <summary>
			/// Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>
			/// <list type="table">
			/// <listheader>
			/// <term>Return value</term>
			/// <description>Meaning</description>
			/// </listheader>
			/// <item>
			/// <term><see langword="true"/></term>
			/// <description>If the enumerator was successfully advanced to the next element.</description>
			/// </item>
			/// <item>
			/// <term><see langword="false"/></term>
			/// <description>If the enumerator has passed the end of the collection.</description>
			/// </item>
			/// </list>
			/// </returns>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (++_currentIndex >= 81)
				{
					return false;
				}
				else
				{
					_currentPointer++;
					return true;
				}
			}

			/// <summary>
			/// Sets the enumerator to its initial position, which is before the first element
			/// in the collection.
			/// </summary>
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset()
			{
				_currentPointer = _start;
				_currentIndex = -1;
			}
		}
	}
}
