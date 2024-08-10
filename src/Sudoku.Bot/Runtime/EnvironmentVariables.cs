namespace Sudoku.Runtime;

/// <summary>
/// 提供的是环境变量。
/// </summary>
internal sealed class EnvironmentVariables
{
	/// <summary>
	/// 机器人的运行上下文。为一个并发字典，按群存储不同的上下文数据。
	/// </summary>
	internal static readonly ConcurrentDictionary<string, BotRunningContext> RunningContexts = new();
}
