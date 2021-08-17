namespace Sudoku.CodeGenerating;

/// <summary>
/// Indicates an attribute to mark a type, to indicate the type will be generated a default method
/// called <c>Equals</c>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class AutoEqualityAttribute : Attribute
{
	/// <summary>
	/// Initializes an instance with the specified members.
	/// </summary>
	/// <param name="members">The members.</param>
	public AutoEqualityAttribute(params string[] members) => FieldOrPropertyList = members;


	/// <summary>
	/// Indicates the field of property list.
	/// </summary>
	public string[] FieldOrPropertyList { get; }
}
