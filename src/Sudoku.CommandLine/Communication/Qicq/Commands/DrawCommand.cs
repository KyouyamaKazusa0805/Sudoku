namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates draw command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
internal sealed class DrawCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_DrawSub"]!;

	/// <inheritdoc/>
	public override string EnvironmentCommand => R["_Command_Draw"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var split = args.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (split is not [var rawCoordinate, var rawIdentifier])
		{
			return false;
		}

		if (ICommandDataProvider.GetIdentifier(rawIdentifier) is not { } identifier)
		{
			return false;
		}

		var triplet = ICommandDataProvider.GetCoordinate(rawCoordinate);
		if (triplet is (false, false, false))
		{
			return false;
		}

		Debug.Assert(Painter is not null);

		var tempNodes = new List<ViewNode>();
		triplet.Switch(
			cells => cells.ForEach(cell => tempNodes.Add(new CellViewNode(identifier, cell))),
			candidates => candidates.ForEach(candidate => tempNodes.Add(new CandidateViewNode(identifier, candidate))),
			house => tempNodes.Add(new HouseViewNode(identifier, house))
		);

		Painter.AddNodes(tempNodes);

		await e.SendPictureThenDeleteAsync();
		return true;
	}
}
