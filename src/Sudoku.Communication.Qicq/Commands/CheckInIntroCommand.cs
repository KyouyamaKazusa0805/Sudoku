namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates check-in introduction command.
/// </summary>
[Command]
internal sealed class CheckInIntroCommand : Command
{
	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (args != R["_Command_CheckInIntro"])
		{
			return false;
		}

		await e.SendMessageAsync(R["_MessageFormat_CheckInIntro"]);
		return true;
	}
}
