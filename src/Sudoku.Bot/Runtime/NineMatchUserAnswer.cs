namespace Sudoku.Runtime;

/// <summary>
/// 表示用户在 <see cref="GameMode.NineMatch"/> 游戏模式下的填充结果。
/// </summary>
/// <param name="userId"><inheritdoc/></param>
/// <param name="answerString"><inheritdoc/></param>
public sealed class NineMatchUserAnswer(string userId, string answerString) : UserAnswer(userId, answerString)
{
	/// <inheritdoc/>
	public override GameMode Mode => GameMode.NineMatch;

	/// <inheritdoc/>
	public override Type ResultType => typeof(int);


	/// <inheritdoc/>
	public override bool IsValidAnswer([NotNullWhen(true)] out object? result)
	{
		var returnValue = int.TryParse(AnswerString, out var r);
		result = r;
		return returnValue;
	}
}
