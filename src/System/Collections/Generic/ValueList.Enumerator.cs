namespace System.Collections.Generic;

partial struct ValueList<TNotNull>
{
	/// <summary>
	/// Defines the enumerator of this type.
	/// </summary>
	public unsafe ref struct Enumerator
	{
		/// <summary>
		/// Indicates the inner pointer.
		/// </summary>
		private readonly ValueList<TNotNull>* _ptr;

		/// <summary>
		/// Indicates the current position.
		/// </summary>
		private byte _current = unchecked((byte)-1);


		/// <summary>
		/// Initializes the <see cref="Enumerator"/> type via the current instance.
		/// </summary>
		/// <param name="ptr">The pointer that points to the list.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Enumerator(in ValueList<TNotNull> ptr)
		{
			fixed (ValueList<TNotNull>* p = &ptr)
			{
				_ptr = p;
			}
		}


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public readonly TNotNull Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _ptr->_startPtr[_current];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext() => ++_current != _ptr->_length;
	}
}
