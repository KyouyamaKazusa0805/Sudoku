namespace System;

/// <inheritdoc/>
/// <param name="paramName">The parameter name.</param>
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public sealed class ArgumentNullRefException(string? paramName) : ArgumentNullException(paramName)
{
	/// <summary>
	/// Throws an <see cref="ArgumentNullRefException"/> if argument is <see langword="null"/> reference.
	/// </summary>
	/// <typeparam name="T">The type of the argument reference.</typeparam>
	/// <param name="argument">
	/// <para>The reference type argument to validate as non-null.</para>
	/// <para><i>
	/// Please note that the argument requires a <see langword="ref"/> modifier, but it does not modify the referenced value
	/// of the argument. It is nearly equal to <see langword="in"/> modifier.
	/// However, the method will invoke <see cref="IsNullRef{T}(ref T)"/>, where the only argument is passed by <see langword="ref"/>.
	/// Therefore, here the current method argument requires a modifier <see langword="ref"/> instead of <see langword="in"/>.
	/// </i></para>
	/// </param>
	/// <param name="paramName">
	/// The name of the parameter with which argument corresponds. If you omit this parameter, the name of argument is used.
	/// </param>
	/// <exception cref="ArgumentNullRefException">Throws when <paramref name="argument"/> is <see langword="null"/>.</exception>
	/// <seealso cref="IsNullRef{T}(ref T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void ThrowIfNullRef<T>(scoped ref T argument, [ConstantExpected, CallerArgumentExpression(nameof(argument))] string? paramName = null)
	{
		if (IsNullRef(ref argument))
		{
			throw new ArgumentNullRefException(paramName);
		}
	}
}
