#pragma warning disable IDE0060

namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides the type that can export the methods.
/// </summary>
public static class MqExport
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
		PluginEvents.Event_PluginMenu(new() { Bot = 0, EventType = EventType.META_EVENT, Timestamp = DateTime.Now.Ticks });
	}

	[DllExport]
	public static string MQ_Info()
	{
		AmiableService.ApiKey = "MQ";
		return EventCore.InitEvent();
	}

	[DllExport]
	public static int MQ_Event(
		string robotQQ, int eventType, int extraType, string from, string fromQQ, string targetQQ,
		string content, string index, string msgid, string udpmsg, string unix, IntPtr p)
	{
		AmiableService.ApiKey = "MQ";
		try
		{
			return CommonEvents.XX_Event(
				robotQQ, eventType, extraType, from, fromQQ, targetQQ, content, index, msgid, udpmsg, unix, p.ToInt64());
		}
		catch (Exception ex)
		{
			AmiableService.App.Log($"Event异常:{ex}");
			return 0;
		}
	}
}
