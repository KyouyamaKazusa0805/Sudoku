namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the draw end command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class DrawEndCommand : Command
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
		var context = RunningContexts[e.GroupId];
		context.ExecutingCommand = null;
		context.DrawingContext = new();

		await e.SendMessageAsync(R["_MessageFormat_EndOkay"]!);
		return true;
	}
}
