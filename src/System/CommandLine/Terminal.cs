namespace System.CommandLine;

/// <summary>
/// Provides with the operations for the terminal.
/// </summary>
public static class Terminal
{
	/// <summary>
	/// Pauses the command line, and wait for user pressing a key to continue.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Pause() => Console.ReadKey();

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
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<TNotNull>(TNotNull value) where TNotNull : notnull => Console.Write(value.ToString());

	/// <summary>
	/// Writes the text representation of the specified object to the standard output stream,
	/// with the specified foreground.
	/// </summary>
	/// <typeparam name="TNotNull">The type of the object to be displayed.</typeparam>
	/// <param name="value"><inheritdoc cref="Console.Write(object?)"/></param>
	/// <param name="foreground">The foreground color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<TNotNull>(TNotNull value, ConsoleColor foreground) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		Console.Write(value.ToString());
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the text representation of the specified object to the standard output stream,
	/// with the specified foreground.
	/// </summary>
	/// <typeparam name="TNotNull">The type of the object to be displayed.</typeparam>
	/// <param name="value"><inheritdoc cref="Console.Write(object?)"/></param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Write<TNotNull>(TNotNull value, ConsoleColor foreground, ConsoleColor background) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.Write(value.ToString());
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.WriteLine()"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine() => Console.WriteLine();

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine<TNotNull>(TNotNull value) where TNotNull : notnull => Console.WriteLine(value.ToString());

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="foreground">The foreground color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine<TNotNull>(TNotNull value, ConsoleColor foreground) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		Console.WriteLine(value.ToString());
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteLine<TNotNull>(TNotNull value, ConsoleColor foreground, ConsoleColor background) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		Console.WriteLine(value.ToString());
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

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	public static async Task WriteAsync(string s) => await Console.Out.WriteAsync(s);

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	public static async Task WriteAsync(string s, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		await Console.Out.WriteAsync(s);
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	public static async Task WriteAsync(string s, ConsoleColor foreground, ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		await Console.Out.WriteAsync(s);
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.Write(object?)"/>
	/// <typeparam name="TNotNull">The type of the object to be displayed.</typeparam>
	public static async Task WriteAsync<TNotNull>(TNotNull value) where TNotNull : notnull => await Console.Out.WriteAsync(value.ToString());

	/// <summary>
	/// Writes the text representation of the specified object to the standard output stream,
	/// with the specified foreground.
	/// </summary>
	/// <typeparam name="TNotNull">The type of the object to be displayed.</typeparam>
	/// <param name="value"><inheritdoc cref="Console.Write(object?)"/></param>
	/// <param name="foreground">The foreground color.</param>
	public static async Task WriteAsync<TNotNull>(TNotNull value, ConsoleColor foreground) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		await Console.Out.WriteAsync(value.ToString());
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the text representation of the specified object to the standard output stream,
	/// with the specified foreground.
	/// </summary>
	/// <typeparam name="TNotNull">The type of the object to be displayed.</typeparam>
	/// <param name="value"><inheritdoc cref="Console.Write(object?)"/></param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	public static async Task WriteAsync<TNotNull>(TNotNull value, ConsoleColor foreground, ConsoleColor background) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		await Console.Out.WriteAsync(value.ToString());
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.WriteLine()"/>
	public static async Task WriteLineAsync() => await Console.Out.WriteLineAsync();

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	public static async Task WriteLineAsync<TNotNull>(TNotNull value) where TNotNull : notnull
		=> await Console.Out.WriteLineAsync(value.ToString());

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="foreground">The foreground color.</param>
	public static async Task WriteLineAsync<TNotNull>(TNotNull value, ConsoleColor foreground) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		await Console.Out.WriteLineAsync(value.ToString());
		Console.ResetColor();
	}

	/// <inheritdoc cref="Console.WriteLine(object?)"/>
	/// <typeparam name="TNotNull">The type argument that corresponds to the type of the argument <paramref name="value"/>.</typeparam>
	/// <param name="value"><inheritdoc/></param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	public static async Task WriteLineAsync<TNotNull>(TNotNull value, ConsoleColor foreground, ConsoleColor background) where TNotNull : notnull
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		await Console.Out.WriteLineAsync(value.ToString());
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	public static async Task WriteLineAsync(string s) => await Console.Out.WriteLineAsync(s);

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	public static async Task WriteLineAsync(string s, ConsoleColor foreground)
	{
		Console.ForegroundColor = foreground;
		await Console.Out.WriteLineAsync(s);
		Console.ResetColor();
	}

	/// <summary>
	/// Writes the string value, followed by the current line terminator to the standard output stream.
	/// </summary>
	/// <param name="s">The string.</param>
	/// <param name="foreground">The foreground color.</param>
	/// <param name="background">The background color.</param>
	public static async Task WriteLineAsync(string s, ConsoleColor foreground, ConsoleColor background)
	{
		Console.ForegroundColor = foreground;
		Console.BackgroundColor = background;
		await Console.Out.WriteLineAsync(s);
		Console.ResetColor();
	}
}
