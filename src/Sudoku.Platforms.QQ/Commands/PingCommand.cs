namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a ping command.
/// </summary>
[Command(Permissions.Administrator, Permissions.Owner)]
file sealed class PingCommand : Command
{
	/// <inheritdoc/>
	public override string[] Prefixes => CommonCommandPrefixes.HashTag;

	/// <inheritdoc/>
	public override string CommandName => "ping";

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		using var ping = new Ping();
		await e.SendMessageAsync(
			ping.Send("www.baidu.com") switch
			{
				{ Status: IPStatus.Success, RoundtripTime: var time } => string.Format(R.MessageFormat("PingSuccess")!, time),
				_ => R.MessageFormat("PingFailed")!
			}
		);

		return true;
	}
}
