#undef COMPATIBLE_ORIGINAL_TECHNIQUE_RULES

namespace Sudoku.Analytics.Categorization;

using static ExtraDifficultyFactorNames;

/// <summary>
/// Represents a technique instance, which is used for comparison.
/// </summary>
/// <remarks><b><i>
/// Please note that some fields declared in this type may not be used by neither direct reference nor reflection,
/// but they are reserved ones. In the future I'll implement the searching logic on those fields. Maybe or maybe not :D
/// </i></b></remarks>
public enum Technique
{
	/// <summary>
	/// The placeholder of this enumeration type.
	/// </summary>
	None,

	//
	// Singles
	//
	#region Singles
	/// <summary>
	/// Indicates full house. This technique is the most elementary technique to be used in the candidate view mode.
	/// </summary>
	[Hodoku(4, HodokuDifficultyLevel.Easy, Prefix = "0000")]
	[SudokuExplainer(1.0, SudokuExplainerTechnique.Single, Aliases = ["Single"])]
	[TechniqueMetadata(
		1.0, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(FullHouseStep),
		Links = ["http://sudopedia.enjoysudoku.com/Full_House.html"])]
	FullHouse,

	/// <summary>
	/// Indicates last digit.
	/// </summary>
	[Hodoku(Prefix = "0001")]
	[TechniqueMetadata(
		1.1, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(LastDigitStep),
		SecondarySupportedType = typeof(HiddenSingleStep), Links = ["http://sudopedia.enjoysudoku.com/Last_Digit.html"])]
	LastDigit,

	/// <summary>
	/// Indicates hidden single (in block).
	/// </summary>
	[Hodoku(14, HodokuDifficultyLevel.Easy, Prefix = "0002")]
	[SudokuExplainer(1.2, SudokuExplainerTechnique.HiddenSingle)]
	[TechniqueMetadata(
		1.9, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(HiddenSingleStep),
		PencilmarkVisibility = PencilmarkVisibility.Indirect, Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"])]
	HiddenSingleBlock,

	/// <summary>
	/// Indicates hidden single (in row).
	/// </summary>
	[Hodoku(14, HodokuDifficultyLevel.Easy, Prefix = "0002")]
	[SudokuExplainer(1.5, SudokuExplainerTechnique.HiddenSingle)]
	[TechniqueMetadata(
		2.3, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(HiddenSingleStep),
		PencilmarkVisibility = PencilmarkVisibility.Indirect, Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"])]
	HiddenSingleRow,

	/// <summary>
	/// Indicates hidden single (in column).
	/// </summary>
	[Hodoku(14, HodokuDifficultyLevel.Easy, Prefix = "0002")]
	[SudokuExplainer(1.5, SudokuExplainerTechnique.HiddenSingle)]
	[TechniqueMetadata(
		2.3, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(HiddenSingleStep),
		PencilmarkVisibility = PencilmarkVisibility.Indirect, Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"])]
	HiddenSingleColumn,

	/// <summary>
	/// Indicates naked single.
	/// </summary>
	[Hodoku(4, HodokuDifficultyLevel.Easy, Prefix = "0003")]
	[SudokuExplainer(2.3, SudokuExplainerTechnique.NakedSingle)]
	[TechniqueMetadata(
		2.3, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(NakedSingleStep),
		DirectRating = 1.0, Links = ["http://sudopedia.enjoysudoku.com/Naked_Single.html"])]
	NakedSingle,
	#endregion

	//
	// Direct Singles
	//
	#region Direct Singles
	/// <summary>
	/// Indicates crosshatching in block, equivalent to hidden single in block, but used in direct views.
	/// </summary>
	[TechniqueMetadata(
		1.2, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(HiddenSingleStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"],
		Features = TechniqueFeature.DirectTechniques)]
	CrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in row, equivalent to hidden single in row, but used in direct views.
	/// </summary>
	[TechniqueMetadata(
		1.5, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(HiddenSingleStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"],
		Features = TechniqueFeature.DirectTechniques)]
	CrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in column, equivalent to hidden single in column, but used in direct views.
	/// </summary>
	[TechniqueMetadata(
		1.5, DifficultyLevel.Easy, TechniqueGroup.Single, typeof(HiddenSingleStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"],
		Features = TechniqueFeature.DirectTechniques)]
	CrosshatchingColumn,
	#endregion

	//
	// Complex Singles
	//
	#region Complex Singles
	/// <summary>
	/// Indicates full house, with pointing.
	/// </summary>
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	PointingFullHouse,

	/// <summary>
	/// Indicates full house, with claiming.
	/// </summary>
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	ClaimingFullHouse,

	/// <summary>
	/// Indicates full house, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairFullHouse,

	/// <summary>
	/// Indicates full house, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairPlusFullHouse,

	/// <summary>
	/// Indicates full house, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenPairFullHouse,

	/// <summary>
	/// Indicates full house, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedPairFullHouse,

	/// <summary>
	/// Indicates full house, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenPairFullHouse,

	/// <summary>
	/// Indicates full house, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTripleFullHouse,

	/// <summary>
	/// Indicates full house, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTriplePlusFullHouse,

	/// <summary>
	/// Indicates full house, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenTripleFullHouse,

	/// <summary>
	/// Indicates full house, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedTripleFullHouse,

	/// <summary>
	/// Indicates full house, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenTripleFullHouse,

	/// <summary>
	/// Indicates full house, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadrupleFullHouse,

	/// <summary>
	/// Indicates full house, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadruplePlusFullHouse,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenQuadrupleFullHouse,

	/// <summary>
	/// Indicates crosshatching in block, with pointing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	PointingCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with claiming.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	ClaimingCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairPlusCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTriplePlusCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadrupleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadruplePlusCrosshatchingBlock,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenQuadrupleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in row, with pointing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	PointingCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with claiming.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	ClaimingCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairPlusCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTriplePlusCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadrupleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadruplePlusCrosshatchingRow,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenQuadrupleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in column, with pointing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	PointingCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with claiming.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	ClaimingCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairPlusCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTriplePlusCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadrupleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadruplePlusCrosshatchingColumn,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenQuadrupleCrosshatchingColumn,

	/// <summary>
	/// Indicates naked single, with pointing.
	/// </summary>
	[TechniqueMetadata(
		5.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	PointingNakedSingle,

	/// <summary>
	/// Indicates naked single, with claiming.
	/// </summary>
	[TechniqueMetadata(
		5.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectIntersectionStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	ClaimingNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedPairPlusNakedSingle,

	/// <summary>
	/// Indicates naked single, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedTriplePlusNakedSingle,

	/// <summary>
	/// Indicates naked single, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	LockedHiddenTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadrupleNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		3.3, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	NakedQuadruplePlusNakedSingle,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		3.7, DifficultyLevel.Moderate, TechniqueGroup.ComplexSingle, typeof(DirectSubsetStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [Size, Locked])]
	HiddenQuadrupleNakedSingle,
	#endregion

	//
	// Locked Candidates
	//
	#region Locked Candidates
	/// <summary>
	/// Indicates pointing.
	/// </summary>
	[Hodoku(50, HodokuDifficultyLevel.Medium, Prefix = "0100", Aliases = ["Locked Candidates Type 1"])]
	[SudokuExplainer(2.6, SudokuExplainerTechnique.Pointing)]
	[TechniqueMetadata(
		2.6, DifficultyLevel.Moderate, TechniqueGroup.LockedCandidates, typeof(LockedCandidatesStep),
		Links = ["http://sudopedia.enjoysudoku.com/Locked_Candidates.html"])]
	Pointing,

	/// <summary>
	/// Indicates claiming.
	/// </summary>
	[Hodoku(50, HodokuDifficultyLevel.Medium, Prefix = "0101", Aliases = ["Locked Candidates Type 2"])]
	[SudokuExplainer(2.8, SudokuExplainerTechnique.Claiming)]
	[TechniqueMetadata(
		2.8, DifficultyLevel.Moderate, TechniqueGroup.LockedCandidates, typeof(LockedCandidatesStep),
		Links = ["http://sudopedia.enjoysudoku.com/Locked_Candidates.html"])]
	Claiming,

	/// <summary>
	/// Indicates law of leftover.
	/// </summary>
	[TechniqueMetadata(
		2.0, DifficultyLevel.Moderate, TechniqueGroup.LockedCandidates, typeof(LawOfLeftoverStep),
		PencilmarkVisibility = PencilmarkVisibility.Direct, Abbreviation = "LoL",
		Features = TechniqueFeature.DirectTechniques, Links = ["http://sudopedia.enjoysudoku.com/Law_of_Leftovers.html"])]
	LawOfLeftover,
	#endregion

	//
	// Subsets
	//
	#region Subsets
	/// <summary>
	/// Indicates naked pair.
	/// </summary>
	[Hodoku(60, HodokuDifficultyLevel.Medium, Prefix = "0200")]
	[SudokuExplainer(3.0, SudokuExplainerTechnique.NakedPair)]
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Naked_Pair.html"])]
	NakedPair,

	/// <summary>
	/// Indicates naked pair plus (naked pair (+)).
	/// </summary>
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Naked_Pair.html"])]
	NakedPairPlus,

	/// <summary>
	/// Indicates locked pair.
	/// </summary>
	[Hodoku(40, HodokuDifficultyLevel.Medium, Prefix = "0110-1")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(2.0, SudokuExplainerTechnique.DirectHiddenPair, Aliases = ["Direct Hidden Pair"])]
#endif
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Locked_Pair.html"])]
	LockedPair,

	/// <summary>
	/// Indicates hidden pair.
	/// </summary>
	[Hodoku(70, HodokuDifficultyLevel.Medium, Prefix = "0210")]
	[SudokuExplainer(3.4, SudokuExplainerTechnique.HiddenPair)]
	[TechniqueMetadata(
		3.4, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(HiddenSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Hidden_Pair.html"])]
	HiddenPair,

	/// <summary>
	/// Indicates locked hidden pair.
	/// </summary>
	[Hodoku(Prefix = "0110-2")]
	[TechniqueMetadata(
		3.4, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(HiddenSubsetStep),
		ExtraFactors = [Size, Locked])]
	LockedHiddenPair,

	/// <summary>
	/// Indicates naked triple.
	/// </summary>
	[Hodoku(80, HodokuDifficultyLevel.Medium, Prefix = "0201")]
	[SudokuExplainer(3.6, SudokuExplainerTechnique.NakedTriplet, Aliases = ["Naked Triplet"])]
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Naked_Triple.html"])]
	NakedTriple,

	/// <summary>
	/// Indicates naked triple plus (naked triple (+)).
	/// </summary>
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Naked_Triple.html"])]
	NakedTriplePlus,

	/// <summary>
	/// Indicates locked triple.
	/// </summary>
	[Hodoku(60, HodokuDifficultyLevel.Medium, Prefix = "0111-1")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(2.5, SudokuExplainerTechnique.DirectHiddenTriplet, Aliases = ["Direct Hidden Triplet"])]
#endif
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Locked_Triple.html"])]
	LockedTriple,

	/// <summary>
	/// Indicates hidden triple.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Medium, Prefix = "0211")]
	[SudokuExplainer(4.0, SudokuExplainerTechnique.HiddenTriplet, Aliases = ["Hidden Triplet"])]
	[TechniqueMetadata(
		3.4, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(HiddenSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Hidden_Triple.html"])]
	HiddenTriple,

	/// <summary>
	/// Indicates locked hidden triple.
	/// </summary>
	[Hodoku(Prefix = "0111-2")]
	[TechniqueMetadata(
		3.4, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(HiddenSubsetStep),
		ExtraFactors = [Size, Locked])]
	LockedHiddenTriple,

	/// <summary>
	/// Indicates naked quadruple.
	/// </summary>
	[Hodoku(120, HodokuDifficultyLevel.Hard, Prefix = "0202")]
	[SudokuExplainer(5.0, SudokuExplainerTechnique.NakedQuad, Aliases = ["Naked Quad"])]
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Naked_Quad.html"])]
	NakedQuadruple,

	/// <summary>
	/// Indicates naked quadruple plus (naked quadruple (+)).
	/// </summary>
	[TechniqueMetadata(
		3.0, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(NakedSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Naked_Quad.html"])]
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates hidden quadruple.
	/// </summary>
	[Hodoku(150, HodokuDifficultyLevel.Hard, Prefix = "0212")]
	[SudokuExplainer(5.4, SudokuExplainerTechnique.HiddenQuad, Aliases = ["Hidden Quad"])]
	[TechniqueMetadata(
		3.4, DifficultyLevel.Moderate, TechniqueGroup.Subset, typeof(HiddenSubsetStep),
		ExtraFactors = [Size, Locked], Links = ["http://sudopedia.enjoysudoku.com/Hidden_Quad.html"])]
	HiddenQuadruple,
	#endregion

	//
	// Fishes
	//
	#region Fishes
	/// <summary>
	/// Indicates X-Wing.
	/// </summary>
	[Hodoku(140, HodokuDifficultyLevel.Hard, Prefix = "0300")]
	[SudokuExplainer(3.2, SudokuExplainerTechnique.XWing)]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = ["http://sudopedia.enjoysudoku.com/X-Wing.html", "http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	XWing,

	/// <summary>
	/// Indicates finned X-Wing.
	/// </summary>
	[Hodoku(130, HodokuDifficultyLevel.Hard, Prefix = "0310")]
	[SudokuExplainer(RatingValueAdvanced = [3.4])]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://sudopedia.enjoysudoku.com/Finned_X-Wing.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedXWing,

	/// <summary>
	/// Indicates sashimi X-Wing.
	/// </summary>
	[Hodoku(150, HodokuDifficultyLevel.Hard, Prefix = "0320")]
	[SudokuExplainer(RatingValueAdvanced = [3.5])]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_Fish.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_X-Wing.html"
		])]
	SashimiXWing,

	/// <summary>
	/// Indicates Siamese finned X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedXWing,

	/// <summary>
	/// Indicates Siamese sashimi X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi], Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiXWing,

	/// <summary>
	/// Indicates franken X-Wing.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0330")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenXWing,

	/// <summary>
	/// Indicates finned franken X-Wing.
	/// </summary>
	[Hodoku(390, HodokuDifficultyLevel.Unfair, Prefix = "0340")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	FinnedFrankenXWing,

	/// <summary>
	/// Indicates sashimi franken X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenXWing,

	/// <summary>
	/// Indicates Siamese finned franken X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	SiameseFinnedFrankenXWing,

	/// <summary>
	/// Indicates Siamese sashimi franken X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism], Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiFrankenXWing,

	/// <summary>
	/// Indicates mutant X-Wing.
	/// </summary>
	[Hodoku(450, HodokuDifficultyLevel.Extreme, Prefix = "0350")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantXWing,

	/// <summary>
	/// Indicates finned mutant X-Wing.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0360")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	FinnedMutantXWing,

	/// <summary>
	/// Indicates sashimi mutant X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantXWing,

	/// <summary>
	/// Indicates Siamese finned mutant X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	SiameseFinnedMutantXWing,

	/// <summary>
	/// Indicates Siamese sashimi mutant X-Wing.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantXWing,

	/// <summary>
	/// Indicates swordfish.
	/// </summary>
	[Hodoku(150, HodokuDifficultyLevel.Hard, Prefix = "0301")]
	[SudokuExplainer(3.8, SudokuExplainerTechnique.Swordfish)]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Swordfish.html"])]
	Swordfish,

	/// <summary>
	/// Indicates finned swordfish.
	/// </summary>
	[Hodoku(200, HodokuDifficultyLevel.Unfair, Prefix = "0311")]
	[SudokuExplainer(RatingValueAdvanced = [4.0])]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://sudopedia.enjoysudoku.com/Finned_Swordfish.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedSwordfish,

	/// <summary>
	/// Indicates sashimi swordfish.
	/// </summary>
	[Hodoku(240, HodokuDifficultyLevel.Unfair, Prefix = "0321")]
	[SudokuExplainer(RatingValueAdvanced = [4.1])]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_Fish.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_Swordfish.html"
		])]
	SashimiSwordfish,

	/// <summary>
	/// Indicates Siamese finned swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedSwordfish,

	/// <summary>
	/// Indicates Siamese sashimi swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi], Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiSwordfish,

	/// <summary>
	/// Indicates swordfish.
	/// </summary>
	[Hodoku(350, HodokuDifficultyLevel.Unfair, Prefix = "0331")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Swordfish.html"
		])]
	FrankenSwordfish,

	/// <summary>
	/// Indicates finned franken swordfish.
	/// </summary>
	[Hodoku(410, HodokuDifficultyLevel.Unfair, Prefix = "0341")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Swordfish.html"
		])]
	FinnedFrankenSwordfish,

	/// <summary>
	/// Indicates sashimi franken swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Swordfish.html"
		])]
	SashimiFrankenSwordfish,

	/// <summary>
	/// Indicates Siamese finned franken swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Swordfish.html"
		])]
	SiameseFinnedFrankenSwordfish,

	/// <summary>
	/// Indicates Siamese sashimi franken swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Swordfish.html"
		])]
	SiameseSashimiFrankenSwordfish,

	/// <summary>
	/// Indicates mutant swordfish.
	/// </summary>
	[Hodoku(450, HodokuDifficultyLevel.Extreme, Prefix = "0351")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	MutantSwordfish,

	/// <summary>
	/// Indicates finned mutant swordfish.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0361")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	FinnedMutantSwordfish,

	/// <summary>
	/// Indicates sashimi mutant swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	SashimiMutantSwordfish,

	/// <summary>
	/// Indicates Siamese finned mutant swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	SiameseFinnedMutantSwordfish,

	/// <summary>
	/// Indicates Siamese sashimi mutant swordfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	SiameseSashimiMutantSwordfish,

	/// <summary>
	/// Indicates jellyfish.
	/// </summary>
	[Hodoku(160, HodokuDifficultyLevel.Hard, Prefix = "0302")]
	[SudokuExplainer(5.2, SudokuExplainerTechnique.Jellyfish)]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Jellyfish.html"])]
	Jellyfish,

	/// <summary>
	/// Indicates finned jellyfish.
	/// </summary>
	[Hodoku(250, HodokuDifficultyLevel.Unfair, Prefix = "0312")]
	[SudokuExplainer(RatingValueAdvanced = [5.4])]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://sudopedia.enjoysudoku.com/Finned_Jellyfish.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedJellyfish,

	/// <summary>
	/// Indicates sashimi jellyfish.
	/// </summary>
	[Hodoku(260, HodokuDifficultyLevel.Unfair, Prefix = "0322")]
	[SudokuExplainer(RatingValueAdvanced = [5.6])]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_Fish.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_Jellyfish.html"
		])]
	SashimiJellyfish,

	/// <summary>
	/// Indicates Siamese finned jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedJellyfish,

	/// <summary>
	/// Indicates Siamese sashimi jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Hard, TechniqueGroup.NormalFish, typeof(NormalFishStep),
		ExtraFactors = [Size, Sashimi], Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiJellyfish,

	/// <summary>
	/// Indicates franken jellyfish.
	/// </summary>
	[Hodoku(370, HodokuDifficultyLevel.Unfair, Prefix = "0332")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	FrankenJellyfish,

	/// <summary>
	/// Indicates finned franken jellyfish.
	/// </summary>
	[Hodoku(430, HodokuDifficultyLevel.Unfair, Prefix = "0342")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	FinnedFrankenJellyfish,

	/// <summary>
	/// Indicates sashimi franken jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	SashimiFrankenJellyfish,

	/// <summary>
	/// Indicates Siamese finned franken jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	SiameseFinnedFrankenJellyfish,

	/// <summary>
	/// Indicates Siamese sashimi franken jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	SiameseSashimiFrankenJellyfish,

	/// <summary>
	/// Indicates mutant jellyfish.
	/// </summary>
	[Hodoku(450, HodokuDifficultyLevel.Extreme, Prefix = "0352")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	MutantJellyfish,

	/// <summary>
	/// Indicates finned mutant jellyfish.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0362")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	FinnedMutantJellyfish,

	/// <summary>
	/// Indicates sashimi mutant jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	SashimiMutantJellyfish,

	/// <summary>
	/// Indicates Siamese finned mutant jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	SiameseFinnedMutantJellyfish,

	/// <summary>
	/// Indicates Siamese sashimi mutant jellyfish.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Fiendish, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	SiameseSashimiMutantJellyfish,

	/// <summary>
	/// Indicates squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0303")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory, Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	Squirmbag,

	/// <summary>
	/// Indicates finned squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0313")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedSquirmbag,

	/// <summary>
	/// Indicates sashimi squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0323")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html"])]
	SashimiSquirmbag,

	/// <summary>
	/// Indicates Siamese finned squirmbag.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseFinnedSquirmbag,

	/// <summary>
	/// Indicates Siamese sashimi squirmbag.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html"])]
	SiameseSashimiSquirmbag,

	/// <summary>
	/// Indicates franken squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0333")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		Features = TechniqueFeature.OnlyExistInTheory, ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenSquirmbag,

	/// <summary>
	/// Indicates finned franken squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0343")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	FinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates sashimi franken squirmbag.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates Siamese finned franken squirmbag.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	SiameseFinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates Siamese sashimi franken squirmbag.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SiameseSashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates mutant squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0353")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		Features = TechniqueFeature.OnlyExistInTheory, ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantSquirmbag,

	/// <summary>
	/// Indicates finned mutant squirmbag.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0363")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	FinnedMutantSquirmbag,

	/// <summary>
	/// Indicates sashimi mutant squirmbag.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantSquirmbag,

	/// <summary>
	/// Indicates Siamese finned mutant squirmbag.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	SiameseFinnedMutantSquirmbag,

	/// <summary>
	/// Indicates Siamese sashimi mutant squirmbag.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantSquirmbag,

	/// <summary>
	/// Indicates whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0304")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory, Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	Whale,

	/// <summary>
	/// Indicates finned whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0314")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedWhale,

	/// <summary>
	/// Indicates sashimi whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0324")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html"])]
	SashimiWhale,

	/// <summary>
	/// Indicates Siamese finned whale.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseFinnedWhale,

	/// <summary>
	/// Indicates Siamese sashimi whale.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseSashimiWhale,

	/// <summary>
	/// Indicates franken whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0334")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		Features = TechniqueFeature.OnlyExistInTheory, ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenWhale,

	/// <summary>
	/// Indicates finned franken whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0344")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	FinnedFrankenWhale,

	/// <summary>
	/// Indicates sashimi franken whale.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenWhale,

	/// <summary>
	/// Indicates Siamese finned franken whale.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	SiameseFinnedFrankenWhale,

	/// <summary>
	/// Indicates Siamese sashimi franken whale.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SiameseSashimiFrankenWhale,

	/// <summary>
	/// Indicates mutant whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0354")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		Features = TechniqueFeature.OnlyExistInTheory, ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantWhale,

	/// <summary>
	/// Indicates finned mutant whale.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0364")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	FinnedMutantWhale,

	/// <summary>
	/// Indicates sashimi mutant whale.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantWhale,

	/// <summary>
	/// Indicates Siamese finned mutant whale.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	SiameseFinnedMutantWhale,

	/// <summary>
	/// Indicates Siamese sashimi mutant whale.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantWhale,

	/// <summary>
	/// Indicates leviathan.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0305")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory, Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	Leviathan,

	/// <summary>
	/// Indicates finned leviathan.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0315")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedLeviathan,

	/// <summary>
	/// Indicates sashimi leviathan.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Unfair, Prefix = "0325")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html",])]
	SashimiLeviathan,

	/// <summary>
	/// Indicates Siamese finned leviathan.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedLeviathan,

	/// <summary>
	/// Indicates Siamese sashimi leviathan.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory, Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiLeviathan,

	/// <summary>
	/// Indicates franken leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		Features = TechniqueFeature.OnlyExistInTheory, ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0335")]
	FrankenLeviathan,

	/// <summary>
	/// Indicates finned franken leviathan.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0345")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	FinnedFrankenLeviathan,

	/// <summary>
	/// Indicates sashimi franken leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenLeviathan,

	/// <summary>
	/// Indicates Siamese finned franken leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	SiameseFinnedFrankenLeviathan,

	/// <summary>
	/// Indicates Siamese sashimi franken leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SiameseSashimiFrankenLeviathan,

	/// <summary>
	/// Indicates mutant leviathan.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0355")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		Features = TechniqueFeature.OnlyExistInTheory, ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantLeviathan,

	/// <summary>
	/// Indicates finned mutant leviathan.
	/// </summary>
	[Hodoku(470, HodokuDifficultyLevel.Extreme, Prefix = "0365")]
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	FinnedMutantLeviathan,

	/// <summary>
	/// Indicates sashimi mutant leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantLeviathan,

	/// <summary>
	/// Indicates Siamese finned mutant leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	SiameseFinnedMutantLeviathan,

	/// <summary>
	/// Indicates Siamese sashimi mutant leviathan.
	/// </summary>
	[TechniqueMetadata(
		3.2, DifficultyLevel.Nightmare, TechniqueGroup.ComplexFish, typeof(ComplexFishStep),
		ExtraFactors = [Size, Sashimi, FishShape, Cannibalism],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantLeviathan,
	#endregion

	//
	// Regular Wings
	//
	#region Regular Wings
	/// <summary>
	/// Indicates XY-Wing.
	/// </summary>
	[Hodoku(160, HodokuDifficultyLevel.Hard, Prefix = "0800")]
	[SudokuExplainer(4.2, SudokuExplainerTechnique.XyWing)]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Hard, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		ExtraFactors = [Size, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/XY-Wing.html"])]
	XyWing,

	/// <summary>
	/// Indicates XYZ-Wing.
	/// </summary>
	[Hodoku(180, HodokuDifficultyLevel.Hard, Prefix = "0801")]
	[SudokuExplainer(4.4, SudokuExplainerTechnique.XyzWing)]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Hard, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		ExtraFactors = [Size, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/XYZ-Wing.html"])]
	XyzWing,

	/// <summary>
	/// Indicates WXYZ-Wing.
	/// </summary>
	[Hodoku(Prefix = "0802")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.XyzWing, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Hard, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		ExtraFactors = [Size, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/WXYZ-Wing.html"])]
	WxyzWing,

	/// <summary>
	/// Indicates VWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	VwxyzWing,

	/// <summary>
	/// Indicates UVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	UvwxyzWing,

	/// <summary>
	/// Indicates TUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	TuvwxyzWing,

	/// <summary>
	/// Indicates STUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	StuvwxyzWing,

	/// <summary>
	/// Indicates RSTUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	RstuvwxyzWing,

	/// <summary>
	/// Indicates incomplete WXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Hard, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		ExtraFactors = [Size, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/WXYZ-Wing.html"])]
	IncompleteWxyzWing,

	/// <summary>
	/// Indicates incomplete VWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		ExtraFactors = [Size, Incompleteness])]
	IncompleteVwxyzWing,

	/// <summary>
	/// Indicates incomplete UVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	IncompleteUvwxyzWing,

	/// <summary>
	/// Indicates incomplete TUVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	IncompleteTuvwxyzWing,

	/// <summary>
	/// Indicates incomplete STUVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	IncompleteStuvwxyzWing,

	/// <summary>
	/// Indicates incomplete RSTUVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Fiendish, TechniqueGroup.RegularWing, typeof(RegularWingStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size, Incompleteness])]
	IncompleteRstuvwxyzWing,
	#endregion

	//
	// Irregular Wings
	//
	#region Irregular Wings
	/// <summary>
	/// Indicates W-Wing.
	/// </summary>
	[Hodoku(150, HodokuDifficultyLevel.Hard, Prefix = "0803")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.WWing, RatingValueAdvanced = [4.4])]
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(WWingStep),
		Links = ["http://sudopedia.enjoysudoku.com/W-Wing.html"])]
	WWing,

	/// <summary>
	/// Indicates Multi-Branch W-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(MultiBranchWWingStep),
		ExtraFactors = [Size], Links = ["http://sudopedia.enjoysudoku.com/W-Wing.html"])]
	MultiBranchWWing,

	/// <summary>
	/// Indicates grouped W-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(WWingStep),
		Links = ["http://sudopedia.enjoysudoku.com/W-Wing.html"])]
	GroupedWWing,

	/// <summary>
	/// Indicates M-Wing.
	/// </summary>
	[TechniqueMetadata(4.5, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(MWingStep))]
	MWing,

	/// <summary>
	/// Indicates grouped M-Wing.
	/// </summary>
	[TechniqueMetadata(4.5, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(MWingStep))]
	GroupedMWing,

	/// <summary>
	/// Indicates S-Wing.
	/// </summary>
	[TechniqueMetadata(4.7, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(SWingStep))]
	SWing,

	/// <summary>
	/// Indicates grouped S-Wing.
	/// </summary>
	[TechniqueMetadata(4.7, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(SWingStep))]
	GroupedSWing,

	/// <summary>
	/// Indicates local wing.
	/// </summary>
	[TechniqueMetadata(
		4.8, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(LWingStep),
		Links = ["http://forum.enjoysudoku.com/local-wing-t34685.html"])]
	LWing,

	/// <summary>
	/// Indicates grouped local wing.
	/// </summary>
	[TechniqueMetadata(
		4.8, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(LWingStep),
		Links = ["http://forum.enjoysudoku.com/local-wing-t34685.html"])]
	GroupedLWing,

	/// <summary>
	/// Indicates hybrid wing.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(HWingStep),
		Links = ["http://forum.enjoysudoku.com/hybrid-wings-work-in-progress-t34212.html"])]
	HWing,

	/// <summary>
	/// Indicates grouped hybrid wing.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Hard, TechniqueGroup.IrregularWing, typeof(HWingStep),
		Links = ["http://forum.enjoysudoku.com/hybrid-wings-work-in-progress-t34212.html"])]
	GroupedHWing,
	#endregion

	//
	// Almost Locked Candidates
	//
	#region Almost Locked Candidates
	/// <summary>
	/// Indicates almost locked pair.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [4.5])]
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AlmostLockedCandidates, typeof(AlmostLockedCandidatesStep),
		Abbreviation = "ALP", ExtraFactors = [Size, ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedPair,

	/// <summary>
	/// Indicates almost locked triple.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [5.2])]
	[TechniqueMetadata(
		5.2, DifficultyLevel.Hard, TechniqueGroup.AlmostLockedCandidates, typeof(AlmostLockedCandidatesStep),
		Abbreviation = "ALT", ExtraFactors = [Size, ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedTriple,

	/// <summary>
	/// Indicates almost locked quadruple.
	/// The technique may not be useful because it'll be replaced with Sue de Coq.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AlmostLockedCandidates, typeof(AlmostLockedCandidatesStep),
		Abbreviation = "ALQ", ExtraFactors = [Size, ValueCell], Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedQuadruple,

	/// <summary>
	/// Indicates almost locked triple value type.
	/// The technique may not be often used because it'll be replaced with some kinds of Sue de Coq.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AlmostLockedCandidates, typeof(AlmostLockedCandidatesStep),
		ExtraFactors = [Size, ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedTripleValueType,

	/// <summary>
	/// Indicates almost locked quadruple value type.
	/// The technique may not be often used because it'll be replaced with some kinds of Sue de Coq.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AlmostLockedCandidates, typeof(AlmostLockedCandidatesStep),
		ExtraFactors = [Size, ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedQuadrupleValueType,
	#endregion

	//
	// Extended Subset Principle
	//
	#region Extended Subset Principle
	/// <summary>
	/// Indicates extended subset principle.
	/// </summary>
	[Hodoku(Prefix = "1102", Aliases = ["Subset Counting"])]
	[TechniqueMetadata(
		5.5, DifficultyLevel.Fiendish, TechniqueGroup.ExtendedSubsetPrinciple, typeof(ExtendedSubsetPrincipleStep),
		ExtraFactors = [Size], Abbreviation = "ESP",
		Links = ["http://sudopedia.enjoysudoku.com/Subset_Counting.html", "http://forum.enjoysudoku.com/viewtopic.php?t=3479"])]
	ExtendedSubsetPrinciple,
	#endregion

	//
	// Unique Rectangle
	//
	#region Unique Rectangle
	/// <summary>
	/// Indicates unique rectangle type 1.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0600", Aliases = ["Uniqueness Test 1"])]
	[SudokuExplainer(4.5, SudokuExplainerTechnique.UniqueRectangle)]
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleType1Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2000",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType1,

	/// <summary>
	/// Indicates unique rectangle type 2.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0601", Aliases = ["Uniqueness Test 2"])]
	[SudokuExplainer(4.5, SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleType2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2000",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType2,

	/// <summary>
	/// Indicates unique rectangle type 3.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0602", Aliases = ["Uniqueness Test 3"])]
	[SudokuExplainer(
		techniqueDefined: SudokuExplainerTechnique.UniqueRectangle,
		RatingValueOriginal = [4.5, 4.8], RatingValueAdvanced = [4.6, 4.9])]
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleType3Step),
		ExtraFactors = [Size, Hidden],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2000",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType3,

	/// <summary>
	/// Indicates unique rectangle type 4.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0603", Aliases = ["Uniqueness Test 4"])]
	[SudokuExplainer(4.5, SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2000",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType4,

	/// <summary>
	/// Indicates unique rectangle type 5.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0604", Aliases = ["Uniqueness Test 5"])]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleType2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType5,

	/// <summary>
	/// Indicates unique rectangle type 6.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0605", Aliases = ["Uniqueness Test 6"])]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType6,

	/// <summary>
	/// Indicates hidden unique rectangle.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0606", Aliases = ["Hidden Rectangle"])]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.8])]
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(HiddenUniqueRectangleStep),
		Abbreviation = "HUR", SecondarySupportedType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	HiddenUniqueRectangle,

	/// <summary>
	/// Indicates unique rectangle + 2D.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangle2DOr3XStep),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle2D,

	/// <summary>
	/// Indicates unique rectangle + 2B / 1SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle2B1,

	/// <summary>
	/// Indicates unique rectangle + 2D / 1SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle2D1,

	/// <summary>
	/// Indicates unique rectangle + 3X.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangle2DOr3XStep),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3X,

	/// <summary>
	/// Indicates unique rectangle + 3x / 1SL.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.UniqueRectangle,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3X1L,

	/// <summary>
	/// Indicates unique rectangle + 3X / 1SL.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.UniqueRectangle,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3X1U,

	/// <summary>
	/// Indicates unique rectangle + 3X / 2SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3X2,

	/// <summary>
	/// Indicates unique rectangle + 3N / 2SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3N2,

	/// <summary>
	/// Indicates unique rectangle + 3U / 2SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3U2,

	/// <summary>
	/// Indicates unique rectangle + 3E / 2SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle3E2,

	/// <summary>
	/// Indicates unique rectangle + 4x / 1SL.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.UniqueRectangle,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4X1L,

	/// <summary>
	/// Indicates unique rectangle + 4X / 1SL.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.UniqueRectangle,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4X1U,

	/// <summary>
	/// Indicates unique rectangle + 4x / 2SL.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.UniqueRectangle,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4X2L,

	/// <summary>
	/// Indicates unique rectangle + 4X / 2SL.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.UniqueRectangle,
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4X2U,

	/// <summary>
	/// Indicates unique rectangle + 4X / 3SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4X3,

	/// <summary>
	/// Indicates unique rectangle + 4C / 3SL.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4C3,

	/// <summary>
	/// Indicates unique rectangle-XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [Avoidable, WingSize],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleXyWing,

	/// <summary>
	/// Indicates unique rectangle-XYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [Avoidable, WingSize],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleXyzWing,

	/// <summary>
	/// Indicates unique rectangle-WXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [Avoidable, WingSize],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleWxyzWing,

	/// <summary>
	/// Indicates unique rectangle W-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle,
#if UNIQUE_RECTANGLE_W_WING
		typeof(UniqueRectangleWWingStep),
#endif
#if !UNIQUE_RECTANGLE_W_WING
		Features = TechniqueFeature.NotImplemented,
#endif
		ExtraFactors = [Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleWWing,

	/// <summary>
	/// Indicates unique rectangle sue de coq.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithSueDeCoqStep),
		ExtraFactors = [Size, Isolated, Cannibalism, Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleSueDeCoq,

	/// <summary>
	/// Indicates unique rectangle baba grouping.
	/// </summary>
	[TechniqueMetadata(
		4.9, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWithBabaGroupingStep),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleBabaGrouping,

	/// <summary>
	/// Indicates unique rectangle external type 1.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalType1Or2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType1,

	/// <summary>
	/// Indicates unique rectangle external type 2.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalType1Or2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType2,

	/// <summary>
	/// Indicates unique rectangle external type 3.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalType3Step),
		ExtraFactors = [Size, Avoidable, Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType3,

	/// <summary>
	/// Indicates unique rectangle external type 4.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalType4Step),
		ExtraFactors = [Avoidable, Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType4,

	/// <summary>
	/// Indicates unique rectangle external turbot fish.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalTurbotFishStep),
		ExtraFactors = [Guardian, Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalTurbotFish,

	/// <summary>
	/// Indicates unique rectangle external W-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.8, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalWWingStep),
		ExtraFactors = [Guardian, Avoidable, Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalWWing,

	/// <summary>
	/// Indicates unique rectangle external XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalXyWingStep),
		ExtraFactors = [Guardian, Avoidable, Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalXyWing,

	/// <summary>
	/// Indicates unique rectangle external almost locked sets XZ rule.
	/// </summary>
	[TechniqueMetadata(
		4.8, DifficultyLevel.Fiendish, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleExternalAlmostLockedSetsXzStep),
		ExtraFactors = [Guardian, Avoidable, Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalAlmostLockedSetsXz,
	#endregion

	//
	// Avoidable Rectangle
	//
	#region Avoidable Rectangle
	/// <summary>
	/// Indicates avoidable rectangle type 1.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0607")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.AvoidableRectangle, RatingValueAdvanced = [4.7])] // I think this difficulty may be a mistake.
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleType1Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType1,

	/// <summary>
	/// Indicates avoidable rectangle type 2.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0608")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.AvoidableRectangle, RatingValueAdvanced = [4.5])] // I think this difficulty may be a mistake.
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleType2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType2,

	/// <summary>
	/// Indicates avoidable rectangle type 3.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleType3Step),
		ExtraFactors = [Size, Hidden], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType3,

	/// <summary>
	/// Indicates avoidable rectangle type 5.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleType2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType5,

	/// <summary>
	/// Indicates hidden avoidable rectangle.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(HiddenUniqueRectangleStep),
		Abbreviation = "HAR", SecondarySupportedType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ConjugatePair, Avoidable], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	HiddenAvoidableRectangle,

	/// <summary>
	/// Indicates avoidable rectangle + 2D.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangle2DOr3XStep),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangle2D,

	/// <summary>
	/// Indicates avoidable rectangle + 3X.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangle2DOr3XStep),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangle3X,

	/// <summary>
	/// Indicates avoidable rectangle XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [Avoidable, WingSize], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleXyWing,

	/// <summary>
	/// Indicates avoidable rectangle XYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [Avoidable, WingSize], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleXyzWing,

	/// <summary>
	/// Indicates avoidable rectangle WXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [Avoidable, WingSize], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleWxyzWing,

	/// <summary>
	/// Indicates avoidable rectangle W-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle,
#if UNIQUE_RECTANGLE_W_WING
		typeof(UniqueRectangleWWingStep),
#endif
#if !UNIQUE_RECTANGLE_W_WING
		Features = TechniqueFeature.NotImplemented,
#endif
		ExtraFactors = [Avoidable], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleWWing,

	/// <summary>
	/// Indicates avoidable rectangle sue de coq.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleWithSueDeCoqStep),
		ExtraFactors = [Size, Isolated, Cannibalism, Avoidable], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleSueDeCoq,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in block.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(AvoidableRectangleWithHiddenSingleStep),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleHiddenSingleBlock,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in row.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(AvoidableRectangleWithHiddenSingleStep),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleHiddenSingleRow,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in column.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(AvoidableRectangleWithHiddenSingleStep),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleHiddenSingleColumn,

	/// <summary>
	/// Indicates avoidable rectangle external type 1.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalType1Or2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType1,

	/// <summary>
	/// Indicates avoidable rectangle external type 2.
	/// </summary>
	[TechniqueMetadata(
		4.5, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalType1Or2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType2,

	/// <summary>
	/// Indicates avoidable rectangle external type 3.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalType3Step),
		ExtraFactors = [Size, Avoidable, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType3,

	/// <summary>
	/// Indicates avoidable rectangle external type 4.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalType4Step),
		ExtraFactors = [Avoidable, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType4,

	/// <summary>
	/// Indicates avoidable rectangle external XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.7, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalXyWingStep),
		ExtraFactors = [Guardian, Avoidable, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalXyWing,

	/// <summary>
	/// Indicates avoidable rectangle external W-Wing.
	/// </summary>
	[TechniqueMetadata(
		4.8, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalWWingStep),
		Features = TechniqueFeature.NotImplemented, ExtraFactors = [Guardian, Avoidable, Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalWWing,

	/// <summary>
	/// Indicates avoidable rectangle external almost locked sets XZ rule.
	/// </summary>
	[TechniqueMetadata(
		4.8, DifficultyLevel.Fiendish, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleExternalAlmostLockedSetsXzStep),
		ExtraFactors = [Guardian, Avoidable, Incompleteness], Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalAlmostLockedSetsXz,
	#endregion

	//
	// Unique Loop
	//
	#region Unique Loop
	/// <summary>
	/// Indicates unique loop type 1.
	/// </summary>
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.UniqueLoop, RatingValueOriginal = [4.6, 5.0])]
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueLoop, typeof(UniqueLoopType1Step),
		ExtraFactors = [Length], Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType1,

	/// <summary>
	/// Indicates unique loop type 2.
	/// </summary>
	[SudokuExplainer(
		techniqueDefined: SudokuExplainerTechnique.UniqueLoop,
		RatingValueOriginal = [4.6, 5.0], RatingValueAdvanced = [4.7, 5.1])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.UniqueLoop, typeof(UniqueLoopType2Step),
		ExtraFactors = [Length], Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType2,

	/// <summary>
	/// Indicates unique loop type 3.
	/// </summary>
	[SudokuExplainer(
		techniqueDefined: SudokuExplainerTechnique.UniqueLoop,
		RatingValueOriginal = [4.6, 5.0], RatingValueAdvanced = [4.7, 5.1])]
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueLoop, typeof(UniqueLoopType3Step),
		ExtraFactors = [Length, Size], Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType3,

	/// <summary>
	/// Indicates unique loop type 4.
	/// </summary>
	[SudokuExplainer(
		techniqueDefined: SudokuExplainerTechnique.UniqueLoop,
		RatingValueOriginal = [4.6, 5.0], RatingValueAdvanced = [4.7, 5.1])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.UniqueLoop, typeof(UniqueLoopType4Step),
		ExtraFactors = [Length], Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType4,
	#endregion

	//
	// Extended Rectangle
	//
	#region Extended Rectangle
	/// <summary>
	/// Indicates extended rectangle type 1.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0620")]
#endif
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.ExtendedRectangle, typeof(ExtendedRectangleType1Step),
		ExtraFactors = [Size])]
	ExtendedRectangleType1,

	/// <summary>
	/// Indicates extended rectangle type 2.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0621")]
#endif
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.ExtendedRectangle, typeof(ExtendedRectangleType2Step),
		ExtraFactors = [Size])]
	ExtendedRectangleType2,

	/// <summary>
	/// Indicates extended rectangle type 3.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0622")]
#endif
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.ExtendedRectangle, typeof(ExtendedRectangleType3Step),
		ExtraFactors = [Size, ExtraDigit])]
	ExtendedRectangleType3,

	/// <summary>
	/// Indicates extended rectangle type 4.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0623")]
#endif
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.ExtendedRectangle, typeof(ExtendedRectangleType4Step),
		ExtraFactors = [Size])]
	ExtendedRectangleType4,
	#endregion

	//
	// Bivalue Universal Grave
	//
	#region Bivalue Universal Grave
	/// <summary>
	/// Indicates bi-value universal grave type 1.
	/// </summary>
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0610", Aliases = ["Bivalue Universal Grave + 1"])]
	[SudokuExplainer(5.6, SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType1Step),
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates bi-value universal grave type 2.
	/// </summary>
	[SudokuExplainer(5.7, SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType2Step),
		ExtraFactors = [ExtraDigit],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates bi-value universal grave type 3.
	/// </summary>
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.BivalueUniversalGrave, RatingValueOriginal = [5.8, 6.1])]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType3Step),
		ExtraFactors = [Size, Hidden],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates bi-value universal grave type 4.
	/// </summary>
	[SudokuExplainer(5.7, SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType4Step),
		ExtraFactors = [ConjugatePair],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType4,

	/// <summary>
	/// Indicates bi-value universal grave + n.
	/// </summary>
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.BivalueUniversalGravePlusN, RatingValueAdvanced = [5.7])]
	[TechniqueMetadata(
		5.7, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveMultipleStep),
		Abbreviation = "BUG + n", ExtraFactors = [Size],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Indicates bi-value universal grave false candidate type.
	/// </summary>
	[TechniqueMetadata(
		5.7, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveFalseCandidateTypeStep),
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveFalseCandidateType,

	/// <summary>
	/// Indicates bi-value universal grave + n with forcing chains.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Nightmare, containingGroup: TechniqueGroup.BivalueUniversalGrave,
		Features = TechniqueFeature.NotImplemented,
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGravePlusNForcingChains,

	/// <summary>
	/// Indicates bi-value universal grave XZ rule.
	/// </summary>
	[TechniqueMetadata(
		5.8, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveXzStep),
		Abbreviation = "BUG-XZ",
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveXzRule,

	/// <summary>
	/// Indicates bi-value universal grave XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Hard, containingGroup: TechniqueGroup.BivalueUniversalGrave,
		Abbreviation = "BUG-XY-Wing",
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveXyWing,
	#endregion

	//
	// Reverse Bivalue Universal Grave
	//
	#region Reverse Bivalue Universal Grave
	/// <summary>
	/// Indicates reverse bi-value universal grave type 1.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType1Step),
		ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType1,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 2.
	/// </summary>
	[TechniqueMetadata(
		6.1, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType2Step),
		ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType2,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 3.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType3Step),
		ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType3,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 4.
	/// </summary>
	[TechniqueMetadata(
		6.3, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType4Step),
		ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType4,
	#endregion

	//
	// Uniqueness Clue Cover
	//
	#region Uniqueness Clue Cover
	/// <summary>
	/// Indicates uniqueness clue cover.
	/// </summary>
	[TechniqueMetadata(
		6.5, DifficultyLevel.Fiendish, TechniqueGroup.UniquenessClueCover, typeof(UniquenessClueCoverStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size],
		Links = ["http://sudopedia.enjoysudoku.com/Uniqueness_Clue_Cover.html", "http://forum.enjoysudoku.com/uniqueness-clue-cover-t40814.html"])]
	UniquenessClueCover,
	#endregion

	//
	// RW's Theory
	//
	#region RW's Theory
	/// <summary>
	/// Indicates RW's deadly pattern.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.RwDeadlyPattern,
		Features = TechniqueFeature.NotImplemented, Links = ["http://forum.enjoysudoku.com/yet-another-crazy-uniqueness-technique-t5589.html"])]
	RwDeadlyPattern,
	#endregion

	//
	// Borescoper's Deadly Pattern
	//
	#region Borescoper's Deadly Pattern
	/// <summary>
	/// Indicates Borescoper's deadly pattern type 1.
	/// </summary>
	[TechniqueMetadata(5.3, DifficultyLevel.Hard, TechniqueGroup.BorescoperDeadlyPattern, typeof(BorescoperDeadlyPatternType1Step))]
	BorescoperDeadlyPatternType1,

	/// <summary>
	/// Indicates Borescoper's deadly pattern type 2.
	/// </summary>
	[TechniqueMetadata(5.5, DifficultyLevel.Hard, TechniqueGroup.BorescoperDeadlyPattern, typeof(BorescoperDeadlyPatternType2Step))]
	BorescoperDeadlyPatternType2,

	/// <summary>
	/// Indicates Borescoper's deadly pattern type 3.
	/// </summary>
	[TechniqueMetadata(
		5.3, DifficultyLevel.Hard, TechniqueGroup.BorescoperDeadlyPattern, typeof(BorescoperDeadlyPatternType3Step),
		ExtraFactors = [Size])]
	BorescoperDeadlyPatternType3,

	/// <summary>
	/// Indicates Borescoper's deadly pattern type 4.
	/// </summary>
	[TechniqueMetadata(5.5, DifficultyLevel.Hard, TechniqueGroup.BorescoperDeadlyPattern, typeof(BorescoperDeadlyPatternType4Step))]
	BorescoperDeadlyPatternType4,
	#endregion

	//
	// Qiu's Deadly Pattern
	//
	#region Qiu's Deadly Pattern
	/// <summary>
	/// Indicates Qiu's deadly pattern type 1.
	/// </summary>
	[TechniqueMetadata(
		5.8, DifficultyLevel.Fiendish, TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternType1Step),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 2.
	/// </summary>
	[TechniqueMetadata(
		5.9, DifficultyLevel.Fiendish, TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternType2Step),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType2,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 3.
	/// </summary>
	[TechniqueMetadata(
		5.8, DifficultyLevel.Fiendish, TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternType3Step),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size],
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType3,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 4.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternType4Step),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType4,

	/// <summary>
	/// Indicates locked Qiu's deadly pattern.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternLockedTypeStep),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	LockedQiuDeadlyPattern,

	/// <summary>
	/// Indicates Qiu's deadly pattern external type 1.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternExternalType1Step),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternExternalType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern external type 2.
	/// </summary>
	[TechniqueMetadata(
		6.1, DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternExternalType2Step),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternExternalType2,
	#endregion

	//
	// Unique Matrix
	//
	#region Unique Matrix
	/// <summary>
	/// Indicates unique matrix type 1.
	/// </summary>
	[TechniqueMetadata(
		5.3, DifficultyLevel.Fiendish, TechniqueGroup.UniqueMatrix, typeof(UniqueMatrixType1Step),
		Features = TechniqueFeature.HardToBeGenerated)]
	UniqueMatrixType1,

	/// <summary>
	/// Indicates unique matrix type 2.
	/// </summary>
	[TechniqueMetadata(
		5.4, DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.UniqueMatrix, typeof(UniqueMatrixType2Step),
		Features = TechniqueFeature.HardToBeGenerated)]
	UniqueMatrixType2,

	/// <summary>
	/// Indicates unique matrix type 3.
	/// </summary>
	[TechniqueMetadata(
		5.3, DifficultyLevel.Fiendish, TechniqueGroup.UniqueMatrix, typeof(UniqueMatrixType3Step),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size])]
	UniqueMatrixType3,

	/// <summary>
	/// Indicates unique matrix type 4.
	/// </summary>
	[TechniqueMetadata(
		5.5, DifficultyLevel.Fiendish, TechniqueGroup.UniqueMatrix, typeof(UniqueMatrixType4Step),
		Features = TechniqueFeature.HardToBeGenerated)]
	UniqueMatrixType4,
	#endregion

	//
	// Sue de Coq
	//
	#region Sue de Coq
	/// <summary>
	/// Indicates sue de coq.
	/// </summary>
	[Hodoku(250, HodokuDifficultyLevel.Unfair, Prefix = "1101")]
	[SudokuExplainer(RatingValueAdvanced = [5.0])]
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoqStep),
		Abbreviation = "SdC", ExtraFactors = [Isolated, Cannibalism],
		Links = [
			"http://sudopedia.enjoysudoku.com/Sue_de_Coq.html",
			"http://forum.enjoysudoku.com/two-sector-disjoint-subsets-t2033.html",
			"http://forum.enjoysudoku.com/benchmark-sudoku-list-t3834-15.html#p43170"
		])]
	SueDeCoq,

	/// <summary>
	/// Indicates sue de coq with isolated digit.
	/// </summary>
	[Hodoku(250, HodokuDifficultyLevel.Unfair, Prefix = "1101")]
	[SudokuExplainer(RatingValueAdvanced = [5.0])]
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoqStep),
		ExtraFactors = [Isolated, Cannibalism],
		Links = [
			"http://sudopedia.enjoysudoku.com/Sue_de_Coq.html",
			"http://forum.enjoysudoku.com/two-sector-disjoint-subsets-t2033.html",
			"http://forum.enjoysudoku.com/benchmark-sudoku-list-t3834-15.html#p43170"
		])]
	SueDeCoqIsolated,

	/// <summary>
	/// Indicates 3-dimensional sue de coq.
	/// </summary>
	[TechniqueMetadata(
		5.5, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoq3DimensionStep),
		Links = ["http://sudopedia.enjoysudoku.com/Sue_de_Coq.html"])]
	SueDeCoq3Dimension,

	/// <summary>
	/// Indicates sue de coq cannibalism.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoqStep),
		ExtraFactors = [Isolated, Cannibalism], Links = ["http://sudopedia.enjoysudoku.com/Sue_de_Coq.html"])]
	SueDeCoqCannibalism,
	#endregion

	//
	// Fireworks
	//
	#region Fireworks
	/// <summary>
	/// Indicates firework triple.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.Firework, typeof(FireworkTripleStep),
		Links = ["http://forum.enjoysudoku.com/fireworks-t39513.html"])]
	FireworkTriple,

	/// <summary>
	/// Indicates firework quadruple.
	/// </summary>
	[TechniqueMetadata(
		6.3, DifficultyLevel.Fiendish, TechniqueGroup.Firework, typeof(FireworkQuadrupleStep),
		Links = ["http://forum.enjoysudoku.com/fireworks-t39513.html"])]
	FireworkQuadruple,
	#endregion

	//
	// Broken Wing
	//
	#region Broken Wing
	/// <summary>
	/// Indicates broken wing.
	/// </summary>
	[Hodoku(Prefix = "0705")]
	[TechniqueMetadata(
		5.5, DifficultyLevel.Fiendish, TechniqueGroup.BrokenWing, typeof(GuardianStep),
		Links = ["http://sudopedia.enjoysudoku.com/Broken_Wing.html"])]
	BrokenWing,
	#endregion

	//
	// Bi-value Oddagon
	//
	#region Bivalue Oddagon
	/// <summary>
	/// Indicates bi-value oddagon type 2.
	/// </summary>
	[TechniqueMetadata(
		6.1, DifficultyLevel.Fiendish, TechniqueGroup.BivalueOddagon, typeof(BivalueOddagonType2Step),
		Links = ["http://forum.enjoysudoku.com/technique-share-odd-bivalue-loop-bivalue-oddagon-t33153.html"])]
	BivalueOddagonType2,

	/// <summary>
	/// Indicates bi-value oddagon type 3.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.BivalueOddagon, typeof(BivalueOddagonType3Step),
		ExtraFactors = [Size], Links = ["http://forum.enjoysudoku.com/technique-share-odd-bivalue-loop-bivalue-oddagon-t33153.html"])]
	BivalueOddagonType3,
	#endregion

	//
	// Chromatic Pattern
	//
	#region Chromatic Pattern
	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 1.
	/// </summary>
	[TechniqueMetadata(
		6.5, DifficultyLevel.Fiendish, TechniqueGroup.RankTheory, typeof(ChromaticPatternType1Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType1,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 2.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.RankTheory,
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType2,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 3.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.RankTheory,
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType3,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 4.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.RankTheory,
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType4,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) XZ rule.
	/// </summary>
	[TechniqueMetadata(
		6.7, DifficultyLevel.Fiendish, TechniqueGroup.RankTheory, typeof(ChromaticPatternXzStep),
		Features = TechniqueFeature.HardToBeGenerated)]
	ChromaticPatternXzRule,
	#endregion

	//
	// Single Digit Pattern
	//
	#region Single Digit Pattern
	/// <summary>
	/// Indicates skyscraper.
	/// </summary>
	[Hodoku(130, HodokuDifficultyLevel.Hard, Prefix = "0400")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(6.6, SudokuExplainerTechnique.TurbotFish, Aliases = ["Turbot Fish"])]
#endif
	[TechniqueMetadata(
		4.0, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/Skyscraper.html"])]
	Skyscraper,

	/// <summary>
	/// Indicates two-string kite.
	/// </summary>
	[Hodoku(150, HodokuDifficultyLevel.Hard, Prefix = "0401")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(6.6, SudokuExplainerTechnique.TurbotFish, Aliases = ["Turbot Fish"])]
#endif
	[TechniqueMetadata(
		4.1, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/2-String_Kite.html"])]
	TwoStringKite,

	/// <summary>
	/// Indicates turbot fish.
	/// </summary>
	[Hodoku(120, HodokuDifficultyLevel.Hard, Prefix = "0403")]
	[SudokuExplainer(6.6, SudokuExplainerTechnique.TurbotFish)]
	[TechniqueMetadata(
		4.2, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep),
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?t=833"])]
	TurbotFish,

	/// <summary>
	/// Indicates grouped skyscraper.
	/// </summary>
	[TechniqueMetadata(
		4.2, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/Skyscraper.html"])]
	GroupedSkyscraper,

	/// <summary>
	/// Indicates grouped two-string kite.
	/// </summary>
	[TechniqueMetadata(
		4.3, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/2-String_Kite.html"])]
	GroupedTwoStringKite,

	/// <summary>
	/// Indicates grouped turbot fish.
	/// </summary>
	[TechniqueMetadata(
		4.4, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/2-String_Kite.html"])]
	GroupedTurbotFish,
	#endregion

	//
	// Empty Rectangle
	//
	#region Empty Rectangle
	/// <summary>
	/// Indicates empty rectangle.
	/// </summary>
	[Hodoku(120, HodokuDifficultyLevel.Hard, Prefix = "0402")]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Hard, TechniqueGroup.EmptyRectangle, typeof(EmptyRectangleStep),
		Abbreviation = "ER", Links = ["http://sudopedia.enjoysudoku.com/Empty_Rectangle.html"])]
	EmptyRectangle,
	#endregion

	//
	// Alternating Inference Chain
	//
	#region Chaining
	/// <summary>
	/// Indicates X-Chain.
	/// </summary>
	[Hodoku(260, HodokuDifficultyLevel.Unfair, Prefix = "0701")]
	[SudokuExplainer(RatingValueOriginal = [6.6, 6.9])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		ExtraFactors = [Length], Links = ["http://sudopedia.enjoysudoku.com/X-Chain.html"])]
	XChain,

	/// <summary>
	/// Indicates Y-Chain.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(260, HodokuDifficultyLevel.Unfair, Prefix = "0702")]
#endif
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, ExtraFactors = [Length])]
	YChain,

	/// <summary>
	/// Indicates fishy cycle (X-Cycle).
	/// </summary>
	[Hodoku(Prefix = "0704")]
	[SudokuExplainer(RatingValueOriginal = [6.5, 6.6], Aliases = ["Bidirectional X-Cycle"])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Fishy_Cycle.html"])]
	FishyCycle,

	/// <summary>
	/// Indicates XY-Chain.
	/// </summary>
	[Hodoku(260, HodokuDifficultyLevel.Unfair, Prefix = "0702")]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented, ExtraFactors = [Length], Links = ["http://sudopedia.enjoysudoku.com/XY-Chain.html"])]
	XyChain,

	/// <summary>
	/// Indicates XY-Cycle.
	/// </summary>
	[SudokuExplainer(RatingValueOriginal = [6.6, 7.0])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		ExtraFactors = [Length], Features = TechniqueFeature.NotImplemented)]
	XyCycle,

	/// <summary>
	/// Indicates XY-X-Chain.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented, ExtraFactors = [Length])]
	XyXChain,

	/// <summary>
	/// Indicates remote pair.
	/// </summary>
	[Hodoku(110, HodokuDifficultyLevel.Hard, Prefix = "0703")]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented, ExtraFactors = [Length])]
	RemotePair,

	/// <summary>
	/// Indicates purple cow.
	/// </summary>
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented, ExtraFactors = [Length])]
	PurpleCow,

	/// <summary>
	/// Indicates discontinuous nice loop.
	/// </summary>
	[Hodoku(280, HodokuDifficultyLevel.Unfair, Prefix = "0707")]
	[SudokuExplainer(RatingValueOriginal = [7.0, 7.6], Aliases = ["Forcing Chain"])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Abbreviation = "DNL", ExtraFactors = [Length], Features = TechniqueFeature.NotImplemented,
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?t=2859"])]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates continuous nice loop.
	/// </summary>
	[Hodoku(280, HodokuDifficultyLevel.Unfair, Prefix = "0706")]
	[SudokuExplainer(RatingValueOriginal = [7.0, 7.3], Aliases = ["Bidirectional Cycle"])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Abbreviation = "CNL", ExtraFactors = [Length], Links = ["http://sudopedia.enjoysudoku.com/Nice_Loop.html"])]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates alternating inference chain.
	/// </summary>
	[Hodoku(280, HodokuDifficultyLevel.Unfair, Prefix = "0708")]
	[SudokuExplainer(RatingValueOriginal = [7.0, 7.6])]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Abbreviation = "AIC", ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Alternating_Inference_Chain.html", "http://forum.enjoysudoku.com/viewtopic.php?t=3865"])]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates grouped X-Chain.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXChain,

	/// <summary>
	/// Indicates grouped fishy cycle (grouped X-Cycle).
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedFishyCycle,

	/// <summary>
	/// Indicates grouped XY-Chain.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXyChain,

	/// <summary>
	/// Indicates grouped XY-Cycle.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXyCycle,

	/// <summary>
	/// Indicates grouped XY-X-Chain.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXyXChain,

	/// <summary>
	/// Indicates grouped purple cow.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedPurpleCow,

	/// <summary>
	/// Indicates grouped discontinuous nice loop.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0710")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates grouped continuous nice loop.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0709")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates grouped alternating inference chain.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0711")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates special case that a grouped alternating inference chain has a collision between start and end node.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	NodeCollision,
	#endregion

	//
	// Forcing Chains
	//
	#region Forcing Chains
	/// <summary>
	/// Indicates nishio forcing chains.
	/// </summary>
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.NishioForcingChain, RatingValueOriginal = [7.6, 8.1])]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Nightmare, containingGroup: TechniqueGroup.ForcingChains,
		Links = ["http://sudopedia.enjoysudoku.com/Nishio.html"])]
	NishioForcingChains,

	/// <summary>
	/// Indicates region forcing chains (i.e. house forcing chains).
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1301")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.MultipleForcingChain, RatingValueOriginal = [8.2, 8.6])]
	[TechniqueMetadata(
		8.0, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(RegionForcingChainsStep),
		ExtraFactors = [Length])]
	RegionForcingChains,

	/// <summary>
	/// Indicates cell forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1301")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.MultipleForcingChain, RatingValueOriginal = [8.2, 8.6])]
	[TechniqueMetadata(
		8.0, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(CellForcingChainsStep),
		ExtraFactors = [Length])]
	CellForcingChains,

	/// <summary>
	/// Indicates dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1303")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.6, 9.4])]
	[TechniqueMetadata(
		8.5, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(RegionForcingChainsStep),
		ExtraFactors = [Length], Features = TechniqueFeature.HardToBeGenerated)]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates dynamic cell forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1303")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.6, 9.4])]
	[TechniqueMetadata(
		8.5, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(CellForcingChainsStep),
		ExtraFactors = [Length], Features = TechniqueFeature.HardToBeGenerated)]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates dynamic contradiction forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1304")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.8, 9.4])]
	[TechniqueMetadata(
		9.5, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(BinaryForcingChainsStep),
		ExtraFactors = [Length], Features = TechniqueFeature.HardToBeGenerated)]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates dynamic double forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1304")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.8, 9.4])]
	[TechniqueMetadata(
		9.5, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(BinaryForcingChainsStep),
		ExtraFactors = [Length], Features = TechniqueFeature.HardToBeGenerated)]
	DynamicDoubleForcingChains,

	/// <summary>
	/// Indicates dynamic forcing chains.
	/// </summary>
	[TechniqueMetadata(difficultyLevel: DifficultyLevel.Nightmare, containingGroup: TechniqueGroup.ForcingChains)]
	DynamicForcingChains,
	#endregion

	//
	// Blossom Loop
	//
	#region Blossom Loop
	/// <summary>
	/// Indicates blossom loop.
	/// </summary>
	[TechniqueMetadata(
		8.0, DifficultyLevel.Nightmare, TechniqueGroup.BlossomLoop, typeof(BlossomLoopStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Length],
		Links = ["http://forum.enjoysudoku.com/blossom-loop-t42270.html"])]
	BlossomLoop,
	#endregion

	//
	// Aligned Exclusion
	//
	#region Aligned Exclusion
	/// <summary>
	/// Indicates aligned pair exclusion.
	/// </summary>
	[SudokuExplainer(6.2, SudokuExplainerTechnique.AlignedPairExclusion)]
	[TechniqueMetadata(
		6.2, DifficultyLevel.Fiendish, TechniqueGroup.AlignedExclusion, typeof(AlignedExclusionStep),
		Abbreviation = "APE", Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = [
			"http://sudopedia.enjoysudoku.com/Subset_Exclusion.html",
			"http://sudopedia.enjoysudoku.com/Aligned_Pair_Exclusion.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=3882"
		])]
	AlignedPairExclusion,

	/// <summary>
	/// Indicates aligned triple exclusion.
	/// </summary>
	[SudokuExplainer(7.5, SudokuExplainerTechnique.AlignedTripletExclusion)]
	[TechniqueMetadata(
		7.5, DifficultyLevel.Fiendish, TechniqueGroup.AlignedExclusion, typeof(AlignedExclusionStep),
		Abbreviation = "ATE", Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Subset_Exclusion.html", "http://sudopedia.enjoysudoku.com/Aligned_Pair_Exclusion.html",])]
	AlignedTripleExclusion,

	/// <summary>
	/// Indicates aligned quadruple exclusion.
	/// </summary>
	[TechniqueMetadata(
		8.1, DifficultyLevel.Fiendish, TechniqueGroup.AlignedExclusion, typeof(AlignedExclusionStep),
		Abbreviation = "AQE", Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Subset_Exclusion.html"])]
	AlignedQuadrupleExclusion,

	/// <summary>
	/// Indicates aligned quintuple exclusion.
	/// </summary>
	[TechniqueMetadata(
		8.4, DifficultyLevel.Fiendish, TechniqueGroup.AlignedExclusion, typeof(AlignedExclusionStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, Links = ["http://sudopedia.enjoysudoku.com/Subset_Exclusion.html"])]
	AlignedQuintupleExclusion,
	#endregion

	//
	// XYZ-Ring
	//
	#region XYZ-Ring
	/// <summary>
	/// Indicates XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	XyzLoop,

	/// <summary>
	/// Indicates Siamese XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseXyzLoop,

	/// <summary>
	/// Indicates XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	XyzNiceLoop,

	/// <summary>
	/// Indicates Siamese XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseXyzNiceLoop,

	/// <summary>
	/// Indicates grouped XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	GroupedXyzLoop,

	/// <summary>
	/// Indicates Siamese grouped XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseGroupedXyzLoop,

	/// <summary>
	/// Indicates grouped XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	GroupedXyzNiceLoop,

	/// <summary>
	/// Indicates Siamese grouped XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseGroupedXyzNiceLoop,
	#endregion

	//
	// Almost Locked Sets
	//
	#region Almost Locked Sets
	/// <summary>
	/// Indicates singly linked ALS-XZ.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0901")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.AlsXz, RatingValueAdvanced = [7.5])]
	[TechniqueMetadata(
		5.5, DifficultyLevel.Fiendish, TechniqueGroup.AlmostLockedSetsChainingLike, typeof(AlmostLockedSetsXzStep),
		Abbreviation = "ALS-XZ", Links = ["http://sudopedia.enjoysudoku.com/ALS-XZ.html"])]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates doubly linked ALS-XZ.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0901")]
	[SudokuExplainer(RatingValueAdvanced = [7.5])]
	[TechniqueMetadata(
		5.7, DifficultyLevel.Fiendish, TechniqueGroup.AlmostLockedSetsChainingLike, typeof(AlmostLockedSetsXzStep),
		Abbreviation = "ALS-XZ", Links = ["http://sudopedia.enjoysudoku.com/ALS-XZ.html"])]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates ALS-XY-Wing.
	/// </summary>
	[Hodoku(320, HodokuDifficultyLevel.Unfair, Prefix = "0902")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.AlsXyWing, RatingValueAdvanced = [8.0])]
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.AlmostLockedSetsChainingLike, typeof(AlmostLockedSetsXyWingStep),
		Abbreviation = "ALS-XY-Wing", Links = ["http://sudopedia.enjoysudoku.com/ALS-XY-Wing.html"])]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates ALS-W-Wing.
	/// </summary>
	[TechniqueMetadata(
		6.2, DifficultyLevel.Fiendish, TechniqueGroup.AlmostLockedSetsChainingLike, typeof(AlmostLockedSetsWWingStep),
		Abbreviation = "ALS-W-Wing")]
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates ALS chain.
	/// </summary>
	[Hodoku(340, HodokuDifficultyLevel.Unfair, Prefix = "0903")]
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Fiendish, containingGroup: TechniqueGroup.AlmostLockedSetsChainingLike,
		Features = TechniqueFeature.NotImplemented, Links = ["http://sudopedia.enjoysudoku.com/ALS-XY-Chain.html"])]
	AlmostLockedSetsChain,
	#endregion

	//
	// Empty Rectangle Intersection Pair
	//
	#region Empty Rectangle Intersection Pair
	/// <summary>
	/// Indicates empty rectangle intersection pair.
	/// </summary>
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.EmptyRectangleIntersectionPair, typeof(EmptyRectangleIntersectionPairStep),
		Abbreviation = "ERIP", Links = ["http://forum.enjoysudoku.com/post288015.html"])]
	EmptyRectangleIntersectionPair,
	#endregion

	//
	// Death Blossom
	//
	#region Death Blossom
	/// <summary>
	/// Indicates death blossom.
	/// </summary>
	[Hodoku(360, HodokuDifficultyLevel.Unfair, Prefix = "0904")]
	[TechniqueMetadata(
		8.2, DifficultyLevel.Fiendish, TechniqueGroup.DeathBlossom, typeof(DeathBlossomStep),
		Abbreviation = "DB", ExtraFactors = [Petals], Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	DeathBlossom,

	/// <summary>
	/// Indicates death blossom (house blooming).
	/// </summary>
	[TechniqueMetadata(
		8.3, DifficultyLevel.Fiendish, TechniqueGroup.DeathBlossom, typeof(HouseDeathBlossomStep),
		ExtraFactors = [Petals], Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	HouseDeathBlossom,

	/// <summary>
	/// Indicates death blossom (rectangle blooming).
	/// </summary>
	[TechniqueMetadata(
		8.5, DifficultyLevel.Nightmare, TechniqueGroup.DeathBlossom, typeof(RectangleDeathBlossomStep),
		ExtraFactors = [Petals], Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	RectangleDeathBlossom,

	/// <summary>
	/// Indicates death blossom (A^nLS blooming).
	/// </summary>
	[TechniqueMetadata(
		8.7, DifficultyLevel.Nightmare, TechniqueGroup.DeathBlossom, typeof(NTimesAlmostLockedSetDeathBlossomStep),
		ExtraFactors = [Petals], Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	NTimesAlmostLockedSetDeathBlossom,
	#endregion

	//
	// Symmetry
	//
	#region Symmetry
	/// <summary>
	/// Indicates Gurth's symmetrical placement.
	/// </summary>
	[TechniqueMetadata(
		7.0, DifficultyLevel.Fiendish, TechniqueGroup.Symmetry, typeof(GurthSymmetricalPlacementStep),
		Abbreviation = "GSP", Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=32842#p32842"])]
	GurthSymmetricalPlacement,

	/// <summary>
	/// Indicates extended Gurth's symmetrical placement.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Nightmare, containingGroup: TechniqueGroup.Symmetry,
		Features = TechniqueFeature.NotImplemented | TechniqueFeature.HardToBeGenerated)]
	ExtendedGurthSymmetricalPlacement,

	/// <summary>
	/// Indicates Anti-GSP (Anti- Gurth's Symmetrical Placement).
	/// </summary>
	[TechniqueMetadata(
		7.3, DifficultyLevel.Fiendish, TechniqueGroup.Symmetry, typeof(AntiGurthSymmetricalPlacementStep),
		SecondarySupportedType = typeof(GurthSymmetricalPlacementStep), Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/new-type-of-gsp-t40470.html"])]
	AntiGurthSymmetricalPlacement,
	#endregion

	//
	// Exocet
	//
	#region Exocet
	/// <summary>
	/// Indicates junior exocet.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetBaseStep),
		Abbreviation = "JE", Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocet,

	/// <summary>
	/// Indicates junior exocet with target conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetConjugatePair,

	/// <summary>
	/// Indicates junior exocet mirror mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates junior exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetAdjacentTargetStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates junior exocet incompatible pair.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetIncompatiblePairStep),
		ExtraFactors = [IncompatiblePair], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetIncompatiblePair,

	/// <summary>
	/// Indicates junior exocet target pair.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetTargetPairStep),
		ExtraFactors = [TargetPair], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetTargetPair,

	/// <summary>
	/// Indicates junior exocet generalized fish.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetGeneralizedFishStep),
		ExtraFactors = [GeneralizedFish], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetGeneralizedFish,

	/// <summary>
	/// Indicates junior exocet mirror almost hidden set.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetMirrorAlmostHiddenSetStep),
		ExtraFactors = [AlmostHiddenSet], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetMirrorAlmostHiddenSet,

	/// <summary>
	/// Indicates junior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetLockedMemberStep),
		ExtraFactors = [LockedMember], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetLockedMember,

	/// <summary>
	/// Indicates junior exocet mirror sync.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetMirrorSyncStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetMirrorSync,

	/// <summary>
	/// Indicates senior exocet.
	/// </summary>
	[TechniqueMetadata(
		9.6, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetBaseStep),
		Abbreviation = "SE", Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocet,

	/// <summary>
	/// Indicates senior exocet mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates senior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetLockedMemberStep),
		ExtraFactors = [LockedMember], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocetLockedMember,

	/// <summary>
	/// Indicates senior exocet true base.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(SeniorExocetTrueBaseStep),
		ExtraFactors = [TrueBase], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocetTrueBase,

	/// <summary>
	/// Indicates weak exocet.
	/// </summary>
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetStep),
		Abbreviation = "WE", ExtraFactors = [MissingStabilityBalancer],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocet,

	/// <summary>
	/// Indicates weak exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetAdjacentTargetStep),
		ExtraFactors = [Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocetAdjacentTarget,

	/// <summary>
	/// Indicates weak exocet slash.
	/// </summary>
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetSlashStep),
		ExtraFactors = [SlashElimination],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocetSlash,

	/// <summary>
	/// Indicates weak exocet BZ rectangle.
	/// </summary>
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetBzRectangleStep),
		ExtraFactors = [BzRectangle],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocetBzRectangle,

	/// <summary>
	/// Indicates lame weak exocet.
	/// </summary>
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetStep),
		ExtraFactors = [MissingStabilityBalancer],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	LameWeakExocet,

	/// <summary>
	/// Indicates franken junior exocet.
	/// </summary>
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocet,

	/// <summary>
	/// Indicates franken junior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocetLockedMember,

	/// <summary>
	/// Indicates franken junior exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetAdjacentTargetStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates franken junior exocet mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates mutant junior exocet.
	/// </summary>
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocet,

	/// <summary>
	/// Indicates mutant junior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocetLockedMember,

	/// <summary>
	/// Indicates mutant junior exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetAdjacentTargetStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates mutant junior exocet mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates franken senior exocet.
	/// </summary>
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenSeniorExocet,

	/// <summary>
	/// Indicates franken senior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenSeniorExocetLockedMember,

	/// <summary>
	/// Indicates advanced franken senior exocet.
	/// </summary>
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(AdvancedComplexSeniorExocetStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	AdvancedFrankenSeniorExocet,

	/// <summary>
	/// Indicates mutant senior exocet.
	/// </summary>
	[TechniqueMetadata(
		10.2, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantSeniorExocet,

	/// <summary>
	/// Indicates mutant senior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		10.2, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantSeniorExocetLockedMember,

	/// <summary>
	/// Indicates advanced mutant senior exocet.
	/// </summary>
	[TechniqueMetadata(
		10.1, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(AdvancedComplexSeniorExocetStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	AdvancedMutantSeniorExocet,

	/// <summary>
	/// Indicates double exocet.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(DoubleExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	DoubleExocet,

	/// <summary>
	/// Indicates double exocet uni-fish pattern.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(DoubleExocetGeneralizedFishStep),
		ExtraFactors = [GeneralizedFish], Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	DoubleExocetGeneralizedFish,

	/// <summary>
	/// Indicates pattern-locked quadruple. This quadruple is a special quadruple: it can only be concluded after both JE and SK-Loop are formed.
	/// </summary>
	[TechniqueMetadata(
		difficultyLevel: DifficultyLevel.Nightmare, containingGroup: TechniqueGroup.Exocet,
		Abbreviation = "PLQ", Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	PatternLockedQuadruple,
	#endregion

	//
	// Domino Loop
	//
	#region Domino Loop
	/// <summary>
	/// Indicates domino loop.
	/// </summary>
	[TechniqueMetadata(
		9.6, DifficultyLevel.Nightmare, TechniqueGroup.DominoLoop, typeof(DominoLoopStep),
		Features = TechniqueFeature.HardToBeGenerated, Links = ["http://forum.enjoysudoku.com/domino-loops-sk-loops-beyond-t32789.html"])]
	DominoLoop,
	#endregion

	//
	// Multi-sector Locked Sets
	//
	#region Multi-sector Locked Sets
	/// <summary>
	/// Indicates multi-sector locked sets.
	/// </summary>
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.MultisectorLockedSets, typeof(MultisectorLockedSetsStep),
		Abbreviation = "MSLS", Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/exotic-patterns-a-resume-t30508-270.html"])]
	MultisectorLockedSets,
	#endregion

	//
	// Pattern Overlay
	//
	#region Pattern Overlay
	/// <summary>
	/// Indicates pattern overlay method.
	/// </summary>
	[TechniqueMetadata(
		8.5, DifficultyLevel.LastResort, TechniqueGroup.PatternOverlay, typeof(PatternOverlayStep),
		Abbreviation = "POM", Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Pattern_Overlay_Method.html"])]
	PatternOverlay,
	#endregion

	//
	// Templating
	//
	#region Templating
	/// <summary>
	/// Indicates template set.
	/// </summary>
	[Hodoku(10000, HodokuDifficultyLevel.Extreme, Prefix = "1201")]
	[TechniqueMetadata(
		9.0, DifficultyLevel.LastResort, TechniqueGroup.Templating, typeof(TemplateStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, Links = ["http://sudopedia.enjoysudoku.com/Templating.html"])]
	TemplateSet,

	/// <summary>
	/// Indicates template delete.
	/// </summary>
	[Hodoku(10000, HodokuDifficultyLevel.Extreme, Prefix = "1202")]
	[TechniqueMetadata(
		9.0, DifficultyLevel.LastResort, TechniqueGroup.Templating, typeof(TemplateStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, Links = ["http://sudopedia.enjoysudoku.com/Templating.html"])]
	TemplateDelete,
	#endregion

	//
	// Bowman's Bingo
	//
	#region Bowman's Bingo
	/// <summary>
	/// Indicates bowman's bingo.
	/// </summary>
	[TechniqueMetadata(
		8.0, DifficultyLevel.LastResort, TechniqueGroup.BowmanBingo, typeof(BowmanBingoStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, ExtraFactors = [Length],
		Links = ["http://sudopedia.enjoysudoku.com/Bowman_Bingo.html"])]
	BowmanBingo,
	#endregion

	//
	// Brute Force
	//
	#region Brute Force
	/// <summary>
	/// Indicates brute force.
	/// </summary>
	[Hodoku(10000, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainer((double)AnalyzerResult.MaximumRatingValueTheory, SudokuExplainerTechnique.BruteForce, Aliases = ["Try & Error"])]
	[TechniqueMetadata(
		(double)AnalyzerResult.MaximumRatingValueTheory, DifficultyLevel.LastResort, TechniqueGroup.BruteForce, typeof(BruteForceStep),
		Abbreviation = "BF", Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://sudopedia.enjoysudoku.com/Trial_%26_Error.html"])]
	BruteForce,
	#endregion
}
