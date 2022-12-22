namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the help command.
/// </summary>
[Command]
file sealed class HelpCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("Help")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		await e.SendMessageAsync(R["_HelpMessage"]);
		return true;
	}
}
