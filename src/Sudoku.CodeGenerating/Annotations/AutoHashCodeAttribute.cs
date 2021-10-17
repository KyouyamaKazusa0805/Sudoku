namespace Sudoku.CodeGenerating;

/// <summary>
/// Indicates an attribute that marks a <see langword="class"/> or a <see langword="struct"/>
/// that tells the compiler the type should generate a default <c>GetHashCode</c> method.
/// </summary>
[AttributeUsage(Class | Struct, AllowMultiple = false, Inherited = false)]
public sealed class AutoHashCodeAttribute : Attribute
{
	/// <summary>
	/// Initializes an instance with the specified member list.
	/// </summary>
	/// <param name="dataMembers">The data members.</param>
	public AutoHashCodeAttribute(params string[] dataMembers) => DataMembers = dataMembers;


	/// <summary>
	/// All members to generate.
	/// </summary>
	public string[] DataMembers { get; }
}
