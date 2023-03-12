namespace Sudoku.Platforms.QQ.Modules.Group;

[GroupModule("ping")]
[RequiredRole(SenderRole = GroupRoleKind.God)]
file sealed class PingModule : GroupModule
{
	/// <inheritdoc/>
	public override string[] RaisingPrefix => CommonCommandPrefixes.HashTag;


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
