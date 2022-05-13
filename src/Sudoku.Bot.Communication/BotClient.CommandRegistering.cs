namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// <para>Indicates the command prefix. The prefix is used for the recorgnization for the bot commands.</para>
	/// <para>The default value is <c>"/"</c>.</para>
	/// </summary>
	public string CommandPrefix { get; set; } = "/";

	/// <summary>
	/// Gets all registered commands.
	/// </summary>
	public Command[] RegisteredCommands => Commands.Values.ToArray();

	/// <summary>
	/// Indicates the commands registered.
	/// </summary>
	private ConcurrentDictionary<string, Command> Commands { get; } = new();


	/// <summary>
	/// Registers a command into the bot client, to make the client support this command.
	/// </summary>
	/// <param name="command">The command to be registered.</param>
	/// <remarks>
	/// Please note that the command will match the whole message with the part <c>@bot</c>,
	/// and remove the leading and trailing whitespaces.
	/// In addition, if the command is triggered, the event <see cref="MessageCreated"/> won't be triggered.
	/// </remarks>
	public void RegisterCommand(Command command)
	{
		string commandText = StringResource.Get("CommandNameText")!;

		string commandName = command.Name;
		if (Commands.ContainsKey(commandName))
		{
			string existsText = StringResource.Get("CommandAlreadyExists")!;
			Log.Warn($"[CommandManager] {commandText} {commandName} {existsText}");
		}
		else
		{
			Commands[commandName] = command;

			string registeredSuccessfully = StringResource.Get("CommandRegisteredSuccessfully")!;
			Log.Info($"[CommandManager] {commandText} {commandName} {registeredSuccessfully}");
		}
	}

	/// <summary>
	/// Unregisters a command from the bot client, to make the client support this command.
	/// </summary>
	/// <param name="commandName">The command name whose corresponding command should be unregistered.</param>
	public void UnregisterCommand(string commandName)
	{
		string commandText = StringResource.Get("CommandNameText")!;

		if (Commands.ContainsKey(commandName))
		{
			if (Commands.Remove(commandName, out _))
			{
				string removedSuccessfully = StringResource.Get("CommandRemovedSuccessfully")!;
				Log.Info($"[CommandManager] {commandText} {commandName} {removedSuccessfully}");
			}
			else
			{
				string removedFailed = StringResource.Get("CommandRemovedFailed")!;
				Log.Warn($"[CommandManager] {commandText} {commandName} {removedFailed}");
			}
		}
		else
		{
			string notExistsText = StringResource.Get("CommandNotExist")!;
			Log.Warn($"[CommandManager] {commandText} {commandName} {notExistsText}");
		}
	}
}
