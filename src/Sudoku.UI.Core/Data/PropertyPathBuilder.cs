#pragma warning disable CS0809

namespace Sudoku.UI.Data;

/// <summary>
/// Defines a property path builder that can creates a property path.
/// </summary>
public ref struct PropertyPathBuilder
{
	/// <summary>
	/// Indicates the inner builder.
	/// </summary>
	private StringHandler _sb = new(100);


	/// <summary>
	/// Initializes a <see cref="PropertyPathBuilder"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PropertyPathBuilder()
	{
	}


	/// <inheritdoc cref="object.Equals(object?)"/>
	[Obsolete(RefStructDefaultImplementationMessage.OverriddenEqualsMethod, false, DiagnosticId = "SCA0104", UrlFormat = "https://sunnieshine.github.io/Sudoku/code-analysis/sca0104")]
	public override readonly bool Equals([NotNullWhen(true)] object? obj) => false;

	/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly bool Equals(scoped PropertyPathBuilder other) => _sb == other._sb;

	/// <inheritdoc cref="object.GetHashCode"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override readonly int GetHashCode() => _sb.ToString().GetHashCode();

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => _sb.ToStringAndClear()[..^1];

	/// <summary>
	/// Appends the specified property into the builder.
	/// </summary>
	/// <typeparam name="T">
	/// The type of the argument <paramref name="propertyName"/> references to.
	/// </typeparam>
	/// <param name="propertyName">
	/// The name of the property. Generally using <see langword="nameof"/> expression is okay.
	/// </param>
	/// <returns>The current instance. This return value and be used as chaining invocations.</returns>
	/// <remarks>
	/// For example, if the method invocation is <c><![CDATA[AppendProperty<Path>(nameof(Path.Data))]]></c>,
	/// the expanded property path will be <c>(Path.Data)</c>. Please note that the argument is always
	/// a <see langword="nameof"/> expression, due to C# language design, the <see langword="nameof"/>
	/// expression will only keep the property name part <c>Data</c> as the string result instead of
	/// the full name <c>Path.Data</c>. This is why we should use an extra generic type parameter
	/// to expand the property path.
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when generic type argument <typeparamref name="T"/> is a generic type.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PropertyPathBuilder AppendProperty<T>(string propertyName)
	{
		if (typeof(T).IsGenericType)
		{
			throw new InvalidOperationException("The property path cannot reference to a generic instance.");
		}

		_sb.Append($"({typeof(T).Name}.{propertyName}).");
		return this;
	}

	/// <summary>
	/// Appends the zero index <c>[0]</c> into the builder.
	/// </summary>
	/// <returns>The current instance. This return value and be used as chaining invocations.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PropertyPathBuilder AppendZeroIndex()
	{
		_sb.RemoveFromEnd(1);
		_sb.Append("[0].");
		return this;
	}

	/// <summary>
	/// Appends the specified index into the builder.
	/// </summary>
	/// <param name="index">The index value.</param>
	/// <returns>The current instance. This return value and be used as chaining invocations.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the index is a negative value.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PropertyPathBuilder AppendIndex(int index)
	{
		if (index < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}

		_sb.RemoveFromEnd(1);
		_sb.Append($"[{index}].");
		return this;
	}


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(scoped PropertyPathBuilder left, scoped PropertyPathBuilder right) => left.Equals(right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(scoped PropertyPathBuilder left, scoped PropertyPathBuilder right) => !(left == right);
}
