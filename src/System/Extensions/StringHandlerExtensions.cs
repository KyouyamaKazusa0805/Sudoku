namespace System.Text;

/// <summary>
/// Provides the extension methods on <see cref="StringHandler"/>.
/// </summary>
/// <seealso cref="StringHandler"/>
public static class StringHandlerExtensions
{
	/// <summary>
	/// Append the content into the handler if the specified condition is satisfied.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="value">The value to append.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendWhen(this scoped ref StringHandler @this, bool condition, string value)
	{
		if (condition)
		{
			@this.Append(value);
		}
	}

	/// <summary>
	/// Append a serial of strings converted from a serial of elements.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="this">The handler.</param>
	/// <param name="enumFlags">The list of enumeration flags.</param>
	/// <param name="converter">
	/// The converter that allows the instance to convert into the <see cref="string"/> representation,
	/// whose the rule is defined as a method specified as the function pointer as this argument.
	/// </param>
	/// <param name="separator">The separator.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe void AppendRangeWithSeparatorUnsafe<T>(
		this scoped ref StringHandler @this,
		T enumFlags,
		delegate*<T, string> converter,
		string separator
	) where T : unmanaged, Enum
	{
		foreach (var enumFlag in enumFlags)
		{
			@this.Append(converter(enumFlag));
			@this.Append(separator);
		}

		@this.RemoveFromEnd(separator.Length);
	}
}
