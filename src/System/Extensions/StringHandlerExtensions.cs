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
	public static void AppendWhen(this ref StringHandler @this, bool condition, string value)
	{
		if (condition)
		{
			@this.Append(value);
		}
	}
}
