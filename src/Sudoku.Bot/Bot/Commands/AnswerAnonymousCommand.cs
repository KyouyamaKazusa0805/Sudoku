namespace Sudoku.Bot.Commands;

/// <summary>
/// 表示游戏的匿名指令。
/// </summary>
[AnonymousCommand("PK")]
public sealed class AnswerAnonymousCommand : AnonymousCommand
{
	/// <inheritdoc/>
	public override async Task GroupCallback(ChatMessageApi api, ChatMessage message)
	{
		var valueString = message.GetPlainArguments();
		switch (BotRunningContext.GetContext(message.GroupOpenId))
		{
			case { AnsweringContext.CurrentTimesliceAnswers: var list, Tag: GameMode.NineMatch }:
			{
				var task = Task.FromResult(new NineMatchUserAnswer(message.Author.MemberOpenId, valueString));
				list.Add(await task);
				break;
			}
		}
	}
}
