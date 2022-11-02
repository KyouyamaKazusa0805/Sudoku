namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the help command.
/// </summary>
[Command]
internal sealed class HelpCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_Help"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		await e.SendMessageAsync(R["_HelpMessage"]);
		return true;
	}
}
