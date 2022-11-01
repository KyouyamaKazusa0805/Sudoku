namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the help command.
/// </summary>
[Command]
internal sealed class HelpCommand : Command
{
	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (args != R["_Command_Help"])
		{
			return false;
		}

		await e.SendMessageAsync(R["_HelpMessage"]);
		return true;
	}
}
