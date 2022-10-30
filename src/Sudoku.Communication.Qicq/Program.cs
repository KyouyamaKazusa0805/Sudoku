using static EnvironmentData;

// Add resource router.
R.AddExternalResourceFetecher(typeof(Program).Assembly, Resources.ResourceManager.GetString);

// Creates and initializes a bot.
using var bot = new MiraiBot { Address = R["HostPort"], QQ = R["BotQQ"]!, VerifyKey = R["VerifyKey"] };

try
{
	await bot.LaunchAsync();

	// Registers some necessary events.
	bot.MessageReceived.OfType<GroupMessageReceiver>().Subscribe(onGroupMessageReceiving);
	bot.EventReceived.OfType<MemberJoinedEvent>().Subscribe(onMemberJoined);
	bot.EventReceived.OfType<NewMemberRequestedEvent>().Subscribe(onNewMemberRequested);
	bot.EventReceived.OfType<NewInvitationRequestedEvent>().Subscribe(onInvitationRequested);

	// Blocks the main thread, in order to prevent the main thread exits too fast.
	Terminal.WriteLine(R["BootingSuccessMessage"]!, ConsoleColor.DarkGreen);
	Terminal.Pause();
}
catch (FlurlHttpException)
{
	Terminal.WriteLine(R["BootingFailedDueToMirai"]!, ConsoleColor.DarkRed);
}
catch (InvalidResponseException)
{
	Terminal.WriteLine(R["BootingFailedDueToHttp"]!, ConsoleColor.DarkRed);
}


async void onNewMemberRequested(NewMemberRequestedEvent e)
{
	if (e is not { GroupId: var groupId, Message: var message })
	{
		return;
	}

	if (!isMyGroupId(groupId))
	{
		return;
	}

	if (!await bot.CanHandleInvitationOrJoinRequestAsync(groupId))
	{
		return;
	}

	var bilibiliPattern = R["BilibiliNameRegexPattern"]!;
	if (!message.Trim().IsMatch(bilibiliPattern))
	{
		await e.RejectAsync(R["_MessageFormat_RejectJoiningGroup"]!);
		return;
	}

	await e.ApproveAsync();
}

async void onInvitationRequested(NewInvitationRequestedEvent e)
{
	if (e is not { GroupId: var groupId })
	{
		return;
	}

	if (!isMyGroupId(groupId))
	{
		return;
	}

	if (!await bot.CanHandleInvitationOrJoinRequestAsync(groupId))
	{
		return;
	}

	await e.ApproveAsync();
}

static async void onMemberJoined(MemberJoinedEvent e)
{
	if (e.Member.Group is { Id: var id } group && isMyGroupId(id))
	{
		await group.SendGroupMessageAsync(R["SampleMemberJoinedMessage"]);
	}
}

async void onGroupMessageReceiving(GroupMessageReceiver e)
{
	if (e is not
		{
			Sender:
			{
				Id: var senderId,
				Name: var senderName,
				Permission: var permission,
				MmeberProfile.NickName: var senderOriginalName,
				Group: var group
			} sender,
			MessageChain: var message
		})
	{
		return;
	}

	var random = new Random();
	var plainMessage = message.GetPlainMessage()?.Trim();
	switch (plainMessage)
	{
		case ['!' or '\uff01', .. var slice]: // User commands.
		{
			//
			// Help
			//
			if (isCommand(slice, "_Command_Help") && EnvironmentCommandExecuting is null)
			{
				await e.SendMessageAsync(R["_HelpMessage"]);
				return;
			}

			//
			// Check-in
			//
			if (isCommand(slice, "_Command_CheckIn") && EnvironmentCommandExecuting is null)
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					Directory.CreateDirectory(botUsersDataFolder);
				}

				var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
				var userData = File.Exists(userDataPath)
					? JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!
					: new() { QQ = senderId };

				if (userData.LastCheckIn == DateTime.Today)
				{
					// Disallow user checking in multiple times in a same day.
					await e.SendMessageAsync(R["_MessageFormat_CheckInFailedDueToMultipleInSameDay"]!);
					return;
				}

				if ((DateTime.Today - userData.LastCheckIn).Days == 1)
				{
					// Continuous.
					userData.ComboCheckedIn++;

					var expEarned = generateCheckInExpContinuous(random, userData.ComboCheckedIn);
					userData.Score += expEarned;
					userData.LastCheckIn = DateTime.Today;

					await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessfulAndContinuous"]!, userData.ComboCheckedIn, expEarned));
				}
				else
				{
					// Normal case.
					userData.ComboCheckedIn = 1;

					var expEarned = generateCheckInExp(random);
					userData.Score += expEarned;
					userData.LastCheckIn = DateTime.Today;

					await e.SendMessageAsync(string.Format(R["_MessageFormat_CheckInSuccessful"]!, expEarned));
				}

				var json = JsonSerializer.Serialize(userData);
				await File.WriteAllTextAsync(userDataPath, json);

				return;
			}

			//
			// Check-in manual
			//
			if (isCommand(slice, "_Command_CheckInIntro") && EnvironmentCommandExecuting is null)
			{
				await e.SendMessageAsync(R["_MessageFormat_CheckInIntro"]);
				return;
			}

			//
			// Lookup score
			//
			if (isCommand(slice, "_Command_LookupScore") && EnvironmentCommandExecuting is null)
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					goto DirectlyReturn;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var userDataPath = $"""{botUsersDataFolder}\{senderId}.json""";
				if (!File.Exists(userDataPath))
				{
					goto SpecialCase_UserDataFileNotFound;
				}

				var userData = JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));

				goto DirectlyReturn;

			SpecialCase_UserDataFileNotFound:
				await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));

			DirectlyReturn:
				return;
			}

			//
			// Draw
			//
			if (isCommand(slice, "_Command_Draw") && EnvironmentCommandExecuting is null)
			{
				await e.SendMessageAsync(R["_MessageFormat_DrawStartMessage"]!);

				EnvironmentCommandExecuting = R["_Command_Draw"]!;
				Puzzle = Grid.Empty;
				Painter = ISudokuPainter.Create(800, 10).WithGrid(Puzzle).WithRenderingCandidates(false);
				DrawNodes = new List<ViewNode>();

				return;
			}

			//
			// Draw (Subprocedure)
			//
			if (isCommandStart(slice, "_Command_DrawSub", out var drawSubArgument) && EnvironmentCommandExecuting == R["_Command_Draw"])
			{
				var split = drawSubArgument.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				if (split is not [var rawCoordinate, var rawIdentifier])
				{
					return;
				}

				if (getIdentifier(rawIdentifier) is not { } identifier)
				{
					return;
				}

				if (getCoordinate(rawCoordinate) is not { } triplet)
				{
					return;
				}

				switch (triplet)
				{
					case ({ } cells, _, _):
					{
						cells.ForEach(cell => DrawNodes!.Add(new CellViewNode(identifier, cell)));
						break;
					}
					case (_, { } candidates, _):
					{
						candidates.ForEach(candidate => DrawNodes!.Add(new CandidateViewNode(identifier, candidate)));
						break;
					}
					case (_, _, { } house):
					{
						DrawNodes!.Add(new HouseViewNode(identifier, house));
						break;
					}
				}

				Painter!.WithNodes(DrawNodes!.ToArray());

				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					Directory.CreateDirectory(botUsersDataFolder);
				}

				var picturePath = $"""{botUsersDataFolder}\temp.png""";
				Painter.SaveTo(picturePath);

				await e.SendMessageAsync(new ImageMessage { Path = picturePath });

				File.Delete(picturePath);

				return;
			}

			//
			// Clear
			//
			if (isCommandStart(slice, "_Command_Clear", out var clearSubArgument) && EnvironmentCommandExecuting == R["_Command_Draw"])
			{
				if (getCoordinate(clearSubArgument) is not { } triplet)
				{
					return;
				}

				switch (triplet)
				{
					case ({ } cells, _, _):
					{
						cells.ForEach(cell => DrawNodes!.RemoveAll(r => r is CellViewNode { Cell: var c } && c == cell));
						break;
					}
					case (_, { } candidates, _):
					{
						candidates.ForEach(candidate => DrawNodes!.RemoveAll(r => r is CandidateViewNode { Candidate: var c } && c == candidate));
						break;
					}
					case (_, _, { } house):
					{
						DrawNodes!.RemoveAll(r => r is HouseViewNode { House: var h } && h == house);
						break;
					}
				}

				Painter!.WithNodes(DrawNodes!.ToArray());

				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					Directory.CreateDirectory(botUsersDataFolder);
				}

				var picturePath = $"""{botUsersDataFolder}\temp.png""";
				Painter.SaveTo(picturePath);

				await e.SendMessageAsync(new ImageMessage { Path = picturePath });

				File.Delete(picturePath);

				return;
			}

			//
			// Set
			//
			if (isCommandStart(slice, "_Command_Set", out var setArgument) && EnvironmentCommandExecuting == R["_Command_Draw"])
			{
				var split = setArgument.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				if (split is not [var rawCoordinate, [var rawDigit and >= '0' and <= '9']])
				{
					return;
				}

				if (getCell(rawCoordinate) is not { } cell)
				{
					return;
				}

				Puzzle[cell] = rawDigit - '1';
				Painter!.WithGrid(Puzzle);

				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					Directory.CreateDirectory(botUsersDataFolder);
				}

				var picturePath = $"""{botUsersDataFolder}\temp.png""";
				Painter.SaveTo(picturePath);

				await e.SendMessageAsync(new ImageMessage { Path = picturePath });

				File.Delete(picturePath);

				return;
			}

			//
			// Delete
			//
			if (isCommandStart(slice, "_Command_Delete", out var deleteArgument) && EnvironmentCommandExecuting == R["_Command_Draw"])
			{
				var split = deleteArgument.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				if (split is not [var rawCoordinate, [var rawDigit and >= '0' and <= '9']])
				{
					return;
				}

				if (getCell(rawCoordinate) is not { } cell)
				{
					return;
				}

				Puzzle[cell, rawDigit - '1'] = false;
				Painter!.WithGrid(Puzzle);

				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					Directory.CreateDirectory(botDataFolder);
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["CachedPictureFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					Directory.CreateDirectory(botUsersDataFolder);
				}

				var picturePath = $"""{botUsersDataFolder}\temp.png""";
				Painter.SaveTo(picturePath);

				await e.SendMessageAsync(new ImageMessage { Path = picturePath });

				File.Delete(picturePath);

				return;
			}

			//
			// End
			//
			if (isCommand(slice, "_Command_End") && EnvironmentCommandExecuting is not null)
			{
				EnvironmentCommandExecuting = null;
				DrawNodes = null;
				Painter = null;
				Puzzle = Grid.Empty;

				await e.SendMessageAsync(R["_MessageFormat_EndOkay"]!);
				return;
			}

			break;
		}
		case ['%' or '\uff05', .. var slice]: // Manager commands.
		{
			if (!isMe(sender) && permission is not (Permissions.Owner or Permissions.Administrator))
			{
				await e.SendMessageAsync(R["_MessageFormat_PermissionRequired"]!);
				return;
			}

			//
			// Lookup score
			//
			if (isComplexCommand(slice, "_Command_ComplexLookupScore", out var lookupArguments) && EnvironmentCommandExecuting is null)
			{
				if (lookupArguments is not [var nameOrId])
				{
					return;
				}

				var satisfiedMembers = (
					from member in await @group.GetGroupMembersAsync()
					where member.Id == nameOrId || member.Name == nameOrId
					select member
				).ToArray();
				switch (satisfiedMembers)
				{
					case []:
					{
						await e.SendMessageAsync(R["_MessageFormat_LookupNameOrIdInvalid"]);
						break;
					}
					case { Length: >= 2 }:
					{
						await e.SendMessageAsync(R["_MessageFormat_LookupNameOrIdAmbiguous"]);
						break;
					}
					case [{ Id: var foundMemberId }]:
					{
						var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
						if (!Directory.Exists(folder))
						{
							// Error. The computer does not contain "My Documents" folder.
							// This folder is special; if the computer does not contain the folder, we should return directly.
							goto DirectlyReturn;
						}

						var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
						if (!Directory.Exists(botDataFolder))
						{
							goto SpecialCase_UserDataFileNotFound;
						}

						var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
						if (!Directory.Exists(botUsersDataFolder))
						{
							goto SpecialCase_UserDataFileNotFound;
						}

						var userDataPath = $"""{botUsersDataFolder}\{foundMemberId}.json""";
						if (!File.Exists(userDataPath))
						{
							goto SpecialCase_UserDataFileNotFound;
						}

						var userData = JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(userDataPath))!;
						await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreIs"]!, senderName, userData.Score, senderOriginalName));

						goto DirectlyReturn;

					SpecialCase_UserDataFileNotFound:
						await e.SendMessageAsync(string.Format(R["_MessageFormat_UserScoreNotFound"]!, senderName, senderOriginalName));

					DirectlyReturn:
						return;
					}
				}

				return;
			}

			//
			// Ranking
			//
			if (isCommandStart(slice, "_Command_Ranking", out var rankingArguments) && EnvironmentCommandExecuting is null)
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					await e.SendMessageAsync(R["_MessageFormat_RankingListIsEmpty"]!);
					return;
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					await e.SendMessageAsync(R["_MessageFormat_RankingListIsEmpty"]!);
					return;
				}

				var usersData = (
					from file in Directory.GetFiles(botUsersDataFolder, "*.json")
					let ud = JsonSerializer.Deserialize<UserData>(File.ReadAllText(file))
					where ud is not null
					let qq = ud.QQ
					let nickname = g(qq, @group).Result?.Name
					where nickname is not null
					orderby ud.Score descending, qq
					select (Name: nickname, Data: ud)
				).Take(10); // If the number of members are too large, we should only iterate top 10 elements.

				var rankingStr = string.Join(
					"\r\n",
					usersData.Select(
						static (pair, i) =>
						{
							if (pair is not (var name, { QQ: var qq, Score: var score }))
							{
								throw new();
							}

							var openBrace = R["_Token_OpenBrace"]!;
							var closedBrace = R["_Token_ClosedBrace"]!;
							var colon = R["_Token_Colon"]!;
							var comma = R["_Token_Comma"]!;
							var scoreSuffix = R["ExpString"]!;
							return $"#{i + 1}{colon}{name}{openBrace}{qq}{closedBrace}{comma}{score} {scoreSuffix}";
						}
					)
				);

				var result =
					$"""
					{R["_MessageFormat_RankingResult"]}
					---
					{rankingStr}
					""";

				await e.SendMessageAsync(result);
				return;
			}

			break;
		}
		case [':' or '\uff1a', .. var slice]: // Admin commands.
		{
			if (!isMe(sender))
			{
				await e.SendMessageAsync(R["_MessageFormat_PermissionRequired2"]!);
				return;
			}

			if (isCommandStart(slice, "_Command_UpdateScore", out var updateScoreArguments) && EnvironmentCommandExecuting is null)
			{
				var folder = Environment.GetFolderPath(SpecialFolder.MyDocuments);
				if (!Directory.Exists(folder))
				{
					// Error. The computer does not contain "My Documents" folder.
					// This folder is special; if the computer does not contain the folder, we should return directly.
					return;
				}

				var botDataFolder = $"""{folder}\{R["BotSettingsFolderName"]}""";
				if (!Directory.Exists(botDataFolder))
				{
					await e.SendMessageAsync(R["_MessageFormat_RankingListIsEmpty"]!);
					return;
				}

				var botUsersDataFolder = $"""{botDataFolder}\{R["UserSettingsFolderName"]}""";
				if (!Directory.Exists(botUsersDataFolder))
				{
					await e.SendMessageAsync(R["_MessageFormat_RankingListIsEmpty"]!);
					return;
				}

				var split = updateScoreArguments.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				if (split is not [var nameOrId, var scoreAppending])
				{
					return;
				}

				if (!int.TryParse(scoreAppending, out var value))
				{
					return;
				}

				if (await g2(nameOrId, group) is not { Id: var targetId, Name: var targetName } target)
				{
					return;
				}

				var fileName = $"""{botUsersDataFolder}\{targetId}.json""";
				var userData = File.Exists(fileName)
					? JsonSerializer.Deserialize<UserData>(await File.ReadAllTextAsync(fileName))!
					: new() { QQ = targetId, ComboCheckedIn = 0, LastCheckIn = DateTime.MinValue, Score = 0 };

				userData.Score += value;

				await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(userData));

				await e.SendMessageAsync(string.Format(R["_MessageFormat_ScoreAppending"]!, targetName, value));
				return;
			}

			break;
		}
		default: // Other unrecognized commands, or higher-permission commands visiting.
		{
			return;
		}


		static async Task<Member?> g(string qq, Group group)
			=> (from member in await @group.GetGroupMembersAsync() where member.Id == qq select member).FirstOrDefault();

		static async Task<Member?> g2(string nameOrId, Group group)
			=> (
				from member in await @group.GetGroupMembersAsync()
				where member.Id == nameOrId || member.Name == nameOrId
				select member
			).FirstOrDefault();
	}
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isMyGroupId(string s) => s == R["SudokuGroupQQ"];

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isMe(Member member) => member.Id == R["AdminQQ"];

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isCommand([NotNullWhen(true)] string? slice, string commandKey) => slice == R[commandKey];

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isCommandStart([NotNullWhen(true)] string? slice, string commandKey, [NotNullWhen(true)] out string? stringArgument)
{
	var realCommand = R[commandKey]!;
	if (!(slice?.StartsWith(realCommand) ?? false))
	{
		stringArgument = null;
		return false;
	}

	stringArgument = slice[realCommand.Length..].Trim();
	return true;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static bool isComplexCommand([NotNullWhen(true)] string? slice, string commandKey, [NotNullWhen(true)] out string[]? arguments)
{
	if (slice is null)
	{
		goto InvalidReturn;
	}

	if (!slice.Contains(',') && !slice.Contains('\uff0c'))
	{
		goto InvalidReturn;
	}

	var baseArguments = slice.Split(new[] { ',', '\uff0c' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	if (!Array.TrueForAll(baseArguments, static a => a.Length != 0))
	{
		goto InvalidReturn;
	}

	if (baseArguments is not [var commandName, .. var otherArguments])
	{
		goto InvalidReturn;
	}

	if (R[commandKey] != commandName)
	{
		goto InvalidReturn;
	}

	arguments = otherArguments;
	return true;

InvalidReturn:
	arguments = null;
	return false;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static int generateCheckInExp(Random random)
	=> random.Next(0, 1000) switch
	{
		< 400 => 1, // 40%
		>= 400 and < 700 => 2, // 30%
		>= 700 and < 900 => 3, // 20%
		_ => 4 // 10%
	};

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static int generateCheckInExpContinuous(Random random, int continuousDaysCount)
{
	var earned = generateCheckInExp(random);
	var level = continuousDaysCount / 7;
	return (int)Round(earned * (level * .2 + 1));
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static Identifier? getIdentifier(string name)
{
	if (Enum.TryParse<KnownColor>(name, out var knownColor))
	{
		return f(Color.FromKnownColor(knownColor));
	}

	if (KnownColors.TryGetValue(name, out var dicColor))
	{
		return f(dicColor);
	}

	if (name is ['%', .. var rawColorKind] && KnownKinds.TryGetValue(rawColorKind, out var colorKind))
	{
		return Identifier.FromNamedKind(colorKind);
	}

	if (name.Match("""#([1-9]|1[0-5])""") is [_, .. var rawId] colorLabel)
	{
		return Identifier.FromId(int.Parse(rawId));
	}

	if (name.Match("""#[\dA-Fa-f]{6}([\dA-Fa-f]{2})?""") is { } colorHtml)
	{
		return f(ColorTranslator.FromHtml(colorHtml));
	}

	return null;


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static Identifier f(Color c) => Identifier.FromColor(c.A, c.R, c.G, c.B);
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static int? getCell(string rawCoordinate)
{
	if (RxCyNotation.TryParseCell(rawCoordinate, out var cell2))
	{
		return cell2;
	}

	if (K9Notation.TryParseCell(rawCoordinate, out var cell1))
	{
		return cell1;
	}

	return null;
}

[MethodImpl(MethodImplOptions.AggressiveInlining)]
static (CellMap? Cell, Candidates? Candidate, int? House)? getCoordinate(string rawCoordinate)
{
	if (RxCyNotation.TryParseCandidates(rawCoordinate, out var candidates1))
	{
		return (null, candidates1, null);
	}

	if (RxCyNotation.TryParseCells(rawCoordinate, out var cells2))
	{
		return (cells2, null, null);
	}

	if (K9Notation.TryParseCells(rawCoordinate, out var cells1))
	{
		return (cells1, null, null);
	}

#pragma warning disable IDE0055
	if (rawCoordinate.Match("""[\u884c\u5217\u5bab]\s*[1-9]""") is { } parts
		&& parts.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) is [[var houseNotation], [var label]])
#pragma warning restore IDE0055
	{
		return (null, null, houseNotation switch { '\u884c' => 9, '\u5217' => 18, _ => 0 } + (label - '1'));
	}

	return null;
}

file static partial class Program
{
	private static readonly Dictionary<string, Color> KnownColors = new()
	{
		{ R["ColorRed"]!, Color.Red },
		{ R["ColorGreen"]!, Color.Green },
		{ R["ColorBlue"]!, Color.Blue },
		{ R["ColorYellow"]!, Color.Yellow },
		{ R["ColorBlack"]!, Color.Black },
		{ R["ColorPurple"]!, Color.Purple },
		{ R["ColorSkyblue"]!, Color.SkyBlue },
		{ R["ColorDarkYellow"]!, Color.Gold },
		{ R["ColorDarkGreen"]!, Color.DarkGreen },
		{ R["ColorPink"]!, Color.Pink },
		{ R["ColorOrange1"]!, Color.Orange },
		{ R["ColorOrange2"]!, Color.Orange },
		{ R["ColorGray"]!, Color.Gray }
	};

	private static readonly Dictionary<string, DisplayColorKind> KnownKinds = new()
	{
		{ R["ColorKind_Normal"]!, DisplayColorKind.Normal },
		{ R["ColorKind_Aux1"]!, DisplayColorKind.Auxiliary1 },
		{ R["ColorKind_Aux2"]!, DisplayColorKind.Auxiliary2 },
		{ R["ColorKind_Aux3"]!, DisplayColorKind.Auxiliary3 },
		{ R["ColorKind_Assignment"]!, DisplayColorKind.Assignment },
		{ R["ColorKind_Elimination"]!, DisplayColorKind.Elimination },
		{ R["ColorKind_Exofin"]!, DisplayColorKind.Exofin },
		{ R["ColorKind_Endofin"]!, DisplayColorKind.Endofin },
		{ R["ColorKind_Cannibalism"]!, DisplayColorKind.Cannibalism },
		{ R["ColorKind_Als1"]!, DisplayColorKind.AlmostLockedSet1 },
		{ R["ColorKind_Als2"]!, DisplayColorKind.AlmostLockedSet2 },
		{ R["ColorKind_Als3"]!, DisplayColorKind.AlmostLockedSet3 },
		{ R["ColorKind_Als4"]!, DisplayColorKind.AlmostLockedSet4 },
		{ R["ColorKind_Als5"]!, DisplayColorKind.AlmostLockedSet5 }
	};
}

file static class EnvironmentData
{
	public static string? EnvironmentCommandExecuting = null;
	public static Grid Puzzle = Grid.Empty;
	public static List<ViewNode>? DrawNodes = null;
	public static ISudokuPainter? Painter = null;
}
