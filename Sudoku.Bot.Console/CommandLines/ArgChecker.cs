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
		/// <returns>A <see cref="bool"/> result.</returns>
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

		/// <summary>
		/// Try to get the size value from the arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <param name="size">
		/// (<see langword="out"/> parameter) The result size. If failed to get, the value is always 800.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool TryGetSize(string[] args, out int size)
		{
			if (Array.IndexOf(args, Commands.Size) is not (var index and not -1))
			{
				size = 800;
				return false;
			}

			if (index + 1 >= args.Length)
			{
				size = 800;
				return false;
			}

			if (!int.TryParse(args[index + 1], out size) || size is <= 100 or >= 1600)
			{
				size = 800;
				return false;
			}

			return true;
		}
	}
}
