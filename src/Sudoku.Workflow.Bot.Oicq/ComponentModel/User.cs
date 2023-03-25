namespace Sudoku.Workflow.Bot.Oicq.ComponentModel;

/// <summary>
/// 表示一个可提供 JSON 序列化/反序列化的用户基本数据。
/// </summary>
public sealed class User
{
	/// <summary>
	/// 用户的账号（QQ 号）。
	/// </summary>
	[JsonPropertyName("qq")]
	[JsonPropertyOrder(0)]
	public required string Number { get; set; }

	/// <summary>
	/// 用户的强化卡最高级别。
	/// </summary>
	[JsonIgnore]
	public int CardLevel => UplevelingCards.Keys is var keys && keys.Any() ? keys.Max() : 0;

	/// <summary>
	/// 用户在使用机器人期间积攒的经验值。
	/// </summary>
	[JsonPropertyName("exp")]
	public int ExperiencePoint { get; set; }

	/// <summary>
	/// 用户在使用机器人期间积攒的金币。
	/// </summary>
	[JsonPropertyName("coin")]
	public int Coin { get; set; }

	/// <summary>
	/// 表示用户连续签到的天数。
	/// </summary>
	[JsonPropertyName("continuousDaysCheckIn")]
	public int ComboCheckedIn { get; set; }

	/// <summary>
	/// 记录的是用户上一次签到时候的时间。之所以不是 <see cref="TimeOnly"/> 是因为程序需要判别签到是否连续，需要明确知晓日期；
	/// 而之所以不是 <see cref="DateOnly"/> 是因为程序需要按签到的精确时间提示用户当天已经签过到了。
	/// </summary>
	[JsonPropertyName("lastCheckInDate")]
	public DateTime LastCheckIn { get; set; }

	/// <summary>
	/// 记录的是用户上一轮回答答案的时间。该属性禁止用户重复作答。
	/// </summary>
	[JsonPropertyName("lastAnswerDailyPuzzle")]
	public DateTime LastAnswerDailyPuzzle { get; set; }

	/// <summary>
	/// 用于预存的强化卡。强化卡用于强化操作。强化卡等级越高，那么在获得经验或金币的时候，可以越多获得它们。
	/// </summary>
	[JsonPropertyName("uplevelingCards")]
	public Dictionary<int, int> UplevelingCards { get; set; } = new();

	/// <summary>
	/// 表示用户除了强化卡外的物品。
	/// </summary>
	[JsonPropertyName("items")]
	public Dictionary<Item, int> Items { get; set; } = new();

	/// <summary>
	/// 表示用户游玩 PK 时答对了的题目数量的数据。
	/// </summary>
	[JsonPropertyName("correctedPlaying")]
	public Dictionary<GameMode, int> CorrectedCount { get; set; } = new();

	/// <summary>
	/// 表示用户游玩 PK 时回答题目的数据。因为游戏允许一轮多次回答题目，因此一道题用户回答了三次，第三次才正确，那么这个属性将记录的数据是 3 而不是 1。
	/// </summary>
	[JsonPropertyName("triedPlaying")]
	public Dictionary<GameMode, int> TriedCount { get; set; } = new();

	/// <summary>
	/// 表示用户一共尝试完成了多少次答题。
	/// </summary>
	[JsonPropertyName("totalPlaying")]
	public Dictionary<GameMode, int> TotalPlayingCount { get; set; } = new();
}
