namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that can be applied to a preference item property,
/// indicating which group the preference belongs to.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class PreferenceGroupAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="PreferenceGroupAttribute"/> instance via the name and the index of ordering.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="orderingIndex">The index.</param>
	public PreferenceGroupAttribute(string name, int orderingIndex) => (Name, OrderingIndex) = (name, orderingIndex);


	/// <summary>
	/// Indicates the group name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Indicates the index of the ordering in the group.
	/// </summary>
	public int OrderingIndex { get; }
}
