namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a gaming cancelling command.
/// </summary>
[Command]
file sealed class GamingCancellingCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("CancelGaming")!;

	/// <inheritdoc/>
	public override string EnvironmentCommand => R.Command("MatchStart")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var context = RunningContexts[e.GroupId];
		if (context.AnsweringContext.IsCancelled)
		{
			return true;
		}

		context.AnsweringContext.IsCancelled = true;
		await Task.Delay(10);
		return true;
	}
}
