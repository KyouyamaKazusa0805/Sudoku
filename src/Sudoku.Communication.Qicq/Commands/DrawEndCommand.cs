namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the draw end command.
/// </summary>
[Command]
internal sealed class DrawEndCommand : Command
{
	/// <inheritdoc/>
	public override string? EnvironmentCommand => R["_Command_Draw"];


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (args != R["_Command_End"])
		{
			return false;
		}

		EnvironmentCommandExecuting = null;
		Painter = null;
		Puzzle = Grid.Empty;

		await e.SendMessageAsync(R["_MessageFormat_EndOkay"]!);
		return true;
	}
}
