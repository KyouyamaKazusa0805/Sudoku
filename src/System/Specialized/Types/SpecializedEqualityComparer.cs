namespace System;

/// <summary>
/// Represents a specialized equality comparer.
/// </summary>
/// <typeparam name="T">The type of values to be compared.</typeparam>
/// <param name="isUnsafe">Indicates whether the handler methods are unsafe.</param>
public sealed class SpecializedEqualityComparer<T>(bool isUnsafe) : IEqualityComparer<T>
{
	/// <summary>
	/// The handler for method <see cref="Equals(ref readonly T, ref readonly T)"/>.
	/// </summary>
	private readonly EqualsHandler<T>? _equals;

	/// <summary>
	/// The handler for method <see cref="GetHashCode(ref readonly T)"/>.
	/// </summary>
	private readonly GetHashCodeHandler<T>? _getHashCode;

	/// <inheritdoc cref="_equals"/>
	private readonly unsafe delegate*<ref readonly T, ref readonly T, bool> _equalsUnsafe;

	/// <inheritdoc cref="_getHashCode"/>
	private readonly unsafe delegate*<ref readonly T, int> _getHashCodeUnsafe;


	/// <summary>
	/// Initializes a <see cref="SpecializedEqualityComparer{T}"/> instance.
	/// </summary>
	public SpecializedEqualityComparer(EqualsHandler<T> equals, GetHashCodeHandler<T> getHashCode) : this(false)
	{
		_equals = equals;
		_getHashCode = getHashCode;
	}

	/// <summary>
	/// Initializes a <see cref="SpecializedEqualityComparer{T}"/> instance.
	/// </summary>
	public unsafe SpecializedEqualityComparer(delegate*<ref readonly T, ref readonly T, bool> equals, delegate*<ref readonly T, int> getHashCode) : this(true)
	{
		_equalsUnsafe = equals;
		_getHashCodeUnsafe = getHashCode;
	}


	/// <summary>
	/// Indicates whether the comparer uses unsafe logic.
	/// </summary>
	[MemberNotNullWhen(true, nameof(_equalsUnsafe), nameof(_getHashCodeUnsafe))]
	[MemberNotNullWhen(false, nameof(_equals), nameof(_getHashCode))]
	public bool IsUnsafe { get; } = isUnsafe;


	/// <inheritdoc cref="IEqualityComparer{T}.Equals(T, T)"/>
	public unsafe bool Equals(scoped ref readonly T left, scoped ref readonly T right)
		=> IsUnsafe ? _equalsUnsafe(in left, in right) : _equals(in left, in right);

	/// <inheritdoc cref="IEqualityComparer{T}.GetHashCode(T)"/>
	public unsafe int GetHashCode([DisallowNull] scoped ref readonly T obj)
		=> IsUnsafe ? _getHashCodeUnsafe(in obj) : _getHashCode(in obj);

	/// <inheritdoc/>
	bool IEqualityComparer<T>.Equals(T? x, T? y)
		=> (x, y) switch { (null, null) => true, (not null, not null) => Equals(in x, in y), _ => false };

	/// <inheritdoc/>
	int IEqualityComparer<T>.GetHashCode([DisallowNull] T obj) => GetHashCode(in obj);
}
