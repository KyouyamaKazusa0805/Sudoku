using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

partial struct ValueList<T>
{
	/// <summary>
	/// Defines the enumerator of this type.
	/// </summary>
	public ref struct Enumerator
	{
		/// <summary>
		/// Indicates the inner pointer.
		/// </summary>
		/// <remarks>
		/// <para><i>
		/// Due to the C# implementation, feature "<see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/declarations#scoped-ref"><b><see langword="ref"/> fields</b></see>"
		/// does not support for native memory,
		/// which means you cannot define a reference-based field that reference to native memory.
		/// Therefore, here we can only use bare pointers to describe the internal data. <b>Do not change this field.</b>
		/// </i></para>
		/// <para><i>
		/// C# 11 does not support "<see href="https://github.com/dotnet/roslyn/issues/62243"><b><see langword="ref"/> to <see langword="ref struct"/></b></see>"
		/// neither.
		/// </i></para>
		/// </remarks>
		private readonly unsafe ValueList<T>* _ptr;

		/// <summary>
		/// Indicates the current position.
		/// </summary>
		private byte _current = unchecked((byte)-1);


		/// <summary>
		/// Initializes the <see cref="Enumerator"/> type via the current instance.
		/// </summary>
		/// <param name="ptr">The pointer that points to the list.</param>
		public unsafe Enumerator(scoped ref readonly ValueList<T> ptr)
		{
			fixed (ValueList<T>* p = &ptr)
			{
				_ptr = p;
			}
		}


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
