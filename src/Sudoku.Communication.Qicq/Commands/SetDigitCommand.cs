namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the set digit command.
/// </summary>
[Command]
internal sealed class SetDigitCommand : Command
{
	/// <inheritdoc/>
	public override string? EnvironmentCommand => R["_Command_Draw"];


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var command = R["_Command_Set"]!;
		if (!args.StartsWith(command))
		{
			return false;
		}

		args = args[command.Length..];
		var split = args.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		if (split is not [var rawCoordinate, [var rawDigit and >= '0' and <= '9']])
		{
			return false;
		}

		if (GetCell(rawCoordinate) is not { } cell)
		{
			return false;
		}

		Debug.Assert(Painter is not null);

		Puzzle[cell] = rawDigit - '1';
		Painter.WithGrid(Puzzle);

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
