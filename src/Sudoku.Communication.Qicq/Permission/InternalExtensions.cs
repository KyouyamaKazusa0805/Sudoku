namespace Sudoku.Communication.Qicq.Permission;

/// <summary>
/// Provides with internal extension methods.
/// </summary>
internal static class InternalExtensions
{
	/// <summary>
	/// Determines whether the specified <see cref="MiraiBot"/> instance supports permission on handling requests
	/// of adding or inviting message about the specified group.
	/// </summary>
	/// <param name="this">The <see cref="MiraiBot"/> instance.</param>
	/// <param name="groupId">The group ID.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static async Task<bool> CanHandleInvitationOrJoinRequestAsync(this MiraiBot @this, string groupId)
		=> @this is { Groups: { IsValueCreated: true, Value: var groups }, QQ: var botId }
		&& (await groups.First(g => g.Id == groupId).GetGroupMembersAsync()).FirstOrDefault(m => m.Id == botId) is var foundMember
		&& foundMember is { Permission: Permissions.Administrator or Permissions.Owner };
}
