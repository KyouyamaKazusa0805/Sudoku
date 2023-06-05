namespace Sudoku.Workflow.Bot.Oicq.RootCommands;

/// <summary>
/// 排名指令。
/// </summary>
[Command("排名")]
[RequiredRole(SenderRole = GroupRoleKind.Manager | GroupRoleKind.Owner | GroupRoleKind.God)]
internal sealed class RankCommand : Command
{
	/// <summary>
	/// 表示你要查看的排名的内容类型。可以查看“基本”、“经验值”、“金币”、“魔塔”、“强化”和“签到”。默认为“基本”，即查看基本的排名数据。
	/// </summary>
	[DoubleArgument("类型")]
	[Hint("表示你要查看的排名的内容类型。可以查看“基本”、“经验值”、“金币”、“魔塔”、“强化”、“签到”和“对抗”。默认为“基本”，即查看基本的排名数据。")]
	[DefaultValue<string>("基本")]
	[DisplayingIndex(0)]
	[ArgumentDisplayer("排序依据")]
	public string Type { get; set; } = null!;

	/// <summary>
	/// 表示参与排名的所有人里的前多少名会被显示出来。默认为 10。如果数字过大，该数值则无效。最大为 20。
	/// </summary>
	[DoubleArgument("人数")]
	[Hint("表示参与排名的所有人里的前多少名会被显示出来。默认为 10。如果数字过大，该数值则无效。最大为 20。")]
	[DefaultValue<int>(10)]
	[ValueConverter<NumericConverter<int>>]
	[DisplayingIndex(1)]
	[ArgumentDisplayer("5-25")]
	public int TopCount { get; set; }

	/// <summary>
	/// 表示你要排名的游戏模式。默认为 <see cref="GameMode.FindDifference"/>。
	/// </summary>
	[DoubleArgument("模式")]
	[Hint($"表示你要排名的游戏模式。默认为“九数找相同”。")]
	[DefaultValue<GameMode>(GameMode.FindDifference)]
	[ValueConverter<GameModeConverter>]
	[DisplayingIndex(2)]
	public GameMode GameMode { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { Sender.Group: var group })
		{
			return;
		}

		var finalTopCount = Clamp(TopCount, 5, 25);
		switch (Type)
		{
			case Types.Basic:
			{
				var usersData = (await ScoringOperation.GetUserRankingListAsync(group, rankingListIsEmptyCallback))!.Take(finalTopCount);
				await messageReceiver.SendMessageAsync(
					$"""
					用户基本数据排名：
					{string.Join(
						Environment.NewLine,
						usersData.Select(
							static (pair, i) =>
							{
								var name = pair.Name;
								var qq = pair.Data.Number;
								var score = pair.Data.ExperiencePoint;
								var tower = pair.Data.TowerOfSorcerer;
								var grade = ScoringOperation.GetGrade(score);
								return $"#{i + 1,2} {name} 🚩{score} 📈{tower} 🏅{grade}";
							}
						)
					)}
					---
					排名最多仅列举本群前 {finalTopCount} 名的成绩；想要精确查看用户名次请使用“查询”指令。
					"""
				);

				break;
			}
			case Types.PkResult:
			{
				if (!Enum.IsDefined(GameMode))
				{
					await messageReceiver.SendMessageAsync("输入的“模式”参数不合法。");
					break;
				}

				var usersData = await getDataAsync(
					ud => new PlayingDataTuple(
						ud.TotalPlayingCount.TryGetValue(GameMode, out var r) ? r : 0,
						ud.CorrectedCount.TryGetValue(GameMode, out r) ? r : 0,
						ud.TriedCount.TryGetValue(GameMode, out r) ? r : 0,
						GameMode
					)
				);
				await messageReceiver.SendMessageAsync(
					$"""
					用户基本数据排名：
					{string.Join(
						Environment.NewLine,
						usersData.Select(
							(pair, i) =>
							{
								var name = pair.Name;
								var corrected = pair.Data.Corrected;
								var total = pair.Data.Total;
								return $"#{i + 1,2} {name} - {corrected}/{total} 局（{corrected / (double)total:P2}）";
							}
						)
					)}
					---
					排名最多仅列举本群前 {finalTopCount} 名的成绩；想要精确查看用户名次请使用“查询”指令。
					"""
				);

				break;
			}
			case var type and (Types.ExperiencePoint or Types.Coin or Types.Grade or Types.Tower or Types.ContinuousCheckIn or Types.CardLevel):
			{
				var usersData = await getDataAsync(
					ud => type switch
					{
						Types.ExperiencePoint => ud.ExperiencePoint,
						Types.Coin => ud.Coin,
						Types.CardLevel => ud.CardLevel,
						Types.Grade => ScoringOperation.GetGrade(ud.ExperiencePoint),
						Types.Tower => ud.TowerOfSorcerer,
						Types.ContinuousCheckIn => ud.ComboCheckedIn
					}
				);

				await messageReceiver.SendMessageAsync(
					$"""
					用户{Type}排名：
					{string.Join(
						Environment.NewLine,
						usersData.Select(
							(pair, i) =>
							{
								var (name, data) = pair;
								var unit = Type switch
								{
									Types.ExperiencePoint => "经验值",
									Types.Coin => "金币",
									Types.Grade => "级",
									Types.Tower => "层",
									Types.ContinuousCheckIn => $"天（× {ScoringOperation.GetCheckInRate(data)}）",
									Types.CardLevel => $"级（× {ScoringOperation.GetGlobalRate(data)}）"
								};
								return $"#{i + 1,2} {name} - {data} {unit}";
							}
						)
					)}
					---
					排名最多仅列举本群前 {finalTopCount} 名的成绩；想要精确查看用户名次请使用“查询”指令。
					"""
				);

				break;
			}
			default:
			{
				await messageReceiver.SendMessageAsync("您选取的排序依据不是正确的数据值，无法判断排序内容。请检查输入。");
				break;
			}
		}


		async Task rankingListIsEmptyCallback() => await messageReceiver.SendMessageAsync("群用户列表为空。");

		async Task<IEnumerable<(string Name, T Data)>> getDataAsync<T>(Func<User, T> dataSelector)
			=> (await ScoringOperation.GetUserRankingListAsync(group, rankingListIsEmptyCallback, dataSelector))!.Take(finalTopCount);
	}
}

/// <summary>
/// 为参数“类型”提供数据。
/// </summary>
file static class Types
{
	/// <summary>
	/// 表示排名的数据为基本数据。该排名依据会使得结果排序将经验值、金币和魔塔数据都显示出来。
	/// </summary>
	public const string Basic = "基本";

	/// <summary>
	/// 表示排名的数据为经验值。
	/// </summary>
	public const string ExperiencePoint = "经验值";

	/// <summary>
	/// 表示排名的数据为金币。
	/// </summary>
	public const string Coin = "金币";

	/// <summary>
	/// 表示排名的数据为魔塔。
	/// </summary>
	public const string Tower = "魔塔";

	/// <summary>
	/// 表示排名的数据为级别。
	/// </summary>
	public const string Grade = "级别";

	/// <summary>
	/// 表示排名的数据为签到连续天数。
	/// </summary>
	public const string ContinuousCheckIn = "签到";

	/// <summary>
	/// 表示排名的数据为强化级别。
	/// </summary>
	public const string CardLevel = "强化";

	/// <summary>
	/// 表示排名的数据为对抗。
	/// </summary>
	public const string PkResult = "对抗";
}

/// <summary>
/// 转换 <see cref="RankCommand.GameMode"/> 参数数值的转换器对象。
/// </summary>
/// <seealso cref="RankCommand.GameMode"/>
file sealed class GameModeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(string value) => value switch { "九数找相同" => GameMode.FindDifference, _ => throw new CommandConverterException() };
}

/// <summary>
/// 一个元组对象，存储三个数据表示用户对 PK 进行回答的数据。
/// </summary>
/// <param name="Total">表示用户参与了多少次 PK。</param>
/// <param name="Corrected">表示用户正确了多少题目。</param>
/// <param name="Answered">表示用户正确回答了多少个题目。</param>
/// <param name="GameMode">表示用户需要照哪一个游戏模式进行排序。</param>
file sealed record PlayingDataTuple(int Total, int Corrected, int Answered, GameMode GameMode) : IComparable<PlayingDataTuple>
{
	/// <inheritdoc/>
	public int CompareTo(PlayingDataTuple? other)
	{
		ArgumentNullException.ThrowIfNull(other);

		var (leftTotal, leftCorrected) = (Total, Corrected);
		var (rightTotal, rightCorrected) = (other.Total, other.Corrected);
		var leftWinningPercentage = leftCorrected / (double)leftTotal;
		var rightWinningPercentage = rightCorrected / (double)rightTotal;
		switch (leftWinningPercentage, rightWinningPercentage)
		{
			case (double.NaN, double.NaN):
			{
				// 全为 0 / 0。
				// 模式匹配允许传入 double.NaN，即使我们知道和 NaN 比较运算符的结果总是 false，但模式匹配有特殊优化。
				return 0;
			}
			case (double.NaN, _):
			{
				// 左边没有合适数据（参与 0 次），右边有数据，就算右边排名靠前。
				return -1;
			}
			case (_, double.NaN):
			{
				// 右边没有合适数据，左边有数据，就算左边排名靠前。
				return 1;
			}
			default:
			{
				if (!leftWinningPercentage.NearlyEquals(rightWinningPercentage, 1E-5))
				{
					// 排名正确率。正确率高的排名靠前。
					return Sign(leftWinningPercentage - rightWinningPercentage);
				}

				if (leftTotal != rightTotal)
				{
					// 同等正确率时，按完成次数排名。如果参与局数越多，排名越靠前。
					return Sign(leftTotal - rightTotal);
				}

				// 正确率和完成局数都一样，说明数据都是一样的，那么两者排名一致。
				return 0;
			}
		}
	}
}
