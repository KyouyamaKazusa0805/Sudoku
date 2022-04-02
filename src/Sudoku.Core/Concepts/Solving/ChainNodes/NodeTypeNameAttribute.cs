namespace Sudoku.Concepts.Solving.ChainNodes;

/// <summary>
/// Defines an attribute that can be applied to a field in the type <see cref="NodeType"/>,
/// to tell the runtime that the name is bound with the field, in order to display in the console or UI.
/// </summary>
/// <seealso cref="NodeType"/>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class NodeTypeNameAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="NodeTypeNameAttribute"/> instance via the name.
	/// </summary>
	/// <param name="name">The name.</param>
	public NodeTypeNameAttribute(string name) => Name = name;


	/// <summary>
	/// Indicates the name.
	/// </summary>
	public string Name { get; }
}
