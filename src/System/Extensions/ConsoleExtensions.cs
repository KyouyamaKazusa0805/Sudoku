namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Console"/>.
/// </summary>
/// <seealso cref="Console"/>
public static class ConsoleExtensions
{
	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine([InterpolatedStringHandlerArgument] ref StringHandler handler) =>
		Console.WriteLine(handler.ToStringAndClear());
}
