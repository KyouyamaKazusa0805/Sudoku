namespace Sudoku.Platforms.QQ.Modules;

[BuiltInModule]
file sealed class UpdateSourceModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "加分";

	/// <inheritdoc/>
	public override GroupRoleKind SupportedRoles => GroupRoleKind.GodAccount;

	/// <summary>
	/// Indicates the user name.
	/// </summary>
	[DoubleArgument("用户名")]
	public string? UserName { get; set; }

	/// <summary>
	/// Indicates the user ID.
	/// </summary>
	[DoubleArgument("QQ")]
	public string? UserId { get; set; }

	/// <summary>
	/// Indicates the addition of the score.
	/// </summary>
	[DoubleArgument("分数")]
	[ArgumentValueConverter<Int32Converter>]
	public int Addition { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver groupMessageReceiver)
	{
		if (groupMessageReceiver is not { Sender.Group: var group })
		{
			return;
		}

		var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
		if (!Directory.Exists(folder))
		{
			// Error. The computer does not contain "My Documents" folder.
			// This folder is special; if the computer does not contain the folder, we should return directly.
			return;
		}

		var botDataFolder = $"""{folder}\BotData""";
		if (!Directory.Exists(botDataFolder))
		{
			Directory.CreateDirectory(botDataFolder);
		}

		var botUsersDataFolder = $"""{botDataFolder}\Users""";
		if (!Directory.Exists(botUsersDataFolder))
		{
			Directory.CreateDirectory(botUsersDataFolder);
		}

		switch (UserName, UserId, Addition)
		{
			case ({ } name, null, var addition):
			{
				switch (await getMatchedNicknameMembers(group, name))
				{
					case null or []:
					{
						await groupMessageReceiver.SendMessageAsync($"本群不存在昵称为“{name}”的用户。");
						break;
					}
					case [{ Id: var userId } target]:
					{
						var fileName = $"""{botUsersDataFolder}\{userId}.json""";
						var userData = File.Exists(fileName)
							? Deserialize<UserData>(await File.ReadAllTextAsync(fileName))!
							: new() { QQ = userId, ComboCheckedIn = 0, LastCheckIn = DateTime.MinValue, Score = 0 };

						userData.Score += addition;

						await File.WriteAllTextAsync(fileName, Serialize(userData));
						await groupMessageReceiver.SendMessageAsync($"恭喜用户“{name}”获得 {Scorer.GetEarnedScoringDisplayingString(addition)} 积分！");

						break;
					}
					default:
					{
						await groupMessageReceiver.SendMessageAsync($"本群至少存在两个用户昵称为“{name}”。请改用 QQ 号来确保用户的唯一性");
						break;
					}
				}

				break;
			}
			case (null, { } userId, var addition):
			{
				switch (await getMatchedIdMembers(group, userId))
				{
					case { Name: var name }:
					{
						var fileName = $"""{botUsersDataFolder}\{userId}.json""";
						var userData = File.Exists(fileName)
							? Deserialize<UserData>(await File.ReadAllTextAsync(fileName))!
							: new() { QQ = userId, ComboCheckedIn = 0, LastCheckIn = DateTime.MinValue, Score = 0 };

						userData.Score += addition;

						await File.WriteAllTextAsync(fileName, Serialize(userData));
						await groupMessageReceiver.SendMessageAsync($"恭喜用户“{name}”获得 {Scorer.GetEarnedScoringDisplayingString(addition)} 积分！");

						break;
					}
					default:
					{
						await groupMessageReceiver.SendMessageAsync($"本群不存在 QQ 为“{userId}”的用户。");
						break;
					}
				}

				break;
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static async Task<Member[]> getMatchedNicknameMembers(Group group, string nickname)
			=> (from m in await @group.GetGroupMembersAsync() where m.Name == nickname select m).ToArray();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static async Task<Member?> getMatchedIdMembers(Group group, string id)
			=> (from m in await @group.GetGroupMembersAsync() where m.Id == id select m).FirstOrDefault();
	}
}
