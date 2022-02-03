using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2">
/// <para>Indicates the technique code.</para>
/// <para>
/// Limited by the C# language, here we creates a new property <see cref="TechniqueCode2"/>
/// to pass the value and assign it to the property <see cref="Step.TechniqueCode"/>. If write code
/// to place the property <see cref="Step.TechniqueCode"/> into the primary constructor as a parameter,
/// the default member named <c>TechniqueCode</c> may be duplicate with this parameter's,
/// which isn't allowed in <see langword="record"/> types in the langugae design.
/// </para>
/// </param>
/// <param name="Digit1">Indicates the the first digit used in this unique rectangle pattern.</param>
/// <param name="Digit2">Indicates the the second digit used in this unique rectangle pattern.</param>
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
public abstract record UniqueRectangleStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	in Cells Cells,
	bool IsAvoidable,
	int AbsoluteOffset
) : DeadlyPatternStep(Conclusions, Views), IDistinctableStep<UniqueRectangleStep>
{
	/// <inheritdoc/>
	public sealed override Technique TechniqueCode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TechniqueCode2;
	}

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectangle;

	/// <summary>
	/// Indicates the digit 1 string.
	/// </summary>
	[FormatItem]
	internal string D1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit1 + 1).ToString();
	}

	/// <summary>
	/// Indicates the digit 2 string.
	/// </summary>
	[FormatItem]
	internal string D2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit2 + 1).ToString();
	}

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[FormatItem]
	internal string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Cells.ToString();
	}


	/// <inheritdoc/>
	public static bool Equals(UniqueRectangleStep left, UniqueRectangleStep right) =>
		left.TechniqueCode == right.TechniqueCode && left.AbsoluteOffset == right.AbsoluteOffset
		&& left.Digit1 == right.Digit1 && left.Digit2 == right.Digit2;
}
