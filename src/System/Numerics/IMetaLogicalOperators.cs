namespace System.Numerics;

/// <summary>
/// Defines the type that can use the following operators:
/// <list type="bullet">
/// <item><see cref="operator true(T)"/></item>
/// <item><see cref="operator false(T)"/></item>
/// </list>
/// </summary>
/// <typeparam name="T">The type of the current instance.</typeparam>
/// <remarks>
/// <para>
/// The meta logical operators can be used as a part of the full expansion
/// of operators <c><![CDATA[&&]]></c> and <c><![CDATA[||]]></c>.
/// </para>
/// <para>
/// For more information, you can visit
/// <see href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/true-false-operators">
/// this link
/// </see>.
/// </para>
/// </remarks>
public interface IMetaLogicalOperators</*[Self]*/ T> where T : IMetaLogicalOperators<T>
{
	/// <summary>
	/// <para>
	/// Checks whether the current instance is at the <see langword="true"/> status.
	/// The operator will be invoked by <c><![CDATA[operator ||]]></c>, which cannot be overloadable directly.
	/// </para>
	/// <para>
	/// The expression <c><![CDATA[a || b]]></c> will be expanded to <c><![CDATA[false(a) ? a : a | b]]></c>,
	/// where the variable <c>a</c> corresponds to the argument <paramref name="value"/>.
	/// </para>
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current instance is
	/// at the <see langword="true"/> status.
	/// </returns>
	static abstract bool operator true(T value);

	/// <summary>
	/// <para>
	/// Checks whether the current instance is at the <see langword="false"/> status.
	/// The operator will be invoked by <c><![CDATA[operator &&]]></c>, which cannot be overloadable directly.
	/// </para>
	/// <para>
	/// The expression <c><![CDATA[a && b]]></c> will be expanded to <c><![CDATA[false(a) ? a : a & b]]></c>,
	/// where the variable <c>a</c> corresponds to the argument <paramref name="value"/>.
	/// </para>
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>
	/// The <see cref="bool"/> result indicating whether the current instance is
	/// at the <see langword="false"/> status.
	/// </returns>
	static abstract bool operator false(T value);
}
