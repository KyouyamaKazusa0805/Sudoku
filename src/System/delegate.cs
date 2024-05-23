namespace System;

/// <summary>
/// Represents a set of method groups that can be used as delegate-typed arguments, in easy ways.
/// </summary>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
public static class @delegate
{
	/// <summary>
	/// Do nothing. This method is equivalent to lambda expression <c>static () => {}</c>.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DoNothing()
	{
	}

	/// <summary>
	/// Makes the variable <paramref name="value"/> be an equivalent <see cref="bool"/> value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value to be checked.</param>
	/// <returns>The logical conversion result.</returns>
	/// <remarks>
	/// This method does not use casting to make the target <typeparamref name="T"/> value to be converted into a <see cref="bool"/> value.
	/// If the type <typeparamref name="T"/> has implemented <see cref="ILogicalOperators{TSelf}"/>, C# language will allow this type
	/// using <see cref="bool"/>-like operators <see langword="operator"/> <![CDATA[&&]]> and <see langword="operator"/> ||
	/// to determine multiple <typeparamref name="T"/> values are <see langword="true"/> or <see langword="false"/> via the specified way
	/// to be checked. This is just like C programming language rule - allowing any implicit casts from an integer to a boolean value like:
	/// <code><![CDATA[
	/// if (integer) // integer != 0 in C and op_True(integer) in C#
	/// {
	///     // ...
	/// }
	/// ]]></code>
	/// The backing implementation is like the following code in C#:
	/// <code><![CDATA[
	/// struct Int32
	/// {
	///     public static bool operator true(int value) => value != 0;
	///     public sttaic bool operator false(int value) => value == 0;
	/// }
	/// ]]></code>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool True<T>(T value) where T : ILogicalOperators<T> => !!value;

	/// <inheritdoc cref="True{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool True<T>(scoped ref readonly T value) where T : ILogicalOperators<T> => !!value;

	/// <summary>
	/// Makes the variable <paramref name="value"/> be an equivalent <see cref="bool"/> value, and negate it.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="value">The value to be checked.</param>
	/// <returns>The logical conversion result.</returns>
	/// <remarks>
	/// <inheritdoc cref="True{T}(T)" path="/remarks"/>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool False<T>(T value) where T : ILogicalOperators<T> => !value;

	/// <inheritdoc cref="False{T}(T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool False<T>(scoped ref readonly T value) where T : ILogicalOperators<T> => !value;

	/// <summary>
	/// Provides with the default way to convert the specified instance of type <see cref="short"/>
	/// into a <see cref="string"/> value.
	/// </summary>
	/// <typeparam name="T">The type of the argument.</typeparam>
	/// <param name="this">The instance.</param>
	/// <returns>The <see cref="string"/> value.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the argument <paramref name="this"/> return <see langword="null"/>
	/// as the <c>ToString</c> method result.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToStringConverter<T>(scoped ref readonly T @this) where T : notnull
		=> @this.ToString().Unwrap();

	/// <summary>
	/// Returns an empty string equivalent to <see cref="string.Empty"/> no matter what the argument is.
	/// </summary>
	/// <typeparam name="T">The type of the argument. In fact the argument won't be used in this method.</typeparam>
	/// <param name="instance">The instance. In fact the argument won't be used in this method.</param>
	/// <returns>A string that is equal to <see cref="string.Empty"/>.</returns>
	/// <seealso cref="string.Empty"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ReturnEmptyString<T>(T instance) => string.Empty;

	/// <summary>
	/// Merges two integers by bits. This method will be used by LINQ method
	/// <see cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"/>.
	/// </summary>
	/// <typeparam name="T">The type of the value. The value must be an integer type, and supports for shifting operators.</typeparam>
	/// <param name="interim">The interim value.</param>
	/// <param name="next">The next value to be merged by its bits.</param>
	/// <returns>The final value merged.</returns>
	/// <seealso cref="Enumerable.Aggregate{TSource}(IEnumerable{TSource}, Func{TSource, TSource, TSource})"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T BitMerger<T>(T interim, T next) where T : IBinaryInteger<T>, IShiftOperators<T, T, T>
		=> interim | T.MultiplicativeIdentity << next;

	/// <summary>
	/// Merges two flags of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the enumeration.</typeparam>
	/// <param name="left">The first instance to be merged.</param>
	/// <param name="right">The second instance to be merged.</param>
	/// <returns>A merged result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe T EnumFlagMerger<T>(T left, T right) where T : unmanaged, Enum
		=> sizeof(T) switch
		{
			1 or 2 or 4 when (Unsafe.As<T, int>(ref left) | Unsafe.As<T, int>(ref right)) is var f => Unsafe.As<int, T>(ref f),
			8 when (Unsafe.As<T, long>(ref left) | Unsafe.As<T, long>(ref right)) is var f => Unsafe.As<long, T>(ref f),
			_ => throw new NotSupportedException(ResourceDictionary.ExceptionMessage("UnderlyingTypeNotSupported"))
		};

	/// <summary>
	/// Returns the argument <paramref name="value"/>.
	/// </summary>
	/// <typeparam name="T">The type of the argument.</typeparam>
	/// <param name="value">The value.</param>
	/// <returns>The value itself.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T Self<T>(T value) => value;

	/// <summary>
	/// Represents Y-Combinator. This method can allow you create recursive lambdas.
	/// </summary>
	/// <typeparam name="T">The type of the argument to be passed.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="f">The recursion logic.</param>
	/// <returns>A function that creates a nesting lambda that is a recursive lambda.</returns>
	/// <remarks>
	/// You can use this to create customized recursive lambda:
	/// <code><![CDATA[
	/// //                           T TResult
	/// var factorial = YCombinator<int, int>(lambda => x => x == 0 ? 1 : x * lambda(x - 1));
	/// //                          ~~~  ~~~
	/// int result = factorial(5); // T -> TResult
	/// Console.WriteLine(result);
	/// ]]></code>
	/// </remarks>
	public static Func<T, TResult> YCombinator<T, TResult>(Func<Func<T, TResult>, Func<T, TResult>> f)
		// We cannot simplify the lambda to 'f(YCombinator(f))' here.
		=> value => f(YCombinator(f))(value);
}
