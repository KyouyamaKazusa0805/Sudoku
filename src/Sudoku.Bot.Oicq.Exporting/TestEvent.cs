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
		if (
			e is not AmiableMessageEventArgs
			{
				UserId: 747507738L,
				GroupId: 663888612L or 924849321L,
				RawMessage: "#Hello"
			} args
		)
		{
			return;
		}

		args.SendMessage(
			"""
			机器人测试成功! 接触神经元链接稳定!
			
			向日葵的数独机器人
			版本：V0.1
			
			※ 项目目前刚开始开发中，只支持群主使用，见谅。
			
			"""
		);
	}
}
