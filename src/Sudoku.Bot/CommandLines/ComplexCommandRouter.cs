namespace Sudoku.Bot.CommandLines
{
	/// <summary>
	/// Indicates a complex command router.
	/// </summary>
	public readonly struct ComplexCommandRouter
	{
		/// <summary>
		/// Initializes an instance with the specified command names and the handler.
		/// </summary>
		/// <param name="handler">The handler which is executed when the command is triggered.</param>
		/// <param name="commands">The command names.</param>
		public ComplexCommandRouter(CommandHandler handler, params string[] commands)
		{
			Commands = commands;
			Handler = handler;
		}


		/// <summary>
		/// Indicates the command names. All commands will point to a same handler.
		/// </summary>
		public string[] Commands { get; }

		/// <summary>
		/// Indicates the corresponding handler.
		/// </summary>
		public CommandHandler Handler { get; }
	}
}
