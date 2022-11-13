namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the clear command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
internal sealed class ClearCommand : Command
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

		Debug.Assert(Painter is not null);

		triplet.Switch(
			cells => cells.ForEach(cell => Painter.RemoveNodesWhen(r => r is CellViewNode { Cell: var c } && c == cell)),
			candidates => candidates.ForEach(candidate => Painter.RemoveNodesWhen(r => r is CandidateViewNode { Candidate: var c } && c == candidate)),
			house => Painter.RemoveNodesWhen(r => r is HouseViewNode { House: var h } && h == house)
		);

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
