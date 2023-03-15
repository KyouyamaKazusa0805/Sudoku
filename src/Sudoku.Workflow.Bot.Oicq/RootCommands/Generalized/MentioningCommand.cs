namespace Sudoku.Workflow.Bot.Oicq.RootCommands.Generalized;

/// <summary>
/// 艾特指令。该指令较为特殊，它不带有前缀，也不是人触发的条件。
/// 它主要是为了去处理一些其他指令里的回复情况，如开始游戏后，回答问题需要艾特的时候，就会用到这个指令来处理。
/// </summary>
[Command]
file sealed class MentioningCommand : IModule
{
	/// <inheritdoc/>
	public bool? IsEnable { get; set; } = true;


	/// <inheritdoc/>
	public void Execute(MessageReceiverBase @base)
	{
		if (@base is not GroupMessageReceiver
			{
				GroupId: var groupId,
				Sender: var sender,
				MessageChain: [SourceMessage, AtMessage { Target: BotNumber }, PlainMessage { Text: var message }]
			})
		{
			return;
		}

		if (!RunningContexts.TryGetValue(groupId, out var context))
		{
			return;
		}

		switch (context)
		{
			case { AnsweringContext.CurrentRoundAnsweredValues: { } answeredValues, ExecutingCommand: "开始游戏" }:
			{
				var trimmed = message.Trim();
				if (int.TryParse(trimmed, out var validInteger))
				{
					answeredValues.Add(new(sender, validInteger));
				}

				break;
			}
		}
	}
}
