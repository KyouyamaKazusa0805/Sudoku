namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates draw command.
/// </summary>
[Command]
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

		if (GetIdentifier(rawIdentifier) is not { } identifier)
		{
			return false;
		}

		var triplet = GetCoordinate(rawCoordinate);
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

		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			// This folder is special; if the computer does not contain the folder, we should return directly.
			return true;
		}

		var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var botUsersDataFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			Directory.CreateDirectory(botUsersDataFolder);
		}

		var picturePath = $"""{botUsersDataFolder}\temp.png""";
		Painter.SaveTo(picturePath);

		await e.SendMessageAsync(new ImageMessage { Path = picturePath });

		File.Delete(picturePath);
		return true;
	}
}
