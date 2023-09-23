using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Eliminations;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet</b> technique.
/// </summary>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="exocet">Indicates the exocet pattern used.</param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
/// <param name="eliminations">Indicates the eliminations, grouped by the type.</param>
public abstract partial class ExocetStep(
	View[]? views,
	StepSearcherOptions options,
	[DataMember] Exocet exocet,
	[DataMember] Mask digitsMask,
	ExocetElimination[] eliminations
) : Step(from e in eliminations from c in e.Conclusions select c, views, options)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 9.4M;

	private protected string DigitsStr => Options.Converter.DigitConverter(DigitsMask);

	private protected string BaseCellsStr => Options.Converter.CellConverter(BaseMap);

	private protected string TargetCellsStr => Options.Converter.CellConverter(TargetMap);

	/// <summary>
	/// Indicates the map of the base cells.
	/// </summary>
	private CellMap BaseMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.BaseCellsMap;
	}

	/// <summary>
	/// Indicates the map of the target cells.
	/// </summary>
	private CellMap TargetMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.TargetCellsMap;
	}
}
