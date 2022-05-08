namespace Sudoku.Bot.Communication;

/// <summary>
/// Provides with the default configuration on private or public domains.
/// </summary>
public static class Intents
{
	/// <summary>
	/// Indicates the events being able to be used in public domain.
	/// </summary>
	public const Intent PublicDomain
		= Intent.GUILDS | Intent.GUILD_MEMBERS | Intent.GUILD_MESSAGE_REACTIONS
		| Intent.DIRECT_MESSAGE_CREATE | Intent.MESSAGE_AUDIT | Intent.AUDIO_ACTION
		| Intent.AT_MESSAGE_CREATE;

	/// <summary>
	/// Indicates the events being able to be used in private domain.
	/// </summary>
	public const Intent PrivateDomain = PublicDomain | Intent.MESSAGE_CREATE;
}
