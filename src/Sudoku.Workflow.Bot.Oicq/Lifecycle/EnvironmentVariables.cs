namespace Sudoku.Workflow.Bot.Oicq.Lifecycle;

/// <summary>
/// 表示一些环境变量。
/// </summary>
internal static class EnvironmentVariables
{
	/// <summary>
	/// 随机数生成器。
	/// </summary>
	public static readonly Random Rng = new();

	/// <summary>
	/// 题目生成器。
	/// </summary>
	public static readonly PatternBasedPuzzleGenerator Generator = new();

	/// <summary>
	/// 逻辑解题工具。
	/// </summary>
	public static readonly LogicalSolver Solver = CommonLogicalSolvers.Suitable with { };

	/// <summary>
	/// 自动填充题目一些无关紧要的填数的实例。
	/// </summary>
	public static readonly DefaultAutoFiller GridAutoFiller = new();

	/// <summary>
	/// 机器人的运行上下文。为一个并发字典，按群存储不同的上下文数据。
	/// </summary>
	internal static readonly ConcurrentDictionary<string, BotRunningContext> RunningContexts = new();
}
