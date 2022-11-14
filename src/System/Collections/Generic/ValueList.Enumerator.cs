namespace System.Collections.Generic;

partial struct ValueList<T>
{
	partial struct Enumerator
	{
		/// <summary>
		/// Indicates the inner pointer.
		/// </summary>
		private readonly unsafe ValueList<T>* _ptr;

		/// <summary>
		/// Indicates the current position.
		/// </summary>
		private byte _current = unchecked((byte)-1);


		/// <inheritdoc cref="IEnumerator{T}.Current"/>
		public unsafe readonly T Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _ptr->_startPtr[_current];
		}


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public unsafe bool MoveNext() => ++_current != _ptr->_length;
	}
}
