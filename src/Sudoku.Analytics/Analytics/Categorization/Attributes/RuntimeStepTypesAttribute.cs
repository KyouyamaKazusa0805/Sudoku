namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with an attribute type that specifies the types where a <see cref="Technique"/> field can be produced.
/// </summary>
/// <param name="stepTypes">Indicates the types derived from <see cref="Step"/>.</param>
/// <seealso cref="Technique"/>
/// <seealso cref="Step"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class RuntimeStepTypesAttribute([PrimaryConstructorParameter] params Type[] stepTypes) : Attribute;
