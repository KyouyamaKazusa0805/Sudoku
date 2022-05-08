namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// The API permissions.
/// </summary>
public sealed class ApiPermissions
{
	/// <summary>
	/// The API permissions.
	/// </summary>
	[JsonPropertyName("apis")]
	public List<ApiPermission>? List { get; set; }
}
