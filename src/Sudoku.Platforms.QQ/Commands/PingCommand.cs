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
		switch (ping.Send("www.baidu.com"))
		{
			case { Status: IPStatus.Success, RoundtripTime: var time }:
			{
				await e.SendMessageAsync(string.Format(R["_MessageFormat_PingSuccess"]!, time));

				break;
			}
			default:
			{
				await e.SendMessageAsync(R["_MessageFormat_PingFailed"]!);

				break;
			}
		}

		return true;
	}
}
