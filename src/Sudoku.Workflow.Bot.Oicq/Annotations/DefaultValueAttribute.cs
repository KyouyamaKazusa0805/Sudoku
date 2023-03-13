namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 表示一个特性，用于命令模块的某属性，表示该属性作为命令模块的参数使用的时候，它需要被覆盖的默认数值是多少。
/// </summary>
/// <remarks>
/// 它不等同于属性的初始化器，由于项目的设计，所有的命令模块在运行期间都只保有一个实例，而不会产生新的实例。
/// 如果反复执行同一个指令，那么参数数值就会被保持为上一轮执行该指令期间产生的那个情况。为确保数值重新覆盖为默认数值，该特性专门用于这个处理规则。
/// </remarks>
/// <typeparam name="T">属性本身的类型。</typeparam>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DefaultValueAttribute<T> : CommandLineAnnotationAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="DefaultValueAttribute{T}"/> 类型的实例，并带有一个参数表示其默认数值。
	/// </summary>
	/// <param name="defaultValue">默认数值。</param>
	public DefaultValueAttribute(T? defaultValue) => DefaultValue = defaultValue;


	/// <summary>
	/// 表示该属性需要被覆盖掉的默认数值。
	/// </summary>
	public T? DefaultValue { get; }
}
