namespace Mirai.Net.Modules;

/// <summary>
/// Represents a periodic module.
/// </summary>
public interface IPeriodicModule
{
	/// <summary>
	/// Indicates the time value when the module is running every day.
	/// </summary>
	TimeOnly TriggeringTime { get; }


	/// <summary>
	/// Try to execute the periodic module.
	/// </summary>
	void Execute();
}
