namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines an event filter.
/// </summary>
public interface IEventFilter
{
	/// <summary>
	/// Filters the specified plugin event and the Amiable event arguments.
	/// </summary>
	/// <param name="pluginEvent">The plugin event.</param>
	/// <param name="eventArgs">The event arguments provided.</param>
	/// <returns>The result.</returns>
	bool FilterResult(IPluginEvent pluginEvent, AmiableEventArgs eventArgs);
}
