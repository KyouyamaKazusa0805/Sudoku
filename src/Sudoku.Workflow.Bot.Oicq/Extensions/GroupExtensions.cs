namespace Mirai.Net.Data.Shared;

/// <summary>
/// 为 <see cref="Group"/> 类型的实例提供扩展方法。
/// </summary>
/// <seealso cref="Group"/>
public static class GroupExtensions
{
	/// <summary>
	/// 获取指定群，群成员的昵称等于参数 <paramref name="nickname"/> 的成员。注意，群可能存在多个人名片一样的情况，所以该方法返回的是一个数组，而不是一个元素。
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static async Task<Member[]?> GetMatchedMembersViaNicknameAsync(this Group @this, string nickname)
		=> (from m in await @this.GetGroupMembersAsync() where m.Name == nickname select m).ToArray() is var r && r.Length != 0 ? r : null;

	/// <summary>
	/// 获取指定群，群成员的 QQ 等于参数 <paramref name="id"/> 的成员。
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static async Task<Member?> GetMatchedMemberViaIdAsync(this Group @this, string id)
		=> (from m in await @this.GetGroupMembersAsync() where m.Id == id select m).FirstOrDefault();
}
