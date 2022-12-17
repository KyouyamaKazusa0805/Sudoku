namespace Sudoku.Platforms.QQ.Scheduler;

/// <summary>
/// Defines a scheduled command.
/// </summary>
public abstract class ScheduledCommand
{
	/// <summary>
	/// Indicates the <see cref="TimeOnly"/> instance that describes the time that the operation will be triggered daily.
	/// </summary>
	public abstract TimeOnly TriggeringTime { get; }


	/// <summary>
	/// Try to execute the task.
	/// </summary>
	/// <returns>The task.</returns>
	public abstract Task ExecuteTaskAsync();
}
