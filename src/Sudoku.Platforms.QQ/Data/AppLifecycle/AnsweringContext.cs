﻿namespace Sudoku.Platforms.QQ.Data.AppLifecycle;

/// <summary>
/// The answering context.
/// </summary>
internal sealed class AnsweringContext
{
	/// <summary>
	/// The answered users.
	/// </summary>
	public ConcurrentDictionary<string, int> AnsweredUsers { get; set; } = new();

	/// <summary>
	/// The answered raw values in a loop-scoped round.
	/// </summary>
	public ConcurrentBag<UserPuzzleAnswerDetails> CurrentRoundAnsweredValues { get; set; } = new();

	/// <summary>
	/// Indicates whether the user has emitted cancelled command.
	/// </summary>
	public bool IsCancelled { get; set; }
}
