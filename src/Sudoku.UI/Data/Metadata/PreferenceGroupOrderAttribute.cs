namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that can be applied to a name field,
/// indicating the real displaying order in all groups, in the settings page.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class PreferenceGroupOrderAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="PreferenceGroupOrderAttribute"/> instance via the ordering index.
	/// </summary>
	/// <param name="orderingIndex">The ordering index.</param>
	public PreferenceGroupOrderAttribute(int orderingIndex) => OrderingIndex = orderingIndex;


	/// <summary>
	/// Indicates the ordering index.
	/// </summary>
	public int OrderingIndex { get; }
}
