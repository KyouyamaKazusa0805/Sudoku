namespace Sudoku.Workflow.Bot.Oicq.UserCommands;

/// <summary>
/// Ping 指令。
/// </summary>
[Command("ping")]
[RequiredRole(SenderRole = GroupRoleKind.God)]
internal sealed class PingCommand : Command
{
	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		using var ping = new Ping();
		await messageReceiver.SendMessageAsync(
			ping.Send("www.baidu.com") switch
			{
				{ Status: IPStatus.Success, RoundtripTime: var time } => $"测试连接成功。耗时 {time} 毫秒。",
				_ => "网络测试连接操作失败。请检查网络配置。"
			}
		);
	}
}
