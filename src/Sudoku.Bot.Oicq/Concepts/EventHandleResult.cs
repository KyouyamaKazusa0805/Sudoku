namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines the result after the event being handled.
/// </summary>
public enum EventHandleResult
{
	/// <summary>
	/// Indicates the result is ignored.
	/// </summary>
	NEGLECT = 0,

	/// <summary>
	/// Indicates the result is continued.
	/// </summary>
	CONTINUE = 1,

	/// <summary>
	/// Indicates the result is intercepted.
	/// </summary>
	INTERCEPT = 2,

	/// <summary>
	/// Indicates the result is agreed.
	/// </summary>
	AGREE = 10,

	/// <summary>
	/// Indicates the result is refused.
	/// </summary>
	REFUSE = 20
}
