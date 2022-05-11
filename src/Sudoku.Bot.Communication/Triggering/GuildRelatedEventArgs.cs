namespace Sudoku.Bot.Communication.Triggering;

/// <summary>
/// Provides with the event data for <see cref="GuildRelatedEventHandler"/>.
/// </summary>
/// <seealso cref="GuildRelatedEventHandler"/>
public sealed class GuildRelatedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="GuildRelatedEventArgs"/> instance via the specified GUILD value
	/// and the event triggered.
	/// </summary>
	/// <param name="guild">The GUILD.</param>
	/// <param name="eventType">The event type.</param>
	public GuildRelatedEventArgs(Guild guild, string eventType) => (Guild, EventType) = (guild, eventType);


	/// <summary>
	/// Indicates a <see cref="string"/> value indicating which event is triggered.
	/// The possible values are <see cref="RawMessageTypes.GuildCreated"/>,
	/// <see cref="RawMessageTypes.GuildUpdated"/> and <see cref="RawMessageTypes.GuildDeleted"/>.
	/// </summary>
	public string EventType { get; }

	/// <summary>
	/// Indicates the GUILD.
	/// </summary>
	public Guild Guild { get; }
}
