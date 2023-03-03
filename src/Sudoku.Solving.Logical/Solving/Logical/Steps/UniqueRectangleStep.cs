﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2">
/// <para>Indicates the technique code.</para>
/// <para><i>
/// Limited by the C# language, here we creates a new property <see cref="TechniqueCode2"/>
/// to pass the value and assign it to the property <see cref="Step.TechniqueCode"/>. If write code
/// to place the property <see cref="Step.TechniqueCode"/> into the primary constructor as a parameter,
/// the default member named <c>TechniqueCode</c> may be duplicate with this parameter's,
/// which isn't allowed in <see langword="record"/> types in the language design.
/// </i></para>
/// </param>
/// <param name="Digit1">Indicates the first digit used in this unique rectangle pattern.</param>
/// <param name="Digit2">Indicates the second digit used in this unique rectangle pattern.</param>
/// <param name="Cells">Indicates the cells used.</param>
/// <param name="IsAvoidable">
/// <para>Indicates whether the current unique rectangle is avoidable rectangle.</para>
/// <para>
/// For the designation and the implementation, <b>Avoidable Rectangle</b> and <b>Unique Rectangle</b>
/// are used as the same type.
/// </para>
/// </param>
/// <param name="AbsoluteOffset">
/// <para>Indicates the absolute offset.</para>
/// <para>
/// The value will be an <see cref="int"/> value to compare all possible cases
/// of unique rectangle structures to be iterated. The greater the value is,
/// the later the unique rectangle structure will be processed. The value must be between 0 and 485.
/// Other values are invalid and useless. The number of all possible unique rectangle structures is 486.
/// </para>
/// </param>
/// <seealso cref="Step.TechniqueCode"/>
internal abstract record UniqueRectangleStep(
	Conclusion[] Conclusions,
	View[]? Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	bool IsAvoidable,
	int AbsoluteOffset
) : DeadlyPatternStep(Conclusions, Views), IDistinctableStep<UniqueRectangleStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <inheritdoc/>
	public override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => TechniqueCode2;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectangle;

	private protected string D1Str => (Digit1 + 1).ToString();

	private protected string D2Str => (Digit2 + 1).ToString();

	private protected string CellsStr => Cells.ToString();


	/// <inheritdoc/>
	public static bool Equals(UniqueRectangleStep left, UniqueRectangleStep right)
		=> left.TechniqueCode == right.TechniqueCode && left.AbsoluteOffset == right.AbsoluteOffset
		&& left.Digit1 == right.Digit1 && left.Digit2 == right.Digit2;
}
