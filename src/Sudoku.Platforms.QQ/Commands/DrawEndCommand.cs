namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Indicates the draw end command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class DrawEndCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("End")!;

	/// <inheritdoc/>
	public override string EnvironmentCommand => R.Command("Draw")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var context = RunningContexts[e.GroupId];
		context.ExecutingCommand = null;
		context.DrawingContext = new();

		await e.SendMessageAsync(R.MessageFormat("EndOkay")!);
		return true;
	}
}
