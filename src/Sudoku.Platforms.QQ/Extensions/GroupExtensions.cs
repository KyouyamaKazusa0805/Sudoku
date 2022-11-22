namespace Mirai.Net.Data.Shared;

/// <summary>
/// Provides with extension methods on <see cref="Group"/>.
/// </summary>
/// <seealso cref="Group"/>
public static class GroupExtensions
{
	/// <summary>
	/// Gets the member whose QQ or name is specified one.
	/// </summary>
	/// <param name="this">The group.</param>
	/// <param name="nameOrId">The QQ number or target member's name.</param>
	/// <returns>The target member returned. If none found, <see langword="null"/>.</returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static async Task<Member?> GetMemberAsync(this Group @this, string nameOrId)
	{
		var result = (Member?)null;
		foreach (var member in await @this.GetGroupMembersAsync())
		{
			if (member.Name == nameOrId || member.Id == nameOrId)
			{
				if (result is null)
				{
					result = member;
				}
				else
				{
					return null;
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Gets the member whose QQ number is the specified one.
	/// </summary>
	/// <param name="this">The group.</param>
	/// <param name="qq">The QQ number.</param>
	/// <returns>The target member returned. If none found, <see langword="null"/>.</returns>
	public static async Task<Member?> GetMemberFromQQAsync(this Group @this, string qq)
	{
		foreach (var member in await @this.GetGroupMembersAsync())
		{
			if (member.Id == qq)
			{
				return member;
			}
		}

		return null;
	}
}
