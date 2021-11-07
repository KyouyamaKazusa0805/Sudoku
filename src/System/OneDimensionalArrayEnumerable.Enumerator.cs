namespace System;

partial struct OneDimensionalArrayEnumerable<T>
{
	/// <summary>
	/// Defines an enumerator that iterates the one-dimensional array.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// Indicates the length of the array to iterate.
		/// The value is equal to <c><see cref="_innerArray"/>.Length</c>.
		/// </summary>
		private readonly int _length;

		/// <summary>
		/// Indicates the array to iterate.
		/// </summary>
		private readonly T[] _innerArray;

		/// <summary>
		/// Indicates the current index being iterated.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes a <see cref="Enumerator"/> instance
		/// via the specified array to iterate.
		/// </summary>
		/// <param name="innerArray">The array to iterate.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator(T[] innerArray)
		{
			_innerArray = innerArray;
			_length = innerArray.Length;
		}


		/// <summary>
		/// Indicates the current instance being iterated. Please note that the value is returned by reference.
		/// </summary>
		public readonly ref T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _innerArray[_index];
		}


		/// <summary>
		/// Retrieve the iterator to make it points to the next element.
		/// </summary>
		/// <returns>
		/// A <see cref="bool"/> value indicating whether the moving operation is successful.
		/// Returns <see langword="false"/> when the last iteration is for the last element,
		/// and now there's no elements to be iterated.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _length;
	}
}