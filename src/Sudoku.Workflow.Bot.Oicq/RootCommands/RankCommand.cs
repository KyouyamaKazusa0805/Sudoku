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

#if false
	/// <summary>
	/// 表示你要排名的游戏模式。默认为 <see cref="GameMode.FindDifference"/>。
	/// </summary>
	[DoubleArgument("模式")]
	[Hint($"表示你要排名的游戏模式。默认为“九数找相同”。")]
	[DefaultValue<GameMode>(GameMode.FindDifference)]
	[ValueConverter<GameModeConverter>]
	[DisplayingIndex(2)]
	public GameMode GameMode { get; set; }
#endif


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
				var usersData = (
					await ScoringOperation.GetUserRankingListAsync(
						group,
						async () => await messageReceiver.SendMessageAsync("群用户列表为空。")
					)
				)!.Take(finalTopCount);

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
			case var type and (
				Types.ExperiencePoint
				or Types.Coin
				or Types.Grade
				or Types.Tower
				or Types.ContinuousCheckIn
				or Types.CardLevel
#if false
				or Types.PkResult
#endif
			):
			{
				var usersData = (
					await ScoringOperation.GetUserRankingListAsync(
						group,
						async () => await messageReceiver.SendMessageAsync("群用户列表为空。"),
						ud => type switch
						{
							Types.ExperiencePoint => ud.ExperiencePoint,
							Types.Coin => ud.Coin,
							Types.CardLevel => ud.CardLevel,
							Types.Grade => ScoringOperation.GetGrade(ud.ExperiencePoint),
							Types.Tower => ud.TowerOfSorcerer,
							Types.ContinuousCheckIn => ud.ComboCheckedIn
						}
					)
				)!.Take(finalTopCount);

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

#if false
	/// <summary>
	/// 表示排名的数据为对抗。
	/// </summary>
	public const string PkResult = "对抗";
#endif
}

#if false
/// <summary>
/// 转换 <see cref="RankCommand.GameMode"/> 参数数值的转换器对象。
/// </summary>
/// <seealso cref="RankCommand.GameMode"/>
file sealed class GameModeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(string value) => value switch { "九数找相同" => GameMode.FindDifference, _ => throw new CommandConverterException() };
}
#endif