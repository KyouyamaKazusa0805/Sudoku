namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一种分数构成的分布规则。
/// </summary>
public enum Distribution
{
	/// <summary>
	/// 表示分布使用常量分布规则。在计算一些结果的时候，不论抽取次数和抽取人的数据，都只能得到同样的结果。
	/// </summary>
	Constant,

	/// <summary>
	/// 表示分布使用指数分布规则。意味着越极端的结果对应的数值越极端（越难取到）。
	/// </summary>
	Exponent,

	/// <summary>
	/// 表示分布使用高斯分布（正态分布）。意味着中间数据能取到的概率最高，最低是两端的数据。
	/// </summary>
	Normal
}
