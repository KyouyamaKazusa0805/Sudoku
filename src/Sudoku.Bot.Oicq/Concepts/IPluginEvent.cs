namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines a plugin event.
/// </summary>
public interface IPluginEvent
{
	/// <summary>
	/// Indicates the event type that the Amiable is able to trigger.
	/// </summary>
	AmiableEventType EventType { get; }

	/// <summary>
	/// To process the operation via the event arguments.
	/// </summary>
	/// <param name="e">The event arguments provided.</param>
	void Process(AmiableEventArgs e);
}
