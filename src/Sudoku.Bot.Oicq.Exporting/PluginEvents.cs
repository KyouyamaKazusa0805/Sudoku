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
		=> BackingEventHandler.InvokeEvent(AmiableEventType.PluginLoaded, eventArgs);

	/// <summary>
	/// Triggers the event that describes the case that the plugin is enabled.
	/// </summary>
	public static void Event_PluginEnable(AmiableEventArgs eventArgs)
		=> BackingEventHandler.InvokeEvent(AmiableEventType.PluginEnabled, eventArgs);

	/// <summary>
	/// Triggers the event that describes the case that the menu is invoked.
	/// </summary>
	public static int Event_PluginMenu(AmiableEventArgs eventArgs)
	{
		BackingEventHandler.InvokeEvent(AmiableEventType.PluginMenuInvoked, eventArgs);

		return 0;
	}
}
