namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the draw start command.
/// </summary>
[Command]
internal sealed class DrawStartCommand : Command
{
	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (args != R["_Command_Draw"])
		{
			return false;
		}

		EnvironmentCommandExecuting = R["_Command_Draw"]!;
		Puzzle = Grid.Empty;
		Painter = ISudokuPainter.Create(800, 10).WithGrid(Puzzle).WithRenderingCandidates(false);

		await e.SendMessageAsync(R["_MessageFormat_DrawStartMessage"]!);

		return true;
	}
}
