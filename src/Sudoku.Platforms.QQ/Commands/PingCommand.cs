namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a ping command.
/// </summary>
[Command(Permissions.Administrator, Permissions.Owner)]
file sealed class PingCommand : Command
{
	/// <inheritdoc/>
	public override string[] Prefixes => new[] { "#" };

	/// <inheritdoc/>
	public override string CommandName => "ping";

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Strict;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		using var ping = new Ping();
		_ = ping.Send("www.baidu.com") switch
		{
			{ Status: IPStatus.Success, RoundtripTime: var time }
				=> await e.SendMessageAsync(string.Format(R["_MessageFormat_PingSuccess"]!, time)),
			_
				=> await e.SendMessageAsync(R["_MessageFormat_PingFailed"]!)
		};

		return true;
	}
}
