namespace Sudoku.Communication.Qicq.Models;

/// <summary>
/// Defines a user data.
/// </summary>
public sealed class UserData
{
	/// <summary>
	/// The user's QQ number.
	/// </summary>
	[JsonPropertyName("qq")]
	[JsonPropertyOrder(0)]
	public required string QQ { get; set; }

	/// <summary>
	/// Indicates the user's score.
	/// </summary>
	[JsonPropertyName("exp")]
	public int Score { get; set; }

	/// <summary>
	/// Indicates the number of continuous days that the user has checked in.
	/// </summary>
	[JsonPropertyName("continuousDaysCheckIn")]
	public int ComboCheckedIn { get; set; }

	/// <summary>
	/// Indicates the last time checked in of the current user.
	/// </summary>
	[JsonPropertyName("lastCheckInDate")]
	public DateTime LastCheckIn { get; set; }
}
