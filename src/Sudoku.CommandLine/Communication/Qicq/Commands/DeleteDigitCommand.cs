namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the delete command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class DeleteDigitCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_Delete"]!;

	/// <inheritdoc/>
	public override string EnvironmentCommand => R["_Command_Draw"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var split = args.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (split is not [var rawCoordinate, [var rawDigit and >= '0' and <= '9']])
		{
			return false;
		}

		if (ICommandDataProvider.GetCell(rawCoordinate) is not { } cell)
		{
			return false;
		}

		var context = RunningContexts[e.GroupId];
		var drawingContext = context.DrawingContext!;
		var puzzle = drawingContext.Puzzle;
		puzzle[cell, rawDigit - '1'] = false;

		drawingContext.Puzzle = puzzle;
		drawingContext.Painter!.WithGrid(puzzle);

		await e.SendPictureThenDeleteAsync();
		return true;
	}
}
