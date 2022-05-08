namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the default roles.
/// </summary>
public static class DefaultRoles
{
	/// <summary>
	/// Gets the role description via the role ID, a <see cref="string"/> representation.
	/// </summary>
	/// <param name="roleId">The role ID.</param>
	/// <returns>The string value that describes the role.</returns>
	public static string? Get(string roleId)
		=> roleId switch
		{
			"1" => StringResource.Get("Name_NormalMember"),
			"2" => StringResource.Get("Name_Administrator"),
			"4" => StringResource.Get("Name_GuildOwner"),
			"5" => StringResource.Get("Name_ChannelManager"),
			_ => null
		};
}
