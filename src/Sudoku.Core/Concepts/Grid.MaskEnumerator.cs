namespace Sudoku.Concepts;

partial struct Grid
{
	/// <summary>
	/// Defines the default enumerator that iterates the <see cref="Grid"/> through the masks in the current <see cref="Grid"/> instance.
	/// </summary>
	/// <param name="arr">The pointer to an array.</param>
	/// <seealso cref="Grid"/>
	public ref struct MaskEnumerator([UnscopedRef] ref short arr)
	{
		/// <summary>
		/// The current pointer.
		/// </summary>
		private ref short _refCurrent = ref SubtractByteOffset(ref arr, 1);

		/// <summary>
		/// The current index.
		/// </summary>
		private int _currentIndex = -1;


		/// <summary>
		/// Gets the element in the collection at the current position of the enumerator.
		/// </summary>
		public readonly ref short Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _refCurrent;
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
				RefMoveNext(ref _refCurrent);
				return true;
			}
		}

		/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly MaskEnumerator GetEnumerator() => this;
	}
}
