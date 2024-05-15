namespace System;

/// <summary>
/// Represents a specialized comparer.
/// </summary>
/// <typeparam name="T">The type of values to be compared.</typeparam>
/// <param name="isUnsafe">Indicates whether the handler methods are unsafe.</param>
public sealed class SpecializedComparer<T>(bool isUnsafe) : IComparer<T>
{
	/// <summary>
	/// The handler for method <see cref="Compare(ref readonly T, ref readonly T)"/>.
	/// </summary>
	private readonly FuncRefReadOnly<T, T, int>? _compare;

	/// <inheritdoc cref="_compare"/>
	private readonly unsafe delegate*<ref readonly T, ref readonly T, int> _compareUnsafe;


	/// <summary>
	/// Indicates whether the comparer uses unsafe logic.
	/// </summary>
	[MemberNotNullWhen(true, nameof(_compareUnsafe))]
	[MemberNotNullWhen(false, nameof(_compare))]
	public bool IsUnsafe { get; } = isUnsafe;


	/// <summary>
	/// Initializes a <see cref="SpecializedComparer{T}"/> instance.
	/// </summary>
	public SpecializedComparer(FuncRefReadOnly<T, T, int> compareHandler) : this(false) => _compare = compareHandler;

	/// <summary>
	/// Initializes a <see cref="SpecializedComparer{T}"/> instance.
	/// </summary>
	public unsafe SpecializedComparer(delegate*<ref readonly T, ref readonly T, int> compareHandler) : this(true)
		=> _compareUnsafe = compareHandler;


	/// <inheritdoc cref="IComparable{T}.CompareTo(T)"/>
	public unsafe int Compare(ref readonly T left, ref readonly T right)
		=> IsUnsafe ? _compareUnsafe(in left, in right) : _compare(in left, in right);

	/// <inheritdoc/>
	int IComparer<T>.Compare(T? x, T? y)
		=> (x, y) switch { (null, null) => 0, (not null, not null) => Compare(in x, in y), (null, not null) => -1, _ => 1 };
}
