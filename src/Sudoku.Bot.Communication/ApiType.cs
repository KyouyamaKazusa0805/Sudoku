namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the type of the API.
/// </summary>
public enum ApiType
{
	/// <summary>
	/// Indicates the API is used for both domains.
	/// </summary>
	Both,

	/// <summary>
	/// Indicates the API is used for public domain.
	/// </summary>
	PublicDomain,

	/// <summary>
	/// Indicates the API is used for private domain.
	/// </summary>
	PrivateDomain
}
