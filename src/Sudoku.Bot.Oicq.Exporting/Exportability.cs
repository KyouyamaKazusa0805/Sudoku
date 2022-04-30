#pragma warning disable CS1591, IDE0060

namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides with a type that is used by expoting to a dynamic linking library,
/// in order to be used and invoked by MyQQ engine.
/// </summary>
/// <remarks><b>
/// Due to the limitation of the exportability, all members in this type marked <see cref="DllExportAttribute"/>
/// cannot be renamed and changed.
/// </b></remarks>
/// <seealso cref="DllExportAttribute"/>
public static class Exportability
{
	[DllExport]
	public static int MQ_End() => 0;

	[DllExport]
	public static int MQ_Message(string SelfId, int Type, string Raw, string Cookies, string SessionKey, string ClientKey)
		=> 0;

	[DllExport]
	public static void MQ_Set()
	{
		AmiableService.ApiKey = "MQ";
		pluginMenuInvoked(new() { Bot = 0, EventType = EventType.MetaEvent, Timestamp = DateTime.Now.Ticks });


		static void pluginMenuInvoked(AmiableEventArgs eventArgs)
			=> BackingEventHandler.InvokeEvent(AmiableEventType.PluginMenuInvoked, eventArgs);
	}

	[DllExport]
	public static string MQ_Info()
	{
		AmiableService.ApiKey = "MQ";
		return BackingEventHandler.InitEvent();
	}

	[DllExport]
	public static int MQ_Event(
		string robotQQ, int eventType, int extraType, string from, string fromQQ, string targetQQ,
		string content, string index, string msgid, string udpmsg, string unix, IntPtr p)
	{
		AmiableService.ApiKey = "MQ";
		try
		{
			return EventRouter.Route(
				robotQQ, eventType, extraType, from, fromQQ, targetQQ, content, index, msgid, udpmsg, unix, p.ToInt64());
		}
		catch (Exception ex)
		{
			AmiableService.App.Log($"Event exception: {ex}");
			return 0;
		}
	}
}
