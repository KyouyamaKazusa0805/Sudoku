namespace System.CommandLine;

/// <summary>
/// Provides with the operations for the terminal.
/// </summary>
public static class Terminal
{
	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write(string s) => Console.Write(s);

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	public static void Write(string s, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		Console.Write(s);
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	public static void Write(string s, ConsoleColor foreground, ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.Write(s);
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine() => Console.WriteLine();

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine<T>(T value) => Console.WriteLine(value?.ToString());

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="foreground">The foreground color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine<T>(T value, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		Console.WriteLine(value?.ToString());
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine<T>(T value, ConsoleColor foreground, ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.WriteLine(value?.ToString());
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine(string s) => Console.WriteLine(s);

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine(string s, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		Console.WriteLine(s);
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine(string s, ConsoleColor foreground, ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.WriteLine(s);
		Console.ResetColor();
	}
}
