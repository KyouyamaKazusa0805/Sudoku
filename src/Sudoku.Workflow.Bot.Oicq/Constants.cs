namespace Sudoku.Workflow.Bot.Oicq;

/// <summary>
/// 表示该项目里反复使用的一些常量或静态只读量。
/// <b><i>虽然名字里写的是常量（constant），但是它仍然可以表示一些单纯只是只读，但不一定非得是编译期常量的量。</i></b>
/// </summary>
internal static partial class Constants
{
	/// <summary>
	/// 表示机器人的 QQ 账号。<b>如果你要使用机器人，请修改喂你自己的机器人的账号。</b>
	/// </summary>
	public const string BotNumber = "979329690";

	/// <summary>
	/// 表示一个全局最高权限的 QQ 账号。该账号可以在任何可以使用的地方使用机器人，避免权限不够导致触发不成功的情况。
	/// </summary>
	public const string GodNumber = "747507738";

	/// <summary>
	/// 这个是单独为数独群设立的一个特殊群账号。很多特殊限制都跟这个特殊群有关系。
	/// </summary>
	public const string SudokuGroupNumber = "924849321";
}
