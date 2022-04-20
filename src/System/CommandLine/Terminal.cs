namespace System.CommandLine;

/// <summary>
/// Provides with the operations for the terminal.
/// </summary>
public static class Terminal
{
	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write([InterpolatedStringHandlerArgument] ref StringHandler handler) =>
		Console.Write(handler.ToStringAndClear());

	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	/// <param name="foreground">The foreground color.</param>
	public static void Write(
		[InterpolatedStringHandlerArgument] ref StringHandler handler, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		Console.Write(handler.ToStringAndClear());
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	public static void Write(
		[InterpolatedStringHandlerArgument] ref StringHandler handler, ConsoleColor foreground,
		ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.Write(handler.ToStringAndClear());
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.Write(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<T>(T value) => Console.Write(value);

	/// <summary>
	/// Writes the text representation of the specified object to the standard output stream,
	/// with the specified foreground.
	/// </summary>
	/// <typeparam name="T">The type of the object to be displayed.</typeparam>
	/// <param name="value"><inheritdoc cref="Console.Write(object?)"/></param>
	/// <param name="foreground">The foreground color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<T>(T value, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		Console.Write(value);
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the text representation of the specified object to the standard output stream,
	/// with the specified foreground.
	/// </summary>
	/// <typeparam name="T">The type of the object to be displayed.</typeparam>
	/// <param name="value"><inheritdoc cref="Console.Write(object?)"/></param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<T>(T value, ConsoleColor foreground, ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.Write(value);
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.WriteLine()"/>
	public static void WriteLine() => Console.WriteLine();

	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine([InterpolatedStringHandlerArgument] ref StringHandler handler) =>
		Console.WriteLine(handler.ToStringAndClear());

	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	/// <param name="foreground">The foreground color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine(
		[InterpolatedStringHandlerArgument] ref StringHandler handler, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		Console.WriteLine(handler.ToStringAndClear());
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value specified the <see cref="StringHandler"/> as an interpolated string,
	/// followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="handler">The string handler that holds the interpolated string.</param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine(
		[InterpolatedStringHandlerArgument] ref StringHandler handler, ConsoleColor foreground,
		ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.WriteLine(handler.ToStringAndClear());
		Console.ResetColor();
	}
}
