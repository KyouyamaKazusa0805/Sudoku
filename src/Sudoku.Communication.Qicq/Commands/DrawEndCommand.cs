namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Indicates the draw end command.
/// </summary>
[Command]
internal sealed class DrawEndCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R["_Command_End"]!;

	/// <inheritdoc/>
	public override string EnvironmentCommand => R["_Command_Draw"]!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		EnvironmentCommandExecuting = null;
		Painter = null;
		Puzzle = Grid.Empty;

		await e.SendMessageAsync(R["_MessageFormat_EndOkay"]!);
		return true;
	}
}
