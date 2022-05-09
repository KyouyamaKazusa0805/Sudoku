namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the status of the API.
/// </summary>
public sealed class ApiStatus
{
	/// <summary>
	/// Indicates the code.
	/// </summary>
	[JsonPropertyName("code")]
	public int Code { get; set; }

	/// <summary>
	/// Indicates the message.
	/// </summary>
	[JsonPropertyName("message")]
	public string? Message { get; set; }
}
