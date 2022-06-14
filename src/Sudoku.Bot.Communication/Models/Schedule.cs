namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Defines a schedule instance.
/// </summary>
/// <remarks>
/// The data type is referenced from
/// <see href="https://bot.q.qq.com/wiki/develop/api/openapi/schedule/model.html#schedule">this link</see>.
/// </remarks>
public sealed class Schedule
{
	/// <summary>
	/// Initializes a <see cref="Schedule"/> instance.
	/// </summary>
	public Schedule() => (StartTime, EndTime) = (DateTime.Now.AddMinutes(10), StartTime.AddHours(1));

	/// <summary>
	/// Initializes a <see cref="Schedule"/> instance, with the specified data.
	/// </summary>
	/// <param name="name">The name of the schedule.</param>
	/// <param name="description">The description of the schedule.</param>
	/// <param name="startTime">The start time.</param>
	/// <param name="endTime">
	/// The end time. The end time is by default later than <paramref name="startTime"/> 1 hour.
	/// </param>
	/// <param name="jumpChannel">The channel that jumped when starting.</param>
	/// <param name="remindType">The remind type.</param>
	public Schedule(
		string? name = null, string? description = null, DateTime? startTime = null,
		DateTime? endTime = null, Channel? jumpChannel = null, RemindType remindType = RemindType.Never)
		=> (Name, Description, StartTime, EndTime, JumpChannelId, RemindType) = (
			name ?? StringResource.Get("DefaultScheduleName"),
			description ?? StringResource.Get("DefaultScheduleDescription"),
			startTime ?? DateTime.Now.AddMinutes(10),
			endTime ?? StartTime.AddHours(1),
			jumpChannel?.Id,
			remindType
		);


	/// <summary>
	/// Indicates the ID of the schedule.
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	/// <summary>
	/// Indicates the name of the schedule.
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; set; }

	/// <summary>
	/// Indicates the description of the schedule.
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	/// <summary>
	/// Indicates the start time, as the timestamp representation, in milliseconds.
	/// The value must be greater than the current time.
	/// </summary>
	[JsonPropertyName("start_timestamp")]
	public string StartTimestamp
	{
		get => new DateTimeOffset(StartTime).ToUnixTimeMilliseconds().ToString();

		set => StartTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value)).DateTime;
	}

	/// <summary>
	/// Indicates the start time. The value must be greater than the current time.
	/// </summary>
	[JsonIgnore]
	public DateTime StartTime { get; set; }

	/// <summary>
	/// Indicates the end time, as the timestamp representation, in milliseconds.
	/// The value must be greater than the start time.
	/// </summary>
	[JsonPropertyName("end_timestamp")]
	public string EndTimestamp
	{
		get => new DateTimeOffset(EndTime).ToUnixTimeMilliseconds().ToString();

		set => EndTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(value)).DateTime;
	}

	/// <summary>
	/// Indicates the end time. The value must be greater than the start time.
	/// </summary>
	[JsonIgnore]
	public DateTime EndTime { get; set; }

	/// <summary>
	/// Indicates the member who creates the schedule.
	/// </summary>
	[JsonPropertyName("creator")]
	public Member? Creator { get; set; }

	/// <summary>
	/// Indicates the channel ID that the schedule jumped when starting.
	/// </summary>
	[JsonPropertyName("jump_channel_id")]
	public string? JumpChannelId { get; set; }

	/// <summary>
	/// Indicates the remind type.
	/// </summary>
	[JsonPropertyName("remind_type"), JsonConverter(typeof(RemindTypeNumberConverter))]
	public RemindType RemindType { get; set; }
}
