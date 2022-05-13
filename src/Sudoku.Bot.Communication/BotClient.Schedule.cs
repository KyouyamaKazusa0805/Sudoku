namespace Sudoku.Bot.Communication;

partial class BotClient
{
	/// <summary>
	/// Gets the schedules in the specified channel.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="since">
	/// Indicates the date time with wich the returned schedules should start.
	/// If the value is <see langword="null"/>, the method will return all schedules in <see cref="DateTime.Today"/>.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates a list of schedules beginning with the specified time, as the result value.
	/// </returns>
	public async Task<List<Schedule>?> GetSchedulesAsync(string channel_id, DateTime? since, Sender? sender)
		=> BotApis.GetSchedulesInChannel is { Path: var path, Method: var method }
		&& since switch
		{
			{ } value => $"?since={new DateTimeOffset(value).ToUnixTimeMilliseconds()}",
			_ => string.Empty
		} is var param
		&& $"{path.ReplaceArgument(channel_id)}{param}" is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<List<Schedule>?>()
			: null;

	/// <summary>
	/// Gets the details of the specified schedule in the specified channel.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="schedule_id">The schedule. <b>The argument cannot be renamed.</b></param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the schedule instance as the result value.</returns>
	public async Task<Schedule?> GetScheduleAsync(string channel_id, string schedule_id, Sender? sender)
		=> BotApis.GetScheduleInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).ReplaceArgument(schedule_id) is var replacedPath
		&& await HttpSendAsync(replacedPath, method, null, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Schedule?>()
			: null;

	/// <summary>
	/// Creates a schedule instance in the specified channel. The member emitting the message
	/// must be required with the permission being able to create such schedule.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="schedule">
	/// The schedule instance. The <see cref="Schedule.Id"/> isn't needed being assigned;
	/// in addition, the property value <see cref="Schedule.StartTime"/> must be greater than
	/// <see cref="DateTime.Now"/>.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the result schedule instance.</returns>
	/// <seealso cref="Schedule.Id"/>
	/// <seealso cref="Schedule.StartTime"/>
	/// <seealso cref="DateTime.Now"/>
	public async Task<Schedule?> CreateScheduleAsync(string channel_id, Schedule schedule, Sender? sender)
		=> BotApis.CreateScheduleInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id) is var replacedPath
		&& JsonContent.Create(new { schedule }) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Schedule?>()
			: null;

	/// <summary>
	/// Modify the specified schedule. The member emitting the message
	/// must be required with the permission being able to create such schedule.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="schedule">The schedule instance having been modified.</param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>A task instance encapsulates the modified schedule instance.</returns>
	public async Task<Schedule?> EditScheduleAsync(string channel_id, Schedule schedule, Sender? sender)
		=> BotApis.ModifyScheduleInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).Replace("{schedule_id}", schedule.Id) is var replacedPath
		&& JsonContent.Create(new { schedule }) is var jsonContent
		&& await HttpSendAsync(replacedPath, method, jsonContent, sender) is { Content: var responseContent }
			? await responseContent.ReadFromJsonAsync<Schedule?>()
			: null;

	/// <summary>
	/// Deletes the specified schedule from the specified channel. The member emitting the message
	/// must be required with the permission being able to create such schedule.
	/// </summary>
	/// <param name="channel_id">The channel. <b>The argument cannot be renamed.</b></param>
	/// <param name="schedule">
	/// The schedule instance. In fact here we only use the property <see cref="Schedule.Id"/>.
	/// </param>
	/// <param name="sender">The sender who sends the message.</param>
	/// <returns>
	/// A task instance encapsulates the <see cref="bool"/> value indicating whether the HTTP response code is 204,
	/// as the result value.
	/// </returns>
	public async Task<bool> DeleteScheduleAsync(string channel_id, Schedule schedule, Sender? sender)
		=> BotApis.DeleteScheduleInChannel is { Path: var path, Method: var method }
		&& path.ReplaceArgument(channel_id).Replace("{schedule_id}", schedule.Id) is var replacedPath
		&& ((await HttpSendAsync(replacedPath, method, null, sender))?.IsSuccessStatusCode ?? false);
}
