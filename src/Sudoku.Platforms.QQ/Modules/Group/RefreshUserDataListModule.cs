namespace Sudoku.Platforms.QQ.Modules.Group;

#if false
[BuiltIn]
#endif
file sealed class RefreshUserDataListModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "用户数据刷新";

	/// <summary>
	/// Indicates the range.
	/// </summary>
	[DoubleArgumentCommand("范围")]
	public string? Range { get; set; }

	/// <inheritdoc/>
	public override string[] RaisingPrefix => CommonCommandPrefixes.HashTag;

	/// <inheritdoc/>
	public override GroupRoleKind RequiredSenderRole => GroupRoleKind.God;


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		var group = messageReceiver.Sender.Group;
		var groupMembers = await group.GetGroupMembersAsync();

		var users = InternalReadWrite.ReadAll(
			Range switch
			{
				Ranges.AllGroups => static _ => true,
				Ranges.CurrentGroup => userId => groupMembers.Any(e => e.Id == userId),
				_ => static _ => false
			}
		);
		if (users is null or [])
		{
			await messageReceiver.SendMessageAsync("刷新完成。无更新。");
			return;
		}

		foreach (var user in users)
		{
			if (!user.UplevelingCards.TryAdd(user.CardLevel, 1))
			{
				user.UplevelingCards[user.CardLevel]++;
			}

			InternalReadWrite.Write(user);
		}

		await messageReceiver.SendMessageAsync($"刷新完成，已更新{Range}共 {users.Length} 个成员的数据。");
	}
}

file static class Ranges
{
	public const string CurrentGroup = "本群";
	public const string AllGroups = "所有";
}
