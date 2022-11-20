namespace Sudoku.Communication.Qicq.AppLifecycle;

/// <summary>
/// The answering context.
/// </summary>
internal sealed class AnsweringContext
{
	/// <summary>
	/// The answered users.
	/// </summary>
	public ConcurrentBag<string> AnsweredUsers { get; set; } = new();

	/// <summary>
	/// The answered raw values in a loop-scoped round.
	/// </summary>
	public ConcurrentBag<UserPuzzleAnswerData> CurrentRoundAnsweredValues { get; set; } = new();
}
