using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Bot.CommandLines
{
	/// <summary>
	/// Indicates a command router.
	/// </summary>
	[DisableParameterlessConstructor]
	public readonly struct CommandRouter
	{
		/// <summary>
		/// Initializes an instance with the specified command name and the handler.
		/// </summary>
		/// <param name="handler">The handler which is executed when the command is triggered.</param>
		/// <param name="command">The command name.</param>
		public CommandRouter(CommandHandler handler, string command)
		{
			Command = command;
			Handler = handler;
		}


		/// <summary>
		/// Indicates the command name.
		/// </summary>
		public string Command { get; }

		/// <summary>
		/// Indicates the corresponding handler.
		/// </summary>
		public CommandHandler Handler { get; }
	}
}
