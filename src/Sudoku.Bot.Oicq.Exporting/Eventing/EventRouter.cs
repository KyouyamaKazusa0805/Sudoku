#pragma warning disable IDE0060

namespace Sudoku.Bot.Oicq.Eventing;

/// <summary>
/// Provides with a router that can route and execute the specified event via the raw data.
/// </summary>
public static class EventRouter
{
	/// <summary>
	/// Routes the event with raw data.
	/// </summary>
	public static int Route(
		string botQQ, int eventType, int extraType, string from, string fromQQ, string targetQQ,
		string content, string index, string messageId, string udpMessage, string unix, long p)
	{
		try
		{
			return (CommonEventType)eventType switch
			{
				CommonEventType.Friend
					=> EventTriggerer.TriggerC2CEvent(
						DateTime.Now.Ticks, long.Parse(botQQ), "friend", int.Parse(messageId),
						long.Parse(fromQQ), content, 0, null),
				CommonEventType.Group
					=> EventTriggerer.TriggerGroupEvent(
						DateTime.Now.Ticks, long.Parse(botQQ), "normal", int.Parse(messageId),
						long.Parse(from), long.Parse(fromQQ), content, 0, null),
				CommonEventType.PluginLoaded
					=> pluginLoaded(BackingEventHandler.GetAmiableEventArgs(DateTime.Now.Ticks, 0, EventType.MetaEvent)),
				CommonEventType.PluginEnabled
					=> pluginEnabled(BackingEventHandler.GetAmiableEventArgs(DateTime.Now.Ticks, 0, EventType.MetaEvent)),
				_ => 0
			};


			static int pluginLoaded(AmiableEventArgs eventArgs)
			{
				BackingEventHandler.InvokeEvent(AmiableEventType.PluginLoaded, eventArgs);
				return 0;
			}

			static int pluginEnabled(AmiableEventArgs eventArgs)
			{
				BackingEventHandler.InvokeEvent(AmiableEventType.PluginEnabled, eventArgs);
				return 0;
			}
		}
		catch (Exception ex) when (ex is { Source: var source, Message: var message, StackTrace: var stackTrace })
		{
			AmiableService.App.Log(
				string.Join(
					"\n",
					new[] { "[Event error]", $"Source: {source}", $"Message: {message}", $"Stack: {stackTrace}" })
			);

			return 0;
		}
	}
}
