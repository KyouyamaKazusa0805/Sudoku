namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute that is used for a <see langword="class"/> or <see langword="struct"/>
/// as a mark that interacts with the source generator, to tell the source generator that
/// it'll generate the source code for equality methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
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
