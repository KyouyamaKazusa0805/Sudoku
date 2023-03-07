namespace Sudoku.Platforms.QQ.Models;

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
	/// Indicates the user's experience point.
	/// </summary>
	[JsonPropertyName("exp")]
	public int ExperiencePoint { get; set; }

	/// <summary>
	/// Indicates the user's coin.
	/// </summary>
	[JsonPropertyName("coin")]
	public int Coin { get; set; }

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

	/// <summary>
	/// Indicates the shopping items.
	/// </summary>
	[JsonPropertyName("items")]
	public Dictionary<ShoppingItem, int> Items { get; set; } = new();

	/// <summary>
	/// Indicates the corrected playing data.
	/// </summary>
	[JsonPropertyName("correctedPlaying")]
	public Dictionary<GamingMode, int> CorrectedCount { get; set; } = new();

	/// <summary>
	/// Indicates the tried playing data.
	/// </summary>
	[JsonPropertyName("triedPlaying")]
	public Dictionary<GamingMode, int> TriedCount { get; set; } = new();

	/// <summary>
	/// Indicates the total playing data.
	/// </summary>
	[JsonPropertyName("totalPlaying")]
	public Dictionary<GamingMode, int> TotalPlayingCount { get; set; } = new();
}
