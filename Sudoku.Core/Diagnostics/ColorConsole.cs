using System;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Provides a <see cref="Console"/> that output the strings with colors.
	/// </summary>
	/// <seealso cref="Console"/>
	public static class ColorConsole
	{
		/// <summary>
		/// Writes the text representation of the specified object, followed by the
		/// current line terminator, to the standard output stream.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="foreground">The foreground color.</param>
		/// <param name="background">The background color.</param>
		public static void WriteLine(object? obj, ConsoleColor foreground, ConsoleColor background)
		{
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
			Console.WriteLine(obj);
			Console.ResetColor();
		}

		/// <summary>
		/// Writes the text representation of the specified object,
		/// to the standard output stream.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="foreground">The foreground color.</param>
		/// <param name="background">The background color.</param>
		public static void Write(object? obj, ConsoleColor foreground, ConsoleColor background)
		{
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
			Console.Write(obj);
			Console.ResetColor();
		}
	}
}
