#pragma warning disable IDE0060

namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides the common events.
/// </summary>
public static class CommonEvents
{
	/// <summary>
	/// Routes the event with raw data.
	/// </summary>
	public static int XX_Event(
		string robotQQ, int eventType, int extraType, string from, string fromQQ, string targetQQ,
		string content, string index, string msgid, string udpmsg, string unix, long p)
	{
		try
		{
			switch ((CommonEventType)eventType)
			{
				case CommonEventType.Friend:
				{
					return MessageEvents.Event_PrivateMessage(
						DateTime.Now.Ticks, long.Parse(robotQQ), "friend", int.Parse(msgid),
						long.Parse(fromQQ), content, 0, null);
				}
				case CommonEventType.Group:
				{
					return MessageEvents.Event_GroupMessage(
						DateTime.Now.Ticks, long.Parse(robotQQ), "normal", int.Parse(msgid),
						long.Parse(from), long.Parse(fromQQ), content, 0, null);
				}
				case CommonEventType.PluginLoaded:
				{
					PluginEvents.Event_PluginLoad(
						EventCore.GetAmiableEventArgs(DateTime.Now.Ticks, 0, EventType.META_EVENT));

					break;
				}
				case CommonEventType.PluginEnable:
				{
					PluginEvents.Event_PluginEnable(
						EventCore.GetAmiableEventArgs(DateTime.Now.Ticks, 0, EventType.META_EVENT));

					break;
				}
			}
		}
		catch (Exception ex) when (ex is { Source: var source, Message: var message, StackTrace: var stackTrace })
		{
			AmiableService.App.Log($"[XXEvent错误]\n来源:{source}\n问题:{message}\nStack{stackTrace}");
		}

		return 0;
	}
}
