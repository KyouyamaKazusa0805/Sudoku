namespace SudokuStudio.Configuration;

/// <summary>
/// Represents a type that defines the details of a grid.
/// </summary>
public sealed class GridInfo
{
	/// <summary>
	/// Indicates the grid.
	/// </summary>
	public required Grid BaseGrid { get; set; }

	/// <summary>
	/// Indicates whether all candidates will be displayed. The default value is <see langword="true"/>.
	/// </summary>
	public bool ShowCandidates { get; set; } = true;

	/// <summary>
	/// Indicates the extra string text for representation of <see cref="BaseGrid"/>. This value is <see langword="null"/>
	/// unless it won't be serialized as normal way.
	/// </summary>
	/// <remarks>
	/// On deserialization, this property has a higher priority value than <see cref="BaseGrid"/>,
	/// meaning if this property isn't <see langword="null"/>, this property will be used for deserialization;
	/// the property <see cref="BaseGrid"/> won't be used; otherwise, <see cref="BaseGrid"/> will be used.
	/// </remarks>
	/// <seealso cref="BaseGrid"/>
	public string? GridString { get; set; }

	/// <summary>
	/// Indicates the data that represents the conclusions and view nodes.
	/// </summary>
	public UserDefinedDrawable? RenderableData { get; set; }
}
