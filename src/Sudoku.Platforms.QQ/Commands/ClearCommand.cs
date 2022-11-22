namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the clear command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class ClearCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_Clear"]!;

	/// <inheritdoc/>
	public override string? EnvironmentCommand => R["_Command_Draw"];

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var triplet = ICommandDataProvider.GetCoordinate(args);
		if (triplet is (false, false, false))
		{
			return false;
		}

		var painter = RunningContexts[e.GroupId].DrawingContext!.Painter!;

		triplet.Switch(
			cells => cells.ForEach(cell => painter.RemoveNodesWhen(r => r is CellViewNode { Cell: var c } && c == cell)),
			candidates => candidates.ForEach(candidate => painter.RemoveNodesWhen(r => r is CandidateViewNode { Candidate: var c } && c == candidate)),
			house => painter.RemoveNodesWhen(r => r is HouseViewNode { House: var h } && h == house)
		);

		await e.SendPictureThenDeleteAsync();

		return true;
	}
}
