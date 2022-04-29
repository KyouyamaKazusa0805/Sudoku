namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Defines a test event handler.
/// </summary>
public sealed class TestPluginEvent : IPluginEvent
{
	/// <inheritdoc/>
	public AmiableEventType EventType => AmiableEventType.Group;


	/// <inheritdoc/>
	public void Process(AmiableEventArgs e)
	{
		if (e is not AmiableMessageEventArgs { UserId: 747507738L, GroupId: 663888612L, RawMessage: "#Hello" } args)
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("机器人测试成功! 接触神经元链接稳定!");
		sb.AppendLine($"当前API:{AmiableService.ApiKey}");

		args.SendMessage(sb.ToString());
	}
}
