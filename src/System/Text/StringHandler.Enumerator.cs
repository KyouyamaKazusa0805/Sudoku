namespace System.Text;

partial struct StringHandler
{
	/// <summary>
	/// Encapsulates the enumerator of this collection.
	/// </summary>
	public unsafe ref struct Enumerator
	{
		/// <summary>
		/// Indicates the length.
		/// </summary>
		private readonly int _length;

		/// <summary>
		/// Indicates whether 
		/// </summary>
		private int _index;

		/// <summary>
		/// Indicates the pointer that points to the current character.
		/// </summary>
		private ref char _ptr;


		/// <summary>
		/// Initializes an instance with the specified character list specified as a <see cref="Span{T}"/>.
		/// </summary>
		/// <param name="chars">The characters.</param>
		/// <exception cref="NullReferenceException">
		/// Throws when the field <see cref="_chars"/> in argument <paramref name="chars"/>
		/// is a <see langword="null"/> reference after having been invoked <see cref="Span{T}.GetPinnableReference()"/>.
		/// </exception>
		/// <seealso cref="Span{T}"/>
		/// <seealso cref="Span{T}.GetPinnableReference()"/>
		internal Enumerator(scoped in StringHandler chars) : this()
		{
			_length = chars.Length;
			_index = -1;

			ref var z = ref chars._chars.GetPinnableReference();
			if (Unsafe.IsNullRef(ref z))
			{
				throw new NullReferenceException("The character series is a null reference.");
			}

			_ptr = ref Unsafe.SubtractByteOffset(ref z, 1);
		}


		/// <inheritdoc cref="IEnumerator.Current"/>
		public char Current { get; private set; }


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			if (++_index >= _length)
			{
				return false;
			}

			_ptr = Unsafe.AddByteOffset(ref _ptr, 1);
			return true;
		}
	}
}
