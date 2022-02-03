using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Solving.Collections;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is an <b>Exocet</b> technique.
/// </summary>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Exocet">INdicates the exocet pattern.</param>
/// <param name="DigitsMask">Indicates the mask that holds all possible digits used.</param>
/// <param name="Eliminations">Indicates all possible eliminations.</param>
public abstract record ExocetStep(
	ImmutableArray<PresentationData> Views,
	in ExocetPattern Exocet,
	short DigitsMask,
	ImmutableArray<ExocetElimination> Eliminations
) : Step(GatherConclusions(Eliminations), Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override bool IsElementary => base.IsElementary;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Exocet;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Exocet;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Nightmare;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;

	/// <summary>
	/// Indiactes the digits string.
	/// </summary>
	[FormatItem]
	internal string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	/// <summary>
	/// Indicates the base map string.
	/// </summary>
	[FormatItem]
	internal string BaseMapStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BaseMap.ToString();
	}

	/// <summary>
	/// Indicates the target map string.
	/// </summary>
	[FormatItem]
	internal string TargetMapStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => TargetMap.ToString();
	}

	/// <summary>
	/// Indicates the map of the base cells.
	/// </summary>
	private Cells BaseMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.BaseCellsMap;
	}

	/// <summary>
	/// Indicates the map of the target cells.
	/// </summary>
	private Cells TargetMap
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Exocet.TargetCellsMap;
	}


	/// <inheritdoc/>
	public sealed override string ToFullString()
	{
		var sb = new StringHandler(initialCapacity: 100);
		sb.Append(base.ToFullString());
		sb.AppendLine();
		sb.AppendRangeWithLines(Eliminations);

		return sb.ToStringAndClear();
	}


	/// <summary>
	/// Gather conclusions.
	/// </summary>
	/// <returns>The gathered result.</returns>
	private static ImmutableArray<Conclusion> GatherConclusions(ImmutableArray<ExocetElimination> eliminations)
	{
		var result = new List<Conclusion>();
		foreach (var eliminationInstance in eliminations)
		{
			result.AddRange(eliminationInstance.AsSpan().ToArray());
		}

		return result.ToImmutableArray();
	}
}
