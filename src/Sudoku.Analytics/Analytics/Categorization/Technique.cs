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
#if UNIQUE_RECTANGLE_W_WING
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.UniqueRectangle, typeof(UniqueRectangleWWingStep),
		ExtraFactors = [Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
#endif
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
#if UNIQUE_RECTANGLE_W_WING
	[TechniqueMetadata(
		4.5, DifficultyLevel.Hard, TechniqueGroup.AvoidableRectangle, typeof(UniqueRectangleWWingStep),
		ExtraFactors = Avoidable, Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
#endif
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
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[Hodoku(100, HodokuDifficultyLevel.Hard, Prefix = "0610")]
	[HodokuAliasedNames("Bivalue Universal Grave + 1")]
	[SudokuExplainer(5.6, SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType1Step))]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates bi-value universal grave type 2.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[SudokuExplainer(5.7, SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType2Step),
		ExtraFactors = [ExtraDigit])]
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates bi-value universal grave type 3.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.BivalueUniversalGrave)]
	[SudokuExplainerDifficultyRating(5.8, 6.1)]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType3Step),
		ExtraFactors = [Size, Hidden])]
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates bi-value universal grave type 4.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[SudokuExplainer(5.7, SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		5.6, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveType4Step),
		ExtraFactors = [ConjugatePair])]
	BivalueUniversalGraveType4,

	/// <summary>
	/// Indicates bi-value universal grave + n.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[SudokuExplainer(techniqueDefined: SudokuExplainerTechnique.BivalueUniversalGravePlusN, RatingValueAdvanced = [5.7])]
	[TechniqueGroup(TechniqueGroup.BivalueUniversalGrave)]
	[Abbreviation("BUG + n")]
	[StaticDifficultyLevel(DifficultyLevel.Hard)]
	[BoundStepTypes(typeof(BivalueUniversalGraveMultipleStep))]
	[StaticDifficulty(5.7)]
	[ExtraDifficultyFactors(Size)]
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Indicates bi-value universal grave false candidate type.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[TechniqueMetadata(5.7, DifficultyLevel.Hard, TechniqueGroup.BivalueUniversalGrave, typeof(BivalueUniversalGraveFalseCandidateTypeStep))]
	BivalueUniversalGraveFalseCandidateType,

	/// <summary>
	/// Indicates bi-value universal grave + n with forcing chains.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[TechniqueGroup(TechniqueGroup.BivalueUniversalGrave)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	BivalueUniversalGravePlusNForcingChains,

	/// <summary>
	/// Indicates bi-value universal grave XZ rule.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[TechniqueGroup(TechniqueGroup.BivalueUniversalGrave)]
	[Abbreviation("BUG-XZ")]
	[StaticDifficultyLevel(DifficultyLevel.Hard)]
	[BoundStepTypes(typeof(BivalueUniversalGraveXzStep))]
	[StaticDifficulty(5.8)]
	BivalueUniversalGraveXzRule,

	/// <summary>
	/// Indicates bi-value universal grave XY-Wing.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2352")]
	[TechniqueGroup(TechniqueGroup.BivalueUniversalGrave)]
	[Abbreviation("BUG-XY-Wing")]
	[StaticDifficultyLevel(DifficultyLevel.Hard)]
	BivalueUniversalGraveXyWing,
	#endregion

	//
	// Reverse Bivalue Universal Grave
	//
	#region Reverse Bivalue Universal Grave
	/// <summary>
	/// Indicates reverse bi-value universal grave type 1.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Reverse_BUG.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=4431")]
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType1Step),
		ExtraFactors = [Length])]
	ReverseBivalueUniversalGraveType1,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 2.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Reverse_BUG.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=4431")]
	[TechniqueMetadata(
		6.1, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType2Step),
		ExtraFactors = [Length])]
	ReverseBivalueUniversalGraveType2,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 3.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Reverse_BUG.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=4431")]
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType3Step),
		ExtraFactors = [Length])]
	ReverseBivalueUniversalGraveType3,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 4.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Reverse_BUG.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=4431")]
	[TechniqueMetadata(
		6.3, DifficultyLevel.Fiendish, TechniqueGroup.ReverseBivalueUniversalGrave, typeof(ReverseBivalueUniversalGraveType4Step),
		ExtraFactors = [Length])]
	ReverseBivalueUniversalGraveType4,
	#endregion

	//
	// Uniqueness Clue Cover
	//
	#region Uniqueness Clue Cover
	/// <summary>
	/// Indicates uniqueness clue cover.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Uniqueness_Clue_Cover.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/uniqueness-clue-cover-t40814.html")]
	[TechniqueMetadata(
		6.5, DifficultyLevel.Fiendish, TechniqueGroup.UniquenessClueCover, typeof(UniquenessClueCoverStep),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size])]
	UniquenessClueCover,
	#endregion

	//
	// RW's Theory
	//
	#region RW's Theory
	/// <summary>
	/// Indicates RW's deadly pattern.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/yet-another-crazy-uniqueness-technique-t5589.html")]
	[TechniqueGroup(TechniqueGroup.RwDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
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
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueGroup(TechniqueGroup.QiuDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(QiuDeadlyPatternType1Step))]
	[StaticDifficulty(5.8)]
	QiuDeadlyPatternType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 2.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueGroup(TechniqueGroup.QiuDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(QiuDeadlyPatternType2Step))]
	[StaticDifficulty(5.9)]
	QiuDeadlyPatternType2,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 3.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueMetadata(
		5.8, DifficultyLevel.Fiendish, TechniqueGroup.QiuDeadlyPattern, typeof(QiuDeadlyPatternType3Step),
		Features = TechniqueFeature.HardToBeGenerated, ExtraFactors = [Size])]
	QiuDeadlyPatternType3,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 4.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueGroup(TechniqueGroup.QiuDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(QiuDeadlyPatternType4Step))]
	[StaticDifficulty(6.0)]
	QiuDeadlyPatternType4,

	/// <summary>
	/// Indicates locked Qiu's deadly pattern.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueGroup(TechniqueGroup.QiuDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(QiuDeadlyPatternLockedTypeStep))]
	[StaticDifficulty(6.0)]
	LockedQiuDeadlyPattern,

	/// <summary>
	/// Indicates Qiu's deadly pattern external type 1.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueGroup(TechniqueGroup.QiuDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(QiuDeadlyPatternExternalType1Step))]
	[StaticDifficulty(6.0)]
	QiuDeadlyPatternExternalType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern external type 2.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/distinction-theory-t35042.html")]
	[TechniqueGroup(TechniqueGroup.QiuDeadlyPattern)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(QiuDeadlyPatternExternalType2Step))]
	[StaticDifficulty(6.1)]
	QiuDeadlyPatternExternalType2,
	#endregion

	//
	// Unique Matrix
	//
	#region Unique Matrix
	/// <summary>
	/// Indicates unique matrix type 1.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.UniqueMatrix)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(UniqueMatrixType1Step))]
	[StaticDifficulty(5.3)]
	UniqueMatrixType1,

	/// <summary>
	/// Indicates unique matrix type 2.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.UniqueMatrix)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(UniqueMatrixType2Step))]
	[StaticDifficulty(5.4)]
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
	[TechniqueGroup(TechniqueGroup.UniqueMatrix)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(UniqueMatrixType4Step))]
	[StaticDifficulty(5.5)]
	UniqueMatrixType4,
	#endregion

	//
	// Sue de Coq
	//
	#region Sue de Coq
	/// <summary>
	/// Indicates sue de coq.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Sue_de_Coq.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/two-sector-disjoint-subsets-t2033.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/benchmark-sudoku-list-t3834-15.html#p43170")]
	[Hodoku(250, HodokuDifficultyLevel.Unfair, Prefix = "1101")]
	[SudokuExplainerDifficultyRating(5.0, IsAdvancedDefined = true)]
	[TechniqueGroup(TechniqueGroup.SueDeCoq)]
	[Abbreviation("SdC")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(SueDeCoqStep))]
	[StaticDifficulty(5.0)]
	[ExtraDifficultyFactors(Isolated, Cannibalism)]
	SueDeCoq,

	/// <summary>
	/// Indicates sue de coq with isolated digit.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Sue_de_Coq.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/two-sector-disjoint-subsets-t2033.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/benchmark-sudoku-list-t3834-15.html#p43170")]
	[Hodoku(250, HodokuDifficultyLevel.Unfair, Prefix = "1101")]
	[SudokuExplainerDifficultyRating(5.0, IsAdvancedDefined = true)]
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoqStep),
		ExtraFactors = [Isolated, Cannibalism])]
	SueDeCoqIsolated,

	/// <summary>
	/// Indicates 3-dimensional sue de coq.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Sue_de_Coq.html")]
	[TechniqueMetadata(5.5, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoq3DimensionStep))]
	SueDeCoq3Dimension,

	/// <summary>
	/// Indicates sue de coq cannibalism.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Sue_de_Coq.html")]
	[TechniqueMetadata(
		5.0, DifficultyLevel.Fiendish, TechniqueGroup.SueDeCoq, typeof(SueDeCoqStep),
		ExtraFactors = [Isolated, Cannibalism])]
	SueDeCoqCannibalism,
	#endregion

	//
	// Fireworks
	//
	#region Fireworks
	/// <summary>
	/// Indicates firework triple.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/fireworks-t39513.html")]
	[TechniqueMetadata(6.0, DifficultyLevel.Fiendish, TechniqueGroup.Firework, typeof(FireworkTripleStep))]
	FireworkTriple,

	/// <summary>
	/// Indicates firework quadruple.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/fireworks-t39513.html")]
	[TechniqueMetadata(6.3, DifficultyLevel.Fiendish, TechniqueGroup.Firework, typeof(FireworkQuadrupleStep))]
	FireworkQuadruple,
	#endregion

	//
	// Broken Wing
	//
	#region Broken Wing
	/// <summary>
	/// Indicates broken wing.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Broken_Wing.html")]
	[HodokuTechniquePrefix("0705")]
	[TechniqueMetadata(5.5, DifficultyLevel.Fiendish, TechniqueGroup.BrokenWing, typeof(GuardianStep))]
	BrokenWing,
	#endregion

	//
	// Bi-value Oddagon
	//
	#region Bivalue Oddagon
	/// <summary>
	/// Indicates bi-value oddagon type 2.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/technique-share-odd-bivalue-loop-bivalue-oddagon-t33153.html")]
	[TechniqueMetadata(6.1, DifficultyLevel.Fiendish, TechniqueGroup.BivalueOddagon, typeof(BivalueOddagonType2Step))]
	BivalueOddagonType2,

	/// <summary>
	/// Indicates bi-value oddagon type 3.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/technique-share-odd-bivalue-loop-bivalue-oddagon-t33153.html")]
	[TechniqueMetadata(
		6.0, DifficultyLevel.Fiendish, TechniqueGroup.BivalueOddagon, typeof(BivalueOddagonType3Step),
		ExtraFactors = [Size])]
	BivalueOddagonType3,
	#endregion

	//
	// Chromatic Pattern
	//
	#region Chromatic Pattern
	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 1.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/chromatic-patterns-t39885.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html")]
	[TechniqueGroup(TechniqueGroup.RankTheory)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(ChromaticPatternType1Step))]
	[StaticDifficulty(6.5)]
	ChromaticPatternType1,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 2.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/chromatic-patterns-t39885.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html")]
	[TechniqueGroup(TechniqueGroup.RankTheory)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	ChromaticPatternType2,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 3.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/chromatic-patterns-t39885.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html")]
	[TechniqueGroup(TechniqueGroup.RankTheory)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	ChromaticPatternType3,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 4.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/chromatic-patterns-t39885.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html")]
	[TechniqueGroup(TechniqueGroup.RankTheory)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	ChromaticPatternType4,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) XZ rule.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.RankTheory)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(ChromaticPatternXzStep))]
	[StaticDifficulty(6.7)]
	ChromaticPatternXzRule,
	#endregion

	//
	// Single Digit Pattern
	//
	#region Single Digit Pattern
	/// <summary>
	/// Indicates skyscraper.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Skyscraper.html")]
	[Hodoku(130, HodokuDifficultyLevel.Hard, Prefix = "0400")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainerTechnique(SudokuExplainerTechnique.TurbotFish)]
	[SudokuExplainerDifficultyRating(6.6)]
	[SudokuExplainerAliasNames("Turbot Fish")]
#endif
	[TechniqueMetadata(4.0, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep))]
	Skyscraper,

	/// <summary>
	/// Indicates two-string kite.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/2-String_Kite.html")]
	[Hodoku(150, HodokuDifficultyLevel.Hard, Prefix = "0401")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainerTechnique(SudokuExplainerTechnique.TurbotFish)]
	[SudokuExplainerDifficultyRating(6.6)]
	[SudokuExplainerAliasNames("Turbot Fish")]
#endif
	[TechniqueMetadata(4.1, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep))]
	TwoStringKite,

	/// <summary>
	/// Indicates turbot fish.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=833")]
	[Hodoku(120, HodokuDifficultyLevel.Hard, Prefix = "0403")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.TurbotFish)]
	[SudokuExplainerDifficultyRating(6.6)]
	[TechniqueMetadata(4.2, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep))]
	TurbotFish,

	/// <summary>
	/// Indicates grouped skyscraper.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Skyscraper.html")]
	[TechniqueMetadata(4.2, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep))]
	GroupedSkyscraper,

	/// <summary>
	/// Indicates grouped two-string kite.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/2-String_Kite.html")]
	[TechniqueMetadata(4.3, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep))]
	GroupedTwoStringKite,

	/// <summary>
	/// Indicates grouped turbot fish.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=833")]
	[TechniqueMetadata(4.4, DifficultyLevel.Hard, TechniqueGroup.SingleDigitPattern, typeof(TwoStrongLinksStep))]
	GroupedTurbotFish,
	#endregion

	//
	// Empty Rectangle
	//
	#region Empty Rectangle
	/// <summary>
	/// Indicates empty rectangle.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Empty_Rectangle.html")]
	[Hodoku(120, HodokuDifficultyLevel.Hard, Prefix = "0402")]
	[TechniqueGroup(TechniqueGroup.EmptyRectangle)]
	[Abbreviation("ER")]
	[StaticDifficultyLevel(DifficultyLevel.Hard)]
	[BoundStepTypes(typeof(EmptyRectangleStep))]
	[StaticDifficulty(4.6)]
	EmptyRectangle,
	#endregion

	//
	// Alternating Inference Chain
	//
	#region Chaining
	/// <summary>
	/// Indicates X-Chain.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/X-Chain.html")]
	[Hodoku(260, HodokuDifficultyLevel.Unfair, Prefix = "0701")]
	[SudokuExplainerDifficultyRating(6.6, 6.9)]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		ExtraFactors = [Length])]
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
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Fishy_Cycle.html")]
	[HodokuTechniquePrefix("0704")]
	[SudokuExplainerDifficultyRating(6.5, 6.6)]
	[SudokuExplainerNames("Bidirectional X-Cycle")]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, ExtraFactors = [Length])]
	FishyCycle,

	/// <summary>
	/// Indicates XY-Chain.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/XY-Chain.html")]
	[Hodoku(260, HodokuDifficultyLevel.Unfair, Prefix = "0702")]
	[TechniqueMetadata(
		4.6, DifficultyLevel.Fiendish, TechniqueGroup.AlternatingInferenceChain, typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented, ExtraFactors = [Length])]
	XyChain,

	/// <summary>
	/// Indicates XY-Cycle.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[SudokuExplainerDifficultyRating(6.6, 7.0)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(ForcingChainStep))]
	[StaticDifficulty(4.6)]
	[ExtraDifficultyFactors(Length)]
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
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=2859")]
	[Hodoku(280, HodokuDifficultyLevel.Unfair, Prefix = "0707")]
	[SudokuExplainerDifficultyRating(7.0, 7.6)]
	[SudokuExplainerNames("Forcing Chain")]
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[Abbreviation("DNL")]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(ForcingChainStep))]
	[StaticDifficulty(4.6)]
	[ExtraDifficultyFactors(Length)]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates continuous nice loop.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Nice_Loop.html")]
	[Hodoku(280, HodokuDifficultyLevel.Unfair, Prefix = "0706")]
	[SudokuExplainerDifficultyRating(7.0, 7.3)]
	[SudokuExplainerNames("Bidirectional Cycle")]
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[Abbreviation("CNL")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(ForcingChainStep))]
	[StaticDifficulty(4.6)]
	[ExtraDifficultyFactors(Length)]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates alternating inference chain.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Alternating_Inference_Chain.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=3865")]
	[Hodoku(280, HodokuDifficultyLevel.Unfair, Prefix = "0708")]
	[SudokuExplainerDifficultyRating(7.0, 7.6)]
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[Abbreviation("AIC")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(ForcingChainStep))]
	[StaticDifficulty(4.6)]
	[ExtraDifficultyFactors(Length)]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates grouped X-Chain.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedXChain,

	/// <summary>
	/// Indicates grouped fishy cycle (grouped X-Cycle).
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedFishyCycle,

	/// <summary>
	/// Indicates grouped XY-Chain.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedXyChain,

	/// <summary>
	/// Indicates grouped XY-Cycle.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedXyCycle,

	/// <summary>
	/// Indicates grouped XY-X-Chain.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedXyXChain,

	/// <summary>
	/// Indicates grouped purple cow.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedPurpleCow,

	/// <summary>
	/// Indicates grouped discontinuous nice loop.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0710")]
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates grouped continuous nice loop.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0709")]
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates grouped alternating inference chain.
	/// </summary>
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0711")]
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates special case that a grouped alternating inference chain has a collision between start and end node.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlternatingInferenceChain)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	NodeCollision,
	#endregion

	//
	// Forcing Chains
	//
	#region Forcing Chains
	/// <summary>
	/// Indicates nishio forcing chains.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Nishio.html")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.NishioForcingChain)]
	[SudokuExplainerDifficultyRating(7.6, 8.1)]
	[TechniqueGroup(TechniqueGroup.ForcingChains)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	NishioForcingChains,

	/// <summary>
	/// Indicates region forcing chains (i.e. house forcing chains).
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1301")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.MultipleForcingChain)]
	[SudokuExplainerDifficultyRating(8.2, 8.6)]
	[TechniqueMetadata(
		8.0, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(RegionForcingChainsStep),
		ExtraFactors = [Length])]
	RegionForcingChains,

	/// <summary>
	/// Indicates cell forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1301")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.MultipleForcingChain)]
	[SudokuExplainerDifficultyRating(8.2, 8.6)]
	[TechniqueMetadata(
		8.0, DifficultyLevel.Nightmare, TechniqueGroup.ForcingChains, typeof(CellForcingChainsStep),
		ExtraFactors = [Length])]
	CellForcingChains,

	/// <summary>
	/// Indicates dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1303")]
	[SudokuExplainerDifficultyRating(8.6, 9.4)]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.DynamicForcingChain)]
	[TechniqueGroup(TechniqueGroup.ForcingChains)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[BoundStepTypes(typeof(RegionForcingChainsStep))]
	[StaticDifficulty(8.5)]
	[ExtraDifficultyFactors(Length)]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates dynamic cell forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1303")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.DynamicForcingChain)]
	[SudokuExplainerDifficultyRating(8.6, 9.4)]
	[TechniqueGroup(TechniqueGroup.ForcingChains)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[BoundStepTypes(typeof(CellForcingChainsStep))]
	[StaticDifficulty(8.5)]
	[ExtraDifficultyFactors(Length)]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates dynamic contradiction forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1304")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.DynamicForcingChain)]
	[SudokuExplainerDifficultyRating(8.8, 9.4)]
	[TechniqueGroup(TechniqueGroup.ForcingChains)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[BoundStepTypes(typeof(BinaryForcingChainsStep))]
	[StaticDifficulty(9.5)]
	[ExtraDifficultyFactors(Length)]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates dynamic double forcing chains.
	/// </summary>
	[Hodoku(500, HodokuDifficultyLevel.Extreme, Prefix = "1304")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.DynamicForcingChain)]
	[SudokuExplainerDifficultyRating(8.8, 9.4)]
	[TechniqueGroup(TechniqueGroup.ForcingChains)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[BoundStepTypes(typeof(BinaryForcingChainsStep))]
	[StaticDifficulty(9.5)]
	[ExtraDifficultyFactors(Length)]
	DynamicDoubleForcingChains,

	/// <summary>
	/// Indicates dynamic forcing chains.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.ForcingChains)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	DynamicForcingChains,
	#endregion

	//
	// Blossom Loop
	//
	#region Blossom Loop
	/// <summary>
	/// Indicates blossom loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/blossom-loop-t42270.html")]
	[TechniqueGroup(TechniqueGroup.BlossomLoop)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[BoundStepTypes(typeof(BlossomLoopStep))]
	[StaticDifficulty(8.0)]
	[ExtraDifficultyFactors(Length)]
	BlossomLoop,
	#endregion

	//
	// Aligned Exclusion
	//
	#region Aligned Exclusion
	/// <summary>
	/// Indicates aligned pair exclusion.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Subset_Exclusion.html")]
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Aligned_Pair_Exclusion.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?t=3882")]
	[TechniqueGroup(TechniqueGroup.AlignedExclusion)]
	[Abbreviation("APE")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.AlignedPairExclusion)]
	[SudokuExplainerDifficultyRating(6.2)]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlignedExclusionStep))]
	[StaticDifficulty(6.2)]
	AlignedPairExclusion,

	/// <summary>
	/// Indicates aligned triple exclusion.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Subset_Exclusion.html")]
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Aligned_Triple_Exclusion.html")]
	[TechniqueGroup(TechniqueGroup.AlignedExclusion)]
	[Abbreviation("ATE")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.AlignedTripletExclusion)]
	[SudokuExplainerDifficultyRating(7.5)]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlignedExclusionStep))]
	[StaticDifficulty(7.5)]
	AlignedTripleExclusion,

	/// <summary>
	/// Indicates aligned quadruple exclusion.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Subset_Exclusion.html")]
	[TechniqueGroup(TechniqueGroup.AlignedExclusion)]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlignedExclusionStep))]
	[StaticDifficulty(8.1)]
	AlignedQuadrupleExclusion,

	/// <summary>
	/// Indicates aligned quintuple exclusion.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Subset_Exclusion.html")]
	[TechniqueGroup(TechniqueGroup.AlignedExclusion)]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlignedExclusionStep))]
	[StaticDifficulty(8.4)]
	AlignedQuintupleExclusion,
	#endregion

	//
	// XYZ-Ring
	//
	#region XYZ-Ring
	/// <summary>
	/// Indicates XYZ loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	XyzLoop,

	/// <summary>
	/// Indicates Siamese XYZ loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	SiameseXyzLoop,

	/// <summary>
	/// Indicates XYZ nice loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	XyzNiceLoop,

	/// <summary>
	/// Indicates Siamese XYZ nice loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	SiameseXyzNiceLoop,

	/// <summary>
	/// Indicates grouped XYZ loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	GroupedXyzLoop,

	/// <summary>
	/// Indicates Siamese grouped XYZ loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.0, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	SiameseGroupedXyzLoop,

	/// <summary>
	/// Indicates grouped XYZ nice loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	GroupedXyzNiceLoop,

	/// <summary>
	/// Indicates Siamese grouped XYZ nice loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/xyz-ring-t42209.html")]
	[TechniqueMetadata(5.2, DifficultyLevel.Fiendish, TechniqueGroup.XyzRing, typeof(XyzRingStep))]
	SiameseGroupedXyzNiceLoop,
	#endregion

	//
	// Almost Locked Sets
	//
	#region Almost Locked Sets
	/// <summary>
	/// Indicates singly linked ALS-XZ.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/ALS-XZ.html")]
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0901")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.AlsXz)]
	[SudokuExplainerDifficultyRating(7.5, IsAdvancedDefined = true)]
	[TechniqueGroup(TechniqueGroup.AlmostLockedSetsChainingLike)]
	[Abbreviation("ALS-XZ")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlmostLockedSetsXzStep))]
	[StaticDifficulty(5.5)]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates doubly linked ALS-XZ.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/ALS-XZ.html")]
	[Hodoku(300, HodokuDifficultyLevel.Unfair, Prefix = "0901")]
	[SudokuExplainerDifficultyRating(7.5, IsAdvancedDefined = true)]
	[TechniqueGroup(TechniqueGroup.AlmostLockedSetsChainingLike)]
	[Abbreviation("ALS-XZ")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlmostLockedSetsXzStep))]
	[StaticDifficulty(5.7)]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates ALS-XY-Wing.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/ALS-XY-Wing.html")]
	[Hodoku(320, HodokuDifficultyLevel.Unfair, Prefix = "0902")]
	[SudokuExplainerTechnique(SudokuExplainerTechnique.AlsXyWing)]
	[SudokuExplainerDifficultyRating(8.0, IsAdvancedDefined = true)]
	[TechniqueGroup(TechniqueGroup.AlmostLockedSetsChainingLike)]
	[Abbreviation("ALS-XY-Wing")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlmostLockedSetsXyWingStep))]
	[StaticDifficulty(6.0)]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates ALS-W-Wing.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.AlmostLockedSetsChainingLike)]
	[Abbreviation("ALS-W-Wing")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AlmostLockedSetsWWingStep))]
	[StaticDifficulty(6.2)]
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates ALS chain.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/ALS-XY-Chain.html")]
	[Hodoku(340, HodokuDifficultyLevel.Unfair, Prefix = "0903")]
	[TechniqueGroup(TechniqueGroup.AlmostLockedSetsChainingLike)]
	[TechniqueFeature(TechniqueFeature.NotImplemented)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	AlmostLockedSetsChain,
	#endregion

	//
	// Empty Rectangle Intersection Pair
	//
	#region Empty Rectangle Intersection Pair
	/// <summary>
	/// Indicates empty rectangle intersection pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/post288015.html")]
	[TechniqueGroup(TechniqueGroup.EmptyRectangleIntersectionPair)]
	[Abbreviation("ERIP")]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(EmptyRectangleIntersectionPairStep))]
	[StaticDifficulty(6.0)]
	EmptyRectangleIntersectionPair,
	#endregion

	//
	// Death Blossom
	//
	#region Death Blossom
	/// <summary>
	/// Indicates death blossom.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Death_Blossom.html")]
	[Hodoku(360, HodokuDifficultyLevel.Unfair, Prefix = "0904")]
	[TechniqueGroup(TechniqueGroup.DeathBlossom)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[Abbreviation("DB")]
	[BoundStepTypes(typeof(DeathBlossomStep))]
	[StaticDifficulty(8.2)]
	[ExtraDifficultyFactors(Petals)]
	DeathBlossom,

	/// <summary>
	/// Indicates death blossom (house blooming).
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Death_Blossom.html")]
	[TechniqueMetadata(
		8.3, DifficultyLevel.Fiendish, TechniqueGroup.DeathBlossom, typeof(HouseDeathBlossomStep),
		ExtraFactors = [Petals])]
	HouseDeathBlossom,

	/// <summary>
	/// Indicates death blossom (rectangle blooming).
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Death_Blossom.html")]
	[TechniqueMetadata(
		8.5, DifficultyLevel.Nightmare, TechniqueGroup.DeathBlossom, typeof(RectangleDeathBlossomStep),
		ExtraFactors = [Petals])]
	RectangleDeathBlossom,

	/// <summary>
	/// Indicates death blossom (A^nLS blooming).
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Death_Blossom.html")]
	[TechniqueMetadata(
		8.7, DifficultyLevel.Nightmare, TechniqueGroup.DeathBlossom, typeof(NTimesAlmostLockedSetDeathBlossomStep),
		ExtraFactors = [Petals])]
	NTimesAlmostLockedSetDeathBlossom,
	#endregion

	//
	// Symmetry
	//
	#region Symmetry
	/// <summary>
	/// Indicates Gurth's symmetrical placement.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/viewtopic.php?p=32842#p32842")]
	[TechniqueGroup(TechniqueGroup.Symmetry)]
	[Abbreviation("GSP")]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(GurthSymmetricalPlacementStep))]
	[StaticDifficulty(7.0)]
	GurthSymmetricalPlacement,

	/// <summary>
	/// Indicates extended Gurth's symmetrical placement.
	/// </summary>
	[TechniqueGroup(TechniqueGroup.Symmetry)]
	[TechniqueFeature(TechniqueFeature.NotImplemented | TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	ExtendedGurthSymmetricalPlacement,

	/// <summary>
	/// Indicates Anti-GSP (Anti- Gurth's Symmetrical Placement).
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/new-type-of-gsp-t40470.html")]
	[TechniqueGroup(TechniqueGroup.Symmetry)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Fiendish)]
	[BoundStepTypes(typeof(AntiGurthSymmetricalPlacementStep), SecondaryTypes = [typeof(GurthSymmetricalPlacementStep)])]
	[StaticDifficulty(7.3)]
	AntiGurthSymmetricalPlacement,
	#endregion

	//
	// Exocet
	//
	#region Exocet
	/// <summary>
	/// Indicates junior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueGroup(TechniqueGroup.Exocet)]
	[Abbreviation("JE")]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[BoundStepTypes(typeof(ExocetBaseStep))]
	[StaticDifficulty(9.4)]
	JuniorExocet,

	/// <summary>
	/// Indicates junior exocet with target conjugate pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetBaseStep))]
	JuniorExocetConjugatePair,

	/// <summary>
	/// Indicates junior exocet mirror mirror conjugate pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror])]
	JuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates junior exocet adjacent target.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetAdjacentTargetStep),
		ExtraFactors = [Mirror])]
	JuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates junior exocet incompatible pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetIncompatiblePairStep),
		ExtraFactors = [IncompatiblePair])]
	JuniorExocetIncompatiblePair,

	/// <summary>
	/// Indicates junior exocet target pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetTargetPairStep),
		ExtraFactors = [TargetPair])]
	JuniorExocetTargetPair,

	/// <summary>
	/// Indicates junior exocet generalized fish.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetGeneralizedFishStep),
		ExtraFactors = [GeneralizedFish])]
	JuniorExocetGeneralizedFish,

	/// <summary>
	/// Indicates junior exocet mirror almost hidden set.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetMirrorAlmostHiddenSetStep),
		ExtraFactors = [AlmostHiddenSet])]
	JuniorExocetMirrorAlmostHiddenSet,

	/// <summary>
	/// Indicates junior exocet locked member.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetLockedMemberStep),
		ExtraFactors = [LockedMember])]
	JuniorExocetLockedMember,

	/// <summary>
	/// Indicates junior exocet mirror sync.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(JuniorExocetMirrorSyncStep),
		ExtraFactors = [Mirror])]
	JuniorExocetMirrorSync,

	/// <summary>
	/// Indicates senior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueGroup(TechniqueGroup.Exocet)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[Abbreviation("SE")]
	[BoundStepTypes(typeof(ExocetBaseStep))]
	[StaticDifficulty(9.6)]
	SeniorExocet,

	/// <summary>
	/// Indicates senior exocet mirror conjugate pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror])]
	SeniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates senior exocet locked member.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ExocetLockedMemberStep),
		ExtraFactors = [LockedMember])]
	SeniorExocetLockedMember,

	/// <summary>
	/// Indicates senior exocet true base.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(SeniorExocetTrueBaseStep),
		ExtraFactors = [TrueBase])]
	SeniorExocetTrueBase,

	/// <summary>
	/// Indicates weak exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/weak-exocet-t39651.html")]
	[TechniqueGroup(TechniqueGroup.Exocet)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[Abbreviation("WE")]
	[BoundStepTypes(typeof(WeakExocetStep))]
	[StaticDifficulty(9.7)]
	[ExtraDifficultyFactors(MissingStabilityBalancer)]
	WeakExocet,

	/// <summary>
	/// Indicates weak exocet adjacent target.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/weak-exocet-t39651.html")]
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetAdjacentTargetStep),
		ExtraFactors = [Mirror])]
	WeakExocetAdjacentTarget,

	/// <summary>
	/// Indicates weak exocet slash.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/weak-exocet-t39651.html")]
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetSlashStep),
		ExtraFactors = [SlashElimination])]
	WeakExocetSlash,

	/// <summary>
	/// Indicates weak exocet BZ rectangle.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/weak-exocet-t39651.html")]
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetBzRectangleStep),
		ExtraFactors = [BzRectangle])]
	WeakExocetBzRectangle,

	/// <summary>
	/// Indicates lame weak exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[ReferenceLink("http://forum.enjoysudoku.com/weak-exocet-t39651.html")]
	[TechniqueMetadata(
		9.7, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(WeakExocetStep),
		ExtraFactors = [MissingStabilityBalancer])]
	LameWeakExocet,

	/// <summary>
	/// Indicates franken junior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep))]
	FrankenJuniorExocet,

	/// <summary>
	/// Indicates franken junior exocet locked member.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember])]
	FrankenJuniorExocetLockedMember,

	/// <summary>
	/// Indicates franken junior exocet adjacent target.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetAdjacentTargetStep),
		ExtraFactors = [Mirror])]
	FrankenJuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates franken junior exocet mirror conjugate pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror])]
	FrankenJuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates mutant junior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep))]
	MutantJuniorExocet,

	/// <summary>
	/// Indicates mutant junior exocet locked member.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember])]
	MutantJuniorExocetLockedMember,

	/// <summary>
	/// Indicates mutant junior exocet adjacent target.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetAdjacentTargetStep),
		ExtraFactors = [Mirror])]
	MutantJuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates mutant junior exocet mirror conjugate pair.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexJuniorExocetMirrorConjugatePairStep),
		ExtraFactors = [Mirror])]
	MutantJuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates franken senior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep))]
	FrankenSeniorExocet,

	/// <summary>
	/// Indicates franken senior exocet locked member.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		10.0, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember])]
	FrankenSeniorExocetLockedMember,

	/// <summary>
	/// Indicates advanced franken senior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(9.8, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(AdvancedComplexSeniorExocetStep))]
	AdvancedFrankenSeniorExocet,

	/// <summary>
	/// Indicates mutant senior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(10.2, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetBaseStep))]
	MutantSeniorExocet,

	/// <summary>
	/// Indicates mutant senior exocet locked member.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		10.2, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [LockedMember])]
	MutantSeniorExocetLockedMember,

	/// <summary>
	/// Indicates advanced mutant senior exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(10.1, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(AdvancedComplexSeniorExocetStep))]
	AdvancedMutantSeniorExocet,

	/// <summary>
	/// Indicates double exocet.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(DoubleExocetBaseStep))]
	DoubleExocet,

	/// <summary>
	/// Indicates double exocet uni-fish pattern.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueMetadata(
		9.4, DifficultyLevel.Nightmare, TechniqueGroup.Exocet, typeof(DoubleExocetGeneralizedFishStep),
		ExtraFactors = [GeneralizedFish])]
	DoubleExocetGeneralizedFish,

	/// <summary>
	/// Indicates pattern-locked quadruple. This quadruple is a special quadruple: it can only be concluded after both JE and SK-Loop are formed.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/jexocet-compendium-t32370.html")]
	[TechniqueGroup(TechniqueGroup.Exocet)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[Abbreviation("PLQ")]
	PatternLockedQuadruple,
	#endregion

	//
	// Domino Loop
	//
	#region Domino Loop
	/// <summary>
	/// Indicates domino loop.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/domino-loops-sk-loops-beyond-t32789.html")]
	[TechniqueGroup(TechniqueGroup.DominoLoop)]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[BoundStepTypes(typeof(DominoLoopStep))]
	[StaticDifficulty(9.6)]
	DominoLoop,
	#endregion

	//
	// Multi-sector Locked Sets
	//
	#region Multi-sector Locked Sets
	/// <summary>
	/// Indicates multi-sector locked sets.
	/// </summary>
	[ReferenceLink("http://forum.enjoysudoku.com/exotic-patterns-a-resume-t30508-270.html")]
	[TechniqueGroup(TechniqueGroup.MultisectorLockedSets)]
	[Abbreviation("MSLS")]
	[TechniqueFeature(TechniqueFeature.HardToBeGenerated)]
	[StaticDifficultyLevel(DifficultyLevel.Nightmare)]
	[BoundStepTypes(typeof(MultisectorLockedSetsStep))]
	[StaticDifficulty(9.4)]
	MultisectorLockedSets,
	#endregion

	//
	// Pattern Overlay
	//
	#region Pattern Overlay
	/// <summary>
	/// Indicates pattern overlay method.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Pattern_Overlay_Method.html")]
	[TechniqueGroup(TechniqueGroup.PatternOverlay)]
	[Abbreviation("POM")]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.LastResort)]
	[BoundStepTypes(typeof(PatternOverlayStep))]
	[StaticDifficulty(8.5)]
	PatternOverlay,
	#endregion

	//
	// Templating
	//
	#region Templating
	/// <summary>
	/// Indicates template set.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Templating.html")]
	[Hodoku(10000, HodokuDifficultyLevel.Extreme, Prefix = "1201")]
	[TechniqueGroup(TechniqueGroup.Templating)]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.LastResort)]
	[BoundStepTypes(typeof(TemplateStep))]
	[StaticDifficulty(9.0)]
	TemplateSet,

	/// <summary>
	/// Indicates template delete.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Templating.html")]
	[Hodoku(10000, HodokuDifficultyLevel.Extreme, Prefix = "1202")]
	[TechniqueGroup(TechniqueGroup.Templating)]
	[TechniqueFeature(TechniqueFeature.WillBeReplacedByOtherTechnique)]
	[StaticDifficultyLevel(DifficultyLevel.LastResort)]
	[BoundStepTypes(typeof(TemplateStep))]
	[StaticDifficulty(9.0)]
	TemplateDelete,
	#endregion

	//
	// Bowman's Bingo
	//
	#region Bowman's Bingo
	/// <summary>
	/// Indicates bowman's bingo.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Bowman_Bingo.html")]
	[TechniqueMetadata(
		8.0, DifficultyLevel.LastResort, TechniqueGroup.BowmanBingo, typeof(BowmanBingoStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique, ExtraFactors = [Length])]
	BowmanBingo,
	#endregion

	//
	// Brute Force
	//
	#region Brute Force
	/// <summary>
	/// Indicates brute force.
	/// </summary>
	[ReferenceLink("http://sudopedia.enjoysudoku.com/Trial_%26_Error.html")]
	[HodokuDifficultyRating(10000, HodokuDifficultyLevel.Extreme)]
	[SudokuExplainerNames("Try & Error")]
	[TechniqueGroup(TechniqueGroup.BruteForce)]
	[Abbreviation("BF")]
	[TechniqueFeature(TechniqueFeature.OnlyExistInTheory)]
	[StaticDifficultyLevel(DifficultyLevel.LastResort)]
	[BoundStepTypes(typeof(BruteForceStep))]
	[StaticDifficulty((double)AnalyzerResult.MaximumRatingValueTheory)]
	BruteForce,
	#endregion
}
