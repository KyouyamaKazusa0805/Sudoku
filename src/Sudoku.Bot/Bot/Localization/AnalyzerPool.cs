namespace Sudoku.Bot.Localization;

/// <summary>
/// 提供分析器对象的初始化和生成。
/// </summary>
internal static class AnalyzerPool
{
	/// <summary>
	/// 分析器对象。
	/// </summary>
	public static Analyzer Analyzer { get; } = new();
}
