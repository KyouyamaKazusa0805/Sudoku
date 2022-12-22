namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the draw start command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class DrawStartCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("Draw")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var context = RunningContexts[e.GroupId];
		context.ExecutingCommand = CommandName;
		context.DrawingContext.Painter = ISudokuPainter.Create(800, 10).WithGrid(Grid.Empty).WithRenderingCandidates(false);
		context.DrawingContext.Puzzle = Grid.Empty;

		await e.SendMessageAsync(R.MessageFormat("DrawStartMessage")!);
		return true;
	}
}
