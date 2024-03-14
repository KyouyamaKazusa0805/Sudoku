namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Provides with difficulty rating value defined in this project for a <see cref="Technique"/> field.
/// </summary>
/// <param name="value">Indicates the value of the rating.</param>
/// <remarks>
/// <para>
/// Due to design of C#, <see cref="decimal"/> values cannot be as an argument while initializing an attribute type,
/// here we use a <see cref="double"/> value to achieve this.
/// </para>
/// <para>
/// In addition, this type may contain same value as arguments passing into <see cref="SudokuExplainerTechniqueAttribute"/>;
/// but sometimes they are not.
/// </para>
/// </remarks>
/// <seealso cref="Technique"/>
/// <seealso cref="SudokuExplainerTechniqueAttribute"/>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class StaticDifficultyAttribute([PrimaryConstructorParameter] double value) : Attribute
{
	/// <summary>
	/// Indicates the value that is used in direct mode.
	/// </summary>
	public double ValueInDirectMode { get; init; }
}
