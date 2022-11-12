// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/57bfe474518ab5b7cfe6bf7424a79ce3af9d6657/src/libraries/System.Runtime/tests/System/Runtime/CompilerServices/DefaultInterpolatedStringHandlerTests.cs

namespace System.Text;

partial struct StringHandler
{
	/// <summary>
	/// Encapsulates the enumerator of this collection.
	/// </summary>
	partial struct Enumerator
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


		/// <inheritdoc cref="IEnumerator.Current"/>
		public char Current { get; private set; }


		/// <inheritdoc cref="IEnumerator.MoveNext"/>
		public bool MoveNext()
		{
			if (++_index >= _length)
			{
				return false;
			}

			RefMoveNext(ref _ptr);
			return true;
		}
	}
}
