#pragma warning disable IDE0060

namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// The core type to handle the events.
/// </summary>
public static class BackingEventHandler
{
	[DllExport]
	public static int Test() => 0;

	/// <summary>
	/// Indicates the pre-loading event.
	/// </summary>
	/// <param name="args">The arguments.</param>
	public static void PreInitEvent(params object[] args)
	{
		try
		{
			AmiableService.App.SetApiKey(AmiableService.ApiKey);
			AmiableService.App.DefaultApiWrapper.Init(args);
		}
		catch (Exception ex)
		{
			AmiableService.App.Log(ex.ToString());
		}
	}

	/// <summary>
	/// Indicates the initialization event.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <returns>The app info string.</returns>
	public static string InitEvent(params object[] args)
	{
		try
		{
			AmiableService.App.SetApiKey(AmiableService.ApiKey);
			return AmiableService.App.GetAppInfoSring();
		}
		catch (Exception ex)
		{
			AmiableService.App.Log(ex.ToString());
			return string.Empty;
		}
	}

	/// <summary>
	/// Indicates the after-initialized event.
	/// </summary>
	/// <param name="args">The arguments.</param>
	public static void AfterInitEvent(params object[] args) => AmiableService.App.SetApiKey(AmiableService.ApiKey);

	/// <summary>
	/// Invokes an event.
	/// </summary>
	/// <param name="type">The event type to trigger.</param>
	/// <param name="e">The event arguments provided.</param>
	public static void InvokeEvent(AmiableEventType type, AmiableEventArgs e)
	{
		try
		{
			AmiableService.App.SetApiKey(AmiableService.ApiKey);
			var apiWrapper = (IApiWrapper)AmiableService.App.DefaultApiWrapper.Clone();

			apiWrapper.SetData(e);

			e.ApiWrapper = apiWrapper;
			e.AppInfo = AmiableService.App.AppInfo;

			AmiableService.EventHandlers.FindAll(x => x.EventType == type).ForEach(x => x.Process(e));
		}
		catch (Exception ex) when (ex is { Source: var source, Message: var message, StackTrace: var stackTrace })
		{
			AmiableService.App.Log(
				string.Join(
					"\n",
					new[] { "[Event error]", $"Source: {source}", $"Message: {message}", $"Stack: {stackTrace}" })
			);
		}
	}

	/// <summary>
	/// Gets an Amiable event arguments instance.
	/// </summary>
	/// <param name="timestamp">The timestamp.</param>
	/// <param name="bot">The QQ of the bot.</param>
	/// <param name="eventType">The event type.</param>
	/// <returns>The Amiable event arguments instance.</returns>
	public static AmiableEventArgs GetAmiableEventArgs(long timestamp, long bot, EventType eventType)
		=> new()
		{
			Timestamp = timestamp,
			Bot = bot,
			EventType = eventType
		};
}
