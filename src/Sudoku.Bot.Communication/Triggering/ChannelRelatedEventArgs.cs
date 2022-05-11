namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="ChannelRelatedEventHandler"/>.
/// </summary>
/// <seealso cref="ChannelRelatedEventHandler"/>
public sealed class ChannelRelatedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="ChannelRelatedEventArgs"/> instance via the specified channel,
	/// and the specified event type.
	/// </summary>
	/// <param name="channel">The channel.</param>
	/// <param name="eventType">The event type.</param>
	public ChannelRelatedEventArgs(Channel channel, string eventType) => (Channel, EventType) = (channel, eventType);


	/// <summary>
	/// Indiactes which event is triggered.
	/// The possible values are <see cref="RawMessageTypes.ChannelCreated"/>,
	/// <see cref="RawMessageTypes.ChannelUpdated"/> and <see cref="RawMessageTypes.ChannelDeleted"/>.
	/// </summary>
	public string EventType { get; }

	/// <summary>
	/// Indicates the channel.
	/// </summary>
	public Channel Channel { get; }
}