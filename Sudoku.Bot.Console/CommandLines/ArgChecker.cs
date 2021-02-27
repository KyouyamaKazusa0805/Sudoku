using System;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;
using System.Linq;

namespace Sudoku.Bot.Console.CommandLines
{
	/// <summary>
	/// Encapsulates a command line arguments checker.
	/// </summary>
	public static class ArgChecker
	{
		/// <summary>
		/// Check whether the arguments contains the config mode symbol <c>/config</c>.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool IsConfigMode(string[] args) => args.Contains(Commands.ConfigMode);

		/// <summary>
		/// Try to get the path from the arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="path">(<see langword="out"/> parameter) The result path.</param>
		/// <returns></returns>
		public static bool TryGetPath(string[] args, [NotNullWhen(true)] out string? path)
		{
			if (Array.IndexOf(args, Commands.Path) is not (var index and not -1))
			{
				path = null;
				return false;
			}

			if (index + 1 >= args.Length)
			{
				path = null;
				return false;
			}

			path = args[index + 1];
			return true;
		}
	}
}
