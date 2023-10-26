#pragma warning disable IDE0032, IDE0044
using System.ComponentModel;
using System.Diagnostics;

namespace System.Collections.Generic;

/// <summary>
/// Represents a type that can create a <see cref="ValueList{T}"/> instance.
/// </summary>
/// <seealso cref="ValueList{T}"/>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ValueListCreator
{
	/// <summary>
	/// Creates a <see cref="ValueList{T}"/> instance via the specified list of values.
	/// </summary>
	/// <typeparam name="T">The type of each value.</typeparam>
	/// <param name="values">The values.</param>
	/// <returns>A <see cref="ValueList{T}"/> instance that contains a list of values.</returns>
	/// <remarks>
	/// Please note that the type uses unmanaged memory, so you must use <see langword="using"/> keyword
	/// to release unmanaged memory when you don't use it, such as:
	/// <code><![CDATA[
	/// using scoped var list = (ValueList<int>)[1, 2, 3, 4];
	/// 
	/// // ...
	/// ]]></code>
	/// </remarks>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified collection contains at least 257 elements that will make the collection failed to be initialized.
	/// </exception>
	[DebuggerStepThrough]
	public static ValueList<T> Create<T>(scoped ReadOnlySpan<T> values) where T : notnull
	{
		if (values.Length >= byte.MaxValue)
		{
			throw new InvalidOperationException("The specified collection must contain no more 256 elements.");
		}

		var result = new ValueList<T>((byte)values.Length);
		foreach (var element in values)
		{
			result.Add(element);
		}
		return result;
	}
}
