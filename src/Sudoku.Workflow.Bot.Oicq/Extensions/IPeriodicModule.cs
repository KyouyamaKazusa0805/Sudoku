namespace Mirai.Net.Modules;

/// <summary>
/// 提供一个周期性自动执行的模块。这个模块不同于群指令的模块，它在每一天的固定时候自动执行。
/// </summary>
public interface IPeriodicModule
{
	/// <summary>
	/// 表示该模块自动执行的时间。由于它每天都执行，因此无关日期数据，所以返回值类型为 <see cref="TimeOnly"/> 而不是 <see cref="DateTime"/>。
	/// </summary>
	TimeOnly TriggeringTime { get; }


	/// <summary>
	/// 执行的具体实现逻辑。
	/// </summary>
	void Execute();
}
