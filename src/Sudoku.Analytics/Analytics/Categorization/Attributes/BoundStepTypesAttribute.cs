namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Provides with an attribute type that specifies the types where a <see cref="Technique"/> field can be produced.
/// </summary>
/// <param name="primaryType">Indicates the types derived from <see cref="Step"/>.</param>
/// <seealso cref="Technique"/>
/// <seealso cref="Step"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class BoundStepTypesAttribute([PrimaryConstructorParameter] Type primaryType) : Attribute
{
	/// <summary>
	/// Indicates the secondary <see cref="Step"/> types.
	/// </summary>
	/// <remarks>
	/// The types will be used as compatibility. For example, a last digit can be created
	/// using <see cref="LastDigitStep"/>; however, it can be also created by using <see cref="HiddenSingleStep"/>
	/// with more arguments. This property will be initialized as <c>[<see langword="typeof"/>(<see cref="HiddenSingleStep"/>)]</c>.
	/// </remarks>
	public Type[]? SecondaryTypes { get; init; }
}
