namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// 日程对象
/// </summary>
public class Schedule
{
	/// <summary>
	/// 构造日程
	/// </summary>
	public Schedule() => (StartTime, EndTime) = (DateTime.Now.AddMinutes(10), StartTime.AddHours(1));

	/// <summary>
	/// 新建日程
	/// <para>
	/// 注1：开始时间必须大于当前时间<br/>
	/// 注2：结束时间必须大于开始时间<br/>
	/// 注3：调用API每日创建日程数量有限
	/// </para>
	/// </summary>
	/// <param name="name">日程名称</param>
	/// <param name="desc">日程描述</param>
	/// <param name="startTime">开始时间（默认五分钟后）</param>
	/// <param name="endTime">结束时间（默认持续一小时）</param>
	/// <param name="jumpChannel">日程开始时跳转的频道</param>
	/// <param name="remindType">日程时间即将到达时的提醒方式</param>
	public Schedule(
		string name = "新建日程", string desc = "新的日程", DateTime? startTime = null, DateTime? endTime = null,
		Channel? jumpChannel = null, RemindType remindType = RemindType.Never)
		=> (Name, Description, StartTime, EndTime, JumpChannelId, RemindType) = (
			name,
			desc,
			startTime ?? DateTime.Now.AddMinutes(10),
			endTime ?? StartTime.AddHours(1),
			jumpChannel?.Id,
			remindType
		);


	/// <summary>
	/// 日程 id
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }
	/// <summary>
	/// 日程名称
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }
	/// <summary>
	/// 日程描述
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }
	/// <summary>
	/// 日程开始时间戳(ms)
	/// <para>必须大于当前时间</para>
	/// </summary>
	[JsonPropertyName("start_timestamp")]
	public string StartTimestamp
	{
		get => new DateTimeOffset(StartTime).ToUnixTimeMilliseconds().ToString();

		set => StartTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value)).DateTime;
	}
	/// <summary>
	/// 日程开始时间
	/// <para>必须大于当前时间</para>
	/// </summary>
	[JsonIgnore]
	public DateTime StartTime { get; set; }
	/// <summary>
	/// 日程结束时间戳(ms)
	/// <para>必须大于开始时间</para>
	/// </summary>
	[JsonPropertyName("end_timestamp")]
	public string EndTimestamp
	{
		get => new DateTimeOffset(EndTime).ToUnixTimeMilliseconds().ToString();

		set => EndTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value)).DateTime;
	}
	/// <summary>
	/// 日程结束时间
	/// <para>必须大于开始时间</para>
	/// </summary>
	[JsonIgnore]
	public DateTime EndTime { get; set; }
	/// <summary>
	/// 创建者
	/// </summary>
	[JsonPropertyName("creator")]
	public Member? Creator { get; set; }
	/// <summary>
	/// 日程开始时跳转到的子频道 id
	/// </summary>
	[JsonPropertyName("jump_channel_id")]
	public string? JumpChannelId { get; set; }
	/// <summary>
	/// 日程提醒类型
	/// </summary>
	[JsonPropertyName("remind_type"), JsonConverter(typeof(RemindTypeToStringNumberConverter))]
	public RemindType RemindType { get; set; }
}
