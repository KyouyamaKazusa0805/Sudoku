using System.Runtime.CompilerServices;
using Sudoku.Cli.Commands;

namespace System.CommandLine;

/// <summary>
/// Provides with extension methods on <see cref="RootCommand"/>.
/// </summary>
/// <seealso cref="RootCommand"/>
public static class RootCommandExtensions
{
	/// <summary>
	/// Adds a (an) <typeparamref name="T"/> instance as a subcommand to the command.
	/// </summary>
	/// <typeparam name="T">The type of the command.</typeparam>
	/// <param name="this">The current <see cref="RootCommand"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddCommand<T>(this RootCommand @this) where T : Command, ICommand<T>, new()
		=> @this.AddCommand(ICommand<T>.CreateCommand());
}
