namespace Sudoku.Solving.Manual.Text;

/// <summary>
/// Marks onto a step type (inherited from <see cref="Step"/> and implemented the interface <see cref="IStep"/>),
/// to tell the runtime that the property <see cref="Step.Format"/> will get the value
/// through the specified identifier name.
/// </summary>
/// <seealso cref="Step"/>
/// <seealso cref="Step.Format"/>
/// <seealso cref="IStep"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed partial class FormatForwardAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="FormatForwardAttribute"/> instance via an identifier name.
	/// </summary>
	/// <param name="identifierName">The identifier name.</param>
	public FormatForwardAttribute(string identifierName) => IdentifierName = identifierName;


	/// <summary>
	/// Indicates the identifier that the name the runtime will get from the resource dictionary.
	/// </summary>
	public string IdentifierName { get; }
}
