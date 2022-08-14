namespace Sudoku.Presentation;

/// <summary>
/// Represents an enumeration type that stores some kinds of possible behaviors on adding a view node
/// into a view node collection such as <see cref="UserDefinedDisplayable"/>.
/// </summary>
/// <seealso cref="UserDefinedDisplayable"/>
public enum ViewNodeAddBehavior
{
	/// <summary>
	/// Indicates the view node will be added without any reason.
	/// </summary>
	ForceAdd,

	/// <summary>
	/// Indicates replace the view node if a new duplicate one should be added.
	/// This option is used when the method <see cref="View.Contains(Predicate{ViewNode})"/> is called.
	/// </summary>
	/// <seealso cref="View.Contains(Predicate{ViewNode})"/>
	ReplaceIfDuplicate
}
