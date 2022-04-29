namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides with the plugin events triggerer.
/// </summary>
public static class PluginEvents
{
	/// <summary>
	/// Triggers the plugin load event.
	/// </summary>
	public static void Event_PluginLoad(AmiableEventArgs eventArgs)
		=> EventCore.InvokeEvents(AmiableEventType.PluginLoaded, eventArgs);

	/// <summary>
	/// Triggers the event that describes the case that the plugin is enabled.
	/// </summary>
	public static void Event_PluginEnable(AmiableEventArgs eventArgs)
		=> EventCore.InvokeEvents(AmiableEventType.PluginEnable, eventArgs);

	/// <summary>
	/// Triggers the event that describes the case that the menu is invoked.
	/// </summary>
	public static int Event_PluginMenu(AmiableEventArgs eventArgs)
	{
		EventCore.InvokeEvents(AmiableEventType.PluginMenu, eventArgs);

		return 0;
	}
}
