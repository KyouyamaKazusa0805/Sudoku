namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Defines an end-gaming command.
/// </summary>
[Command]
internal sealed class EndGamingCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_EndGaming"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		GamingCancellationToken = true;
		await Task.Delay(10);

		return true;
	}
}
