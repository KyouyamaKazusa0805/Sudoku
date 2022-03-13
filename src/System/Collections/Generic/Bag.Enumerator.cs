namespace System.Collections.Generic;

partial struct Bag<T>
{
	/// <summary>
	/// Indicates the inner enumerator.
	/// </summary>
	public ref partial struct Enumerator
	{
		/// <summary>
		/// The inner array.
		/// </summary>
		private readonly T[] _instance;

		/// <summary>
		/// The number of elements to be iterated.
		/// </summary>
		private readonly int _count;

		/// <summary>
		/// The current index being iterated.
		/// </summary>
		private int _index = -1;


		/// <summary>
		/// Initializes an <see cref="Enumerator"/> instance via the array to be iterated,
		/// and the number of elements to be iterated.
		/// </summary>
		/// <param name="instance">The array instance.</param>
		/// <param name="count">The number of elements to be iterated.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(T[] instance, int count)
		{
			_instance = instance;
			_count = count;
		}


		/// <inheritdoc cref="IEnumerator.Current"/>
		public readonly T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _instance[_index];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext() => ++_index < _count;
	}
}
