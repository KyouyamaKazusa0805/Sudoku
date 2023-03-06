namespace Mirai.Net.Data.Shared;

/// <summary>
/// Provides with extension methods on <see cref="Group"/>.
/// </summary>
/// <seealso cref="Group"/>
public static class GroupExtensions
{
	/// <summary>
	/// Get all matched <see cref="Member"/>s whose nick names are equal to argument <paramref name="nickname"/>.
	/// </summary>
	/// <param name="this">The group instance.</param>
	/// <param name="nickname">The nickname to be checked.</param>
	/// <returns>A task that handles the operation, with a returning value of type <see cref="Member"/>[]?.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static async Task<Member[]?> GetMatchedMembersViaNicknameAsync(this Group @this, string nickname)
		=> (from m in await @this.GetGroupMembersAsync() where m.Name == nickname select m).ToArray() is var r && r.Length != 0 ? r : null;

	/// <summary>
	/// Get the only matched <see cref="Member"/> whose QQ number is equal to argument <paramref name="id"/>.
	/// </summary>
	/// <param name="this">The group instance.</param>
	/// <param name="id">The user QQ number to be checked.</param>
	/// <returns>
	/// A task that handles the operation, with a returning value of type <see cref="Member"/>. If none found, <see langword="null"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static async Task<Member?> GetMatchedMemberViaIdAsync(this Group @this, string id)
		=> (from m in await @this.GetGroupMembersAsync() where m.Id == id select m).FirstOrDefault();
}
