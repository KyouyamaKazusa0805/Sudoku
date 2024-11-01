namespace System.Runtime.Versioning;

/// <summary>
/// Represents an attribute type that is used for describing supported version.
/// </summary>
/// <param name="version">Indicates the target version.</param>
public abstract partial class VersioningAttribute([Property] Version version) : Attribute
{
	/// <summary>
	/// Indicates the description of this API about why it is created or deprecated.
	/// </summary>
	public string? DescriptionLink { get; init; }
}
