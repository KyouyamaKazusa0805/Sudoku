namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that can be applied to a type derived from <see cref="Page"/>,
/// describing the metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageAttribute : Attribute
{
	/// <summary>
	/// Indicates whether the page displays title. The default value is <see langword="true"/>.
	/// </summary>
	public bool DisplayTitle { get; init; } = true;
}
