namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于标记到指令模块的某个参数上，表示该参数是一个双参数指令。
/// </summary>
/// <remarks>
/// <b>双参数指令</b>表示一个参数，需要带一个额外参数，拼凑在一起，才是一个整体的参数信息。
/// </remarks>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DoubleArgumentAttribute : ArgumentAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="DoubleArgumentAttribute"/> 的实例，并给出其双参数的名称。
	/// </summary>
	/// <param name="name">表示参数的名称。</param>
	public DoubleArgumentAttribute(string name) => Name = name;


	/// <inheritdoc/>
	public override int RequiredValuesCount => 1;

	/// <summary>
	/// 表示该双参数的名称，即该参数左边的这个明确下来的数值。
	/// </summary>
	public string Name { get; }
}
