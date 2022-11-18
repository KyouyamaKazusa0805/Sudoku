namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the draw start command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
internal sealed class DrawStartCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_Draw"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		EnvironmentCommandExecuting = CommandName;
		Puzzle = Grid.Empty;
		Painter = ISudokuPainter.Create(800, 10).WithGrid(Puzzle).WithRenderingCandidates(false);

		await e.SendMessageAsync(R["_MessageFormat_DrawStartMessage"]!);
		return true;
	}
}
