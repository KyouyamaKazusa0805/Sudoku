namespace SudokuStudio.Models;

/// <summary>
/// Defines a routing data of technique page.
/// </summary>
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public sealed partial class TechniquePageRoutingData :
	IEquatable<TechniquePageRoutingData>,
	IEqualityOperators<TechniquePageRoutingData, TechniquePageRoutingData, bool>
{
	/// <summary>
	/// Indicates the default culture.
	/// </summary>
	private static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo(1033);


	/// <summary>
	/// Indicates whether the page is enabled.
	/// </summary>
	public bool IsEnabled { get; set; } = true;

	/// <summary>
	/// Indicates the routing page type name.
	/// </summary>
	public required string RoutingPageTypeName { get; set; }

	/// <summary>
	/// Indicates the resource name of this technique.
	/// </summary>
	public string TechniqueResourceName => MergedResources.R[Technique.ToString()]!;

	/// <summary>
	/// Indicates the resource default name (English name) of this technique.
	/// </summary>
	public string TechniqueEnglishName => SudokuDefaultResource.ResourceManager.GetString(Technique.ToString(), DefaultCulture)!;

	/// <summary>
	/// Indicates the technique code that the page related to.
	/// </summary>
	public required Technique Technique { get; set; }

	/// <summary>
	/// Indicates the hash code of the data.
	/// </summary>
	private int HashCode => (int)Technique;


	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] TechniquePageRoutingData? other) => other is not null && Technique == other.Technique;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.SimpleField, nameof(HashCode))]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.SimpleMember, nameof(TechniqueResourceName))]
	public override partial string ToString();
}
