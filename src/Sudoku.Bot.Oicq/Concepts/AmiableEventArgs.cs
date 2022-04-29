namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Defines the event arguments for the Amiable (the SDK provided for this project).
/// </summary>
public class AmiableEventArgs : EventArgs
{
	/// <summary>
	/// Defines a wrapper that can call the backing APIs.
	/// </summary>
	public IApiWrapper? ApiWrapper;

	/// <summary>
	/// Indicates the raw data.
	/// </summary>
	[Obsolete("The property is deprecated. Please use other properties or fields instead.", false)]
	public EventRawData? RawData;

	/// <summary>
	/// The app info.
	/// </summary>
	public AppInfo AppInfo;


	/// <summary>
	/// Indicates the time stamp.
	/// </summary>
	[JsonPropertyName("time")]
	public long Timestamp { get; set; }

	/// <summary>
	/// Indicates the QQ number that points to the bot triggering the event.
	/// </summary>
	[JsonPropertyName("self_id")]
	public long Robot { get; set; }

	/// <summary>
	/// Indicates the event type. The property is referenced from <see href="https://onebot.dev/">Onebot</see> protocol.
	/// </summary>
	[JsonPropertyName("post_type")]
	[JsonConverter(typeof(EnumConverter<EventType>))]
	public EventType EventType { get; set; }

	/// <summary>
	/// Indicates the result of the handling.
	/// </summary>
	public EventHandleResult HandleResult { get; set; } = EventHandleResult.NEGLECT;

	/// <summary>
	/// Indicates the local configuration path of the app.
	/// </summary>
	public string AppDirectory
		=> ApiWrapper?.GetAppDirectory(AppInfo.AppId)
			?? throw new NullReferenceException($"The property '{nameof(ApiWrapper)}' is null.");
}
