namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates an attribute that is used for a <see langword="class"/> or <see langword="struct"/>
/// as a mark that interacts with the source generator, to tell the source generator that
/// it'll generate the source code for deconstruction methods.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructAttribute : Attribute
{
	/// <summary>
	/// Initializes an instance with the specified member list.
	/// </summary>
	/// <param name="members">The members.</param>
	public AutoDeconstructAttribute(params string[] members) => FieldOrPropertyList = members;


	/// <summary>
	/// All members to deconstruct.
	/// </summary>
	public string[] FieldOrPropertyList { get; }
}