namespace System;

/// <summary>
/// Represents an enumerator that iterates on each element of type <typeparamref name="T"/> inside a <see cref="ReadOnlyMemory{T}"/>.
/// </summary>
/// <typeparam name="T">The type of each element.</typeparam>
/// <seealso cref="ReadOnlyMemory{T}"/>
[StructLayout(LayoutKind.Auto)]
[DebuggerStepThrough]
[TypeImpl(TypeImplFlag.AllObjectMethods)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct ReadOnlyMemoryEnumerator<T>(ReadOnlyMemory<T> value)
{
	/// <summary>
	/// Indicates the span to the memory.
	/// </summary>
	private readonly ReadOnlySpan<T> _span = value.Span;

	/// <summary>
	/// Indicates the index that is currently iterated.
	/// </summary>
	private int _index = -1;


	/// <summary>
	/// Indicates the current instance.
	/// </summary>
	public readonly ref readonly T Current => ref _span[_index];


	/// <inheritdoc cref="IEnumerator.MoveNext"/>
	public bool MoveNext() => ++_index < value.Length;
}
