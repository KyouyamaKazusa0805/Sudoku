namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于标记到周期性执行的模块（<see cref="PeriodicCommand"/> 的派生类型）的类型本身上面，表示该模块是启用状态。在运行期间会被反射给识别到。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PeriodicCommandAttribute : CommandAnnotationAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="PeriodicCommandAttribute"/> 类型的实例，并给出周期执行的小时数据。
	/// </summary>
	/// <param name="hour">小时。</param>
	public PeriodicCommandAttribute(int hour) : this(hour, 0, 0)
	{
	}

	/// <summary>
	/// 实例化一个 <see cref="PeriodicCommandAttribute"/> 类型的实例，并给出周期执行的时分两个数据。
	/// </summary>
	/// <param name="hour">小时。</param>
	/// <param name="minute">分。</param>
	public PeriodicCommandAttribute(int hour, int minute) : this(hour, minute, 0)
	{
	}

	/// <summary>
	/// 实例化一个 <see cref="PeriodicCommandAttribute"/> 类型的实例，并给出周期执行的时分秒三个数据。
	/// </summary>
	/// <param name="hour">小时。</param>
	/// <param name="minute">分。</param>
	/// <param name="second">秒。</param>
	public PeriodicCommandAttribute(int hour, int minute, int second) => TriggeringTime = new(hour, minute, second);


	/// <summary>
	/// 表示触发该指令的时间。注意返回值类型为 <see cref="TimeOnly"/> 是因为，
	/// 由于模块的底层设计，所有的周期性指令每天都执行一次，所以这个属性指示的是每天的什么时候执行。
	/// </summary>
	public TimeOnly TriggeringTime { get; }
}
