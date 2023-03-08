namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
file sealed class MentioningModule : IModule
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
