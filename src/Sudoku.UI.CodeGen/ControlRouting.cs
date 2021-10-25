namespace Sudoku.Diagnostics.CodeGen;

/// <summary>
/// Indicates a routing method that makes the UI control and a preference item become a relation.
/// </summary>
/// <param name="context">The source generator context on generating source code.</param>
/// <param name="propertyName">The property name of the preference item.</param>
/// <param name="ctorArgs">The constructor arguments in attribute.</param>
/// <param name="namedArgs">The named arguments in attribute.</param>
/// <param name="controlName">The control name bound.</param>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
internal delegate void ControlRouting(
	ref GeneratorExecutionContext context,
	string propertyName,
	ImmutableArray<TypedConstant> ctorArgs,
	ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs,
	string controlName
);