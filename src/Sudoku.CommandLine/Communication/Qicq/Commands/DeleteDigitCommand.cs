namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the delete command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
internal sealed class DeleteDigitCommand : Command
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

		Debug.Assert(Painter is not null);

		Puzzle[cell, rawDigit - '1'] = false;
		Painter.WithGrid(Puzzle);

		await e.SendPictureThenDeleteAsync();
		return true;
	}
}
