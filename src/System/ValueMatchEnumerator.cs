namespace System;

/// <summary>
/// Represents an enumerator that is used for iteration on matches.
/// </summary>
/// <param name="originalString">Indicates the original string value.</param>
/// <param name="backingEnumerator">Indicates the backing enumerator.</param>
[StructLayout(LayoutKind.Auto)]
public ref partial struct ValueMatchEnumerator(
	string originalString,
	[PrimaryConstructorParameter(MemberKinds.Field, IsImplicitlyReadOnly = false)] Regex.ValueMatchEnumerator backingEnumerator
)/* : IEnumerator<ReadOnlySpan<char>>*/
{
	/// <inheritdoc cref="IEnumerator.Current"/>
	public readonly ReadOnlySpan<char> Current
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => originalString.AsSpan().Slice(_backingEnumerator.Current.Index, _backingEnumerator.Current.Length);
	}

	///// <inheritdoc/>
	//readonly object IEnumerator.Current => Current.ToString();


	/// <inheritdoc cref="IAnyAllMethod{TSelf, TSource}.Any()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Any()
	{
		var copied = this;
		return copied.MoveNext();
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly ValueMatchEnumerator GetEnumerator() => this;

	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool MoveNext() => _backingEnumerator.MoveNext();

	///// <inheritdoc/>
	//readonly void IDisposable.Dispose() { }

	///// <inheritdoc/>
	//[DoesNotReturn]
	//readonly void IEnumerator.Reset() => throw new NotImplementedException();
}
