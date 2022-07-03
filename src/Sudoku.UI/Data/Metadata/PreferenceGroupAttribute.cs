namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that can be applied to a preference item property,
/// indicating which group the preference belongs to.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class PreferenceGroupAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="PreferenceGroupAttribute"/> instance via the name and the description.
	/// </summary>
	/// <param name="name">The name.</param>
	public PreferenceGroupAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the group name.
	/// </summary>
	public string Name { get; }
}
