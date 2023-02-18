namespace SudokuStudio.Models;

/// <summary>
/// Defines a routing data of technique page.
/// </summary>
public sealed class TechniquePageRoutingData
{
	/// <summary>
	/// Indicates the default culture.
	/// </summary>
	private static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo(1033);


	/// <summary>
	/// Indicates whether the page is enabled.
	/// </summary>
	[MemberNotNullWhen(true, nameof(RoutingPageTypeName))]
	public bool IsEnabled { get; set; } = true;

	/// <summary>
	/// Indicates the resource name of this technique.
	/// </summary>
	public string Name => MergedResources.R[Technique.ToString()]!;

	/// <summary>
	/// Indicates the resource default name (English name) of this technique.
	/// </summary>
	public string OriginalName => SudokuDefaultResource.ResourceManager.GetString(Technique.ToString(), DefaultCulture)!;

	/// <summary>
	/// Indicates the routing page type name.
	/// </summary>
	[DisallowNull]
	public string? RoutingPageTypeName { get; set; }

	/// <summary>
	/// Indicates the technique code that the page related to.
	/// </summary>
	public required Technique Technique { get; set; }

	/// <summary>
	/// Indicates the color of the current technique.
	/// </summary>
	public required Color Color { get; set; }
}
