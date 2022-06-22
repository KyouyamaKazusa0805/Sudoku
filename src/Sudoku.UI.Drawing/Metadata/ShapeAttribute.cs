namespace Sudoku.UI.Drawing.Metadata;

/// <summary>
/// Defines a type that marks a field, indicating the field binding with a shape kind.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
internal sealed class ShapeAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="ShapeAttribute"/> instance via the specified shape kind.
	/// </summary>
	/// <param name="kind">The shape kind.</param>
	public ShapeAttribute(ShapeKind kind) => Kind = kind;


	/// <summary>
	/// Indicates the shape kind.
	/// </summary>
	public ShapeKind Kind { get; }
}
