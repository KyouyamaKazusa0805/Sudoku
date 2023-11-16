// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Runtime/tests/System/Runtime/CompilerServices/DefaultInterpolatedStringHandlerTests.cs

using System.Collections;
using System.Runtime.CompilerServices;

namespace System.Text;

partial struct StringHandler
{
	/// <summary>
	/// Encapsulates the enumerator of this collection.
	/// </summary>
	public ref struct Enumerator
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
		/// <exception cref="ArgumentNullRefException">
		/// Throws when the field <see cref="_chars"/> in argument <paramref name="chars"/>
		/// is a <see langword="null"/> reference after having been invoked <see cref="Span{T}.GetPinnableReference()"/>.
		/// </exception>
		/// <seealso cref="Span{T}"/>
		/// <seealso cref="Span{T}.GetPinnableReference()"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator(StringHandler chars) : this()
		{
			_length = chars.Length;
			_index = -1;

			ref var z = ref chars._chars.GetPinnableReference();
			Ref.ThrowIfNullRef(in z);

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

			Unsafe2.RefMoveNext(ref _ptr);
			return true;
		}
	}
}
