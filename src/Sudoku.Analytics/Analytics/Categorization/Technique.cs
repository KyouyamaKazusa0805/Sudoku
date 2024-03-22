#undef COMPATIBLE_ORIGINAL_TECHNIQUE_RULES

namespace Sudoku.Analytics.Categorization;

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
	[Hodoku(Rating = 4, DifficultyLevel = HodokuDifficultyLevel.Easy, Prefix = "0000")]
	[SudokuExplainer(Rating = 1.0, TechniqueDefined = SudokuExplainerTechnique.Single, Aliases = ["Single"])]
	[TechniqueMetadata(
		Rating = 1.0,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(FullHouseStep),
		StepSearcherType = typeof(SingleStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Full_House.html"])]
	FullHouse,

	/// <summary>
	/// Indicates last digit.
	/// </summary>
	[Hodoku(Prefix = "0001")]
	[TechniqueMetadata(
		Rating = 1.1,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(LastDigitStep),
		SecondaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Last_Digit.html"])]
	LastDigit,

	/// <summary>
	/// Indicates hidden single (in block).
	/// </summary>
	[Hodoku(Rating = 14, DifficultyLevel = HodokuDifficultyLevel.Easy, Prefix = "0002")]
	[SudokuExplainer(Rating = 1.2, TechniqueDefined = SudokuExplainerTechnique.HiddenSingle)]
	[TechniqueMetadata(
		Rating = 1.9,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Indirect,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"])]
	HiddenSingleBlock,

	/// <summary>
	/// Indicates hidden single (in row).
	/// </summary>
	[Hodoku(Rating = 14, DifficultyLevel = HodokuDifficultyLevel.Easy, Prefix = "0002")]
	[SudokuExplainer(Rating = 1.5, TechniqueDefined = SudokuExplainerTechnique.HiddenSingle)]
	[TechniqueMetadata(
		Rating = 2.3,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Indirect,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"])]
	HiddenSingleRow,

	/// <summary>
	/// Indicates hidden single (in column).
	/// </summary>
	[Hodoku(Rating = 14, DifficultyLevel = HodokuDifficultyLevel.Easy, Prefix = "0002")]
	[SudokuExplainer(Rating = 1.5, TechniqueDefined = SudokuExplainerTechnique.HiddenSingle)]
	[TechniqueMetadata(
		Rating = 2.3,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Indirect,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"])]
	HiddenSingleColumn,

	/// <summary>
	/// Indicates naked single.
	/// </summary>
	[Hodoku(Rating = 4, DifficultyLevel = HodokuDifficultyLevel.Easy, Prefix = "0003")]
	[SudokuExplainer(Rating = 2.3, TechniqueDefined = SudokuExplainerTechnique.NakedSingle)]
	[TechniqueMetadata(
		Rating = 2.3,
		DirectRating = 1.0,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(NakedSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Single.html"])]
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
		Rating = 1.2,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"],
		Features = TechniqueFeature.DirectTechniques)]
	CrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in row, equivalent to hidden single in row, but used in direct views.
	/// </summary>
	[TechniqueMetadata(
		Rating = 1.5,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Single.html"],
		Features = TechniqueFeature.DirectTechniques)]
	CrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in column, equivalent to hidden single in column, but used in direct views.
	/// </summary>
	[TechniqueMetadata(
		Rating = 1.5,
		DifficultyLevel = DifficultyLevel.Easy,
		ContainingGroup = TechniqueGroup.Single,
		PrimaryStepType = typeof(HiddenSingleStep),
		StepSearcherType = typeof(SingleStepSearcher),
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
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	PointingFullHouse,

	/// <summary>
	/// Indicates full house, with claiming.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	ClaimingFullHouse,

	/// <summary>
	/// Indicates full house, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairFullHouse,

	/// <summary>
	/// Indicates full house, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairPlusFullHouse,

	/// <summary>
	/// Indicates full house, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenPairFullHouse,

	/// <summary>
	/// Indicates full house, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedPairFullHouse,

	/// <summary>
	/// Indicates full house, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenPairFullHouse,

	/// <summary>
	/// Indicates full house, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTripleFullHouse,

	/// <summary>
	/// Indicates full house, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriplePlusFullHouse,

	/// <summary>
	/// Indicates full house, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenTripleFullHouse,

	/// <summary>
	/// Indicates full house, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedTripleFullHouse,

	/// <summary>
	/// Indicates full house, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenTripleFullHouse,

	/// <summary>
	/// Indicates full house, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadrupleFullHouse,

	/// <summary>
	/// Indicates full house, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadruplePlusFullHouse,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenQuadrupleFullHouse,

	/// <summary>
	/// Indicates crosshatching in block, with pointing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	PointingCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with claiming.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	ClaimingCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairPlusCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenPairCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriplePlusCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenTripleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadrupleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in block, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadruplePlusCrosshatchingBlock,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenQuadrupleCrosshatchingBlock,

	/// <summary>
	/// Indicates crosshatching in row, with pointing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	PointingCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with claiming.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	ClaimingCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairPlusCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenPairCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriplePlusCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenTripleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadrupleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in row, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadruplePlusCrosshatchingRow,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenQuadrupleCrosshatchingRow,

	/// <summary>
	/// Indicates crosshatching in column, with pointing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	PointingCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with claiming.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	ClaimingCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairPlusCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenPairCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriplePlusCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenTripleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadrupleCrosshatchingColumn,

	/// <summary>
	/// Indicates crosshatching in column, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadruplePlusCrosshatchingColumn,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenQuadrupleCrosshatchingColumn,

	/// <summary>
	/// Indicates naked single, with pointing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	PointingNakedSingle,

	/// <summary>
	/// Indicates naked single, with claiming.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectIntersectionStep),
		StepSearcherType = typeof(DirectIntersectionStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	ClaimingNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked pair (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedPairPlusNakedSingle,

	/// <summary>
	/// Indicates naked single, with hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked hidden pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenPairNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked triple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriplePlusNakedSingle,

	/// <summary>
	/// Indicates naked single, with hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with locked hidden triple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenTripleNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadrupleNakedSingle,

	/// <summary>
	/// Indicates naked single, with naked quadruple (+).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.3,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedQuadruplePlusNakedSingle,

	/// <summary>
	/// Indicates full house, with hidden quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.7,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.ComplexSingle,
		PrimaryStepType = typeof(DirectSubsetStep),
		StepSearcherType = typeof(DirectSubsetStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Features = TechniqueFeature.DirectTechniques,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenQuadrupleNakedSingle,
	#endregion

	//
	// Locked Candidates
	//
	#region Locked Candidates
	/// <summary>
	/// Indicates pointing.
	/// </summary>
	[Hodoku(
		Rating = 50,
		DifficultyLevel = HodokuDifficultyLevel.Medium,
		Prefix = "0100",
		Aliases = ["Locked Candidates Type 1"])]
	[SudokuExplainer(Rating = 2.6, TechniqueDefined = SudokuExplainerTechnique.Pointing)]
	[TechniqueMetadata(
		Rating = 2.6,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.LockedCandidates,
		PrimaryStepType = typeof(LockedCandidatesStep),
		StepSearcherType = typeof(LockedCandidatesStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Locked_Candidates.html"])]
	Pointing,

	/// <summary>
	/// Indicates claiming.
	/// </summary>
	[Hodoku(
		Rating = 50,
		DifficultyLevel = HodokuDifficultyLevel.Medium,
		Prefix = "0101",
		Aliases = ["Locked Candidates Type 2"])]
	[SudokuExplainer(Rating = 2.8, TechniqueDefined = SudokuExplainerTechnique.Claiming)]
	[TechniqueMetadata(
		Rating = 2.8,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.LockedCandidates,
		PrimaryStepType = typeof(LockedCandidatesStep),
		StepSearcherType = typeof(LockedCandidatesStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Locked_Candidates.html"])]
	Claiming,

	/// <summary>
	/// Indicates law of leftover.
	/// </summary>
	[TechniqueMetadata(
		Rating = 2.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.LockedCandidates,
		PrimaryStepType = typeof(LawOfLeftoverStep),
		StepSearcherType = typeof(LawOfLeftoverStepSearcher),
		PencilmarkVisibility = PencilmarkVisibility.Direct,
		Abbreviation = "LoL",
		Features = TechniqueFeature.DirectTechniques,
		Links = ["http://sudopedia.enjoysudoku.com/Law_of_Leftovers.html"])]
	LawOfLeftover,
	#endregion

	//
	// Subsets
	//
	#region Subsets
	/// <summary>
	/// Indicates naked pair.
	/// </summary>
	[Hodoku(Rating = 60, DifficultyLevel = HodokuDifficultyLevel.Medium, Prefix = "0200")]
	[SudokuExplainer(Rating = 3.0, TechniqueDefined = SudokuExplainerTechnique.NakedPair)]
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Pair.html"])]
	NakedPair,

	/// <summary>
	/// Indicates naked pair plus (naked pair (+)).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Pair.html"])]
	NakedPairPlus,

	/// <summary>
	/// Indicates locked pair.
	/// </summary>
	[Hodoku(Rating = 40, DifficultyLevel = HodokuDifficultyLevel.Medium, Prefix = "0110-1")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(Rating = 2.0, TechniqueDefined = SudokuExplainerTechnique.DirectHiddenPair, Aliases = ["Direct Hidden Pair"])]
#endif
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(LockedSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Locked_Pair.html"])]
	LockedPair,

	/// <summary>
	/// Indicates hidden pair.
	/// </summary>
	[Hodoku(Rating = 70, DifficultyLevel = HodokuDifficultyLevel.Medium, Prefix = "0210")]
	[SudokuExplainer(Rating = 3.4, TechniqueDefined = SudokuExplainerTechnique.HiddenPair)]
	[TechniqueMetadata(
		Rating = 3.4,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(HiddenSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Pair.html"])]
	HiddenPair,

	/// <summary>
	/// Indicates locked hidden pair.
	/// </summary>
	[Hodoku(Prefix = "0110-2")]
	[TechniqueMetadata(
		Rating = 3.4,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(HiddenSubsetStep),
		StepSearcherType = typeof(LockedSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenPair,

	/// <summary>
	/// Indicates naked triple.
	/// </summary>
	[Hodoku(Rating = 80, DifficultyLevel = HodokuDifficultyLevel.Medium, Prefix = "0201")]
	[SudokuExplainer(Rating = 3.6, TechniqueDefined = SudokuExplainerTechnique.NakedTriplet, Aliases = ["Naked Triplet"])]
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Triple.html"],
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriple,

	/// <summary>
	/// Indicates naked triple plus (naked triple (+)).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Triple.html"],
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	NakedTriplePlus,

	/// <summary>
	/// Indicates locked triple.
	/// </summary>
	[Hodoku(Rating = 60, DifficultyLevel = HodokuDifficultyLevel.Medium, Prefix = "0111-1")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(Rating = 2.5, TechniqueDefined = SudokuExplainerTechnique.DirectHiddenTriplet, Aliases = ["Direct Hidden Triplet"])]
#endif
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(LockedSubsetStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Locked_Triple.html"],
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedTriple,

	/// <summary>
	/// Indicates hidden triple.
	/// </summary>
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Medium, Prefix = "0211")]
	[SudokuExplainer(Rating = 4.0, TechniqueDefined = SudokuExplainerTechnique.HiddenTriplet, Aliases = ["Hidden Triplet"])]
	[TechniqueMetadata(
		Rating = 3.4,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(HiddenSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Triple.html"],
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	HiddenTriple,

	/// <summary>
	/// Indicates locked hidden triple.
	/// </summary>
	[Hodoku(Prefix = "0111-2")]
	[TechniqueMetadata(
		Rating = 3.4,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(HiddenSubsetStep),
		StepSearcherType = typeof(LockedSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked])]
	LockedHiddenTriple,

	/// <summary>
	/// Indicates naked quadruple.
	/// </summary>
	[Hodoku(Rating = 120, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0202")]
	[SudokuExplainer(Rating = 5.0, TechniqueDefined = SudokuExplainerTechnique.NakedQuad, Aliases = ["Naked Quad"])]
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Quad.html"])]
	NakedQuadruple,

	/// <summary>
	/// Indicates naked quadruple plus (naked quadruple (+)).
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.0,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(NakedSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Naked_Quad.html"])]
	NakedQuadruplePlus,

	/// <summary>
	/// Indicates hidden quadruple.
	/// </summary>
	[Hodoku(Rating = 150, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0212")]
	[SudokuExplainer(Rating = 5.4, TechniqueDefined = SudokuExplainerTechnique.HiddenQuad, Aliases = ["Hidden Quad"])]
	[TechniqueMetadata(
		Rating = 3.4,
		DifficultyLevel = DifficultyLevel.Moderate,
		ContainingGroup = TechniqueGroup.Subset,
		PrimaryStepType = typeof(HiddenSubsetStep),
		StepSearcherType = typeof(NormalSubsetStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Locked],
		Links = ["http://sudopedia.enjoysudoku.com/Hidden_Quad.html"])]
	HiddenQuadruple,
	#endregion

	//
	// Fishes
	//
	#region Fishes
	/// <summary>
	/// Indicates X-Wing.
	/// </summary>
	[Hodoku(Rating = 140, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0300")]
	[SudokuExplainer(Rating = 3.2, TechniqueDefined = SudokuExplainerTechnique.XWing)]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://sudopedia.enjoysudoku.com/X-Wing.html", "http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	XWing,

	/// <summary>
	/// Indicates finned X-Wing.
	/// </summary>
	[Hodoku(Rating = 130, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0310")]
	[SudokuExplainer(RatingValueAdvanced = [3.4])]
	[TechniqueMetadata(
		Rating = 3.2, DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
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
	[Hodoku(Rating = 150, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0320")]
	[SudokuExplainer(RatingValueAdvanced = [3.5])]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedXWing,

	/// <summary>
	/// Indicates Siamese sashimi X-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiXWing,

	/// <summary>
	/// Indicates franken X-Wing.
	/// </summary>
	[Hodoku(Rating = 300, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0330")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenXWing,

	/// <summary>
	/// Indicates finned franken X-Wing.
	/// </summary>
	[Hodoku(Rating = 390, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0340")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenXWing,

	/// <summary>
	/// Indicates Siamese finned franken X-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	SiameseFinnedFrankenXWing,

	/// <summary>
	/// Indicates Siamese sashimi franken X-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiFrankenXWing,

	/// <summary>
	/// Indicates mutant X-Wing.
	/// </summary>
	[Hodoku(Rating = 450, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0350")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantXWing,

	/// <summary>
	/// Indicates finned mutant X-Wing.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0360")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	FinnedMutantXWing,

	/// <summary>
	/// Indicates sashimi mutant X-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantXWing,

	/// <summary>
	/// Indicates Siamese finned mutant X-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	SiameseFinnedMutantXWing,

	/// <summary>
	/// Indicates Siamese sashimi mutant X-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantXWing,

	/// <summary>
	/// Indicates swordfish.
	/// </summary>
	[Hodoku(Rating = 150, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0301")]
	[SudokuExplainer(Rating = 3.8, TechniqueDefined = SudokuExplainerTechnique.Swordfish)]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Swordfish.html"])]
	Swordfish,

	/// <summary>
	/// Indicates finned swordfish.
	/// </summary>
	[Hodoku(Rating = 200, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0311")]
	[SudokuExplainer(RatingValueAdvanced = [4.0])]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Finned_Fish.html",
			"http://sudopedia.enjoysudoku.com/Finned_Swordfish.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=2793"
		])]
	FinnedSwordfish,

	/// <summary>
	/// Indicates sashimi swordfish.
	/// </summary>
	[Hodoku(
		Rating = 240,
		DifficultyLevel = HodokuDifficultyLevel.Unfair,
		Prefix = "0321")]
	[SudokuExplainer(RatingValueAdvanced = [4.1])]
	[TechniqueMetadata(
		Rating = 3.2, DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html",
			"http://sudopedia.enjoysudoku.com/Sashimi_Swordfish.html"
		])]
	SashimiSwordfish,

	/// <summary>
	/// Indicates Siamese finned swordfish.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedSwordfish,

	/// <summary>
	/// Indicates Siamese sashimi swordfish.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiSwordfish,

	/// <summary>
	/// Indicates swordfish.
	/// </summary>
	[Hodoku(
		Rating = 350,
		DifficultyLevel = HodokuDifficultyLevel.Unfair,
		Prefix = "0331")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
	[Hodoku(Rating = 410, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0341")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Swordfish.html"
		])]
	SiameseSashimiFrankenSwordfish,

	/// <summary>
	/// Indicates mutant swordfish.
	/// </summary>
	[Hodoku(Rating = 450, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0351")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	MutantSwordfish,

	/// <summary>
	/// Indicates finned mutant swordfish.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0361")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Swordfish.html"
		])]
	SiameseSashimiMutantSwordfish,

	/// <summary>
	/// Indicates jellyfish.
	/// </summary>
	[Hodoku(Rating = 160, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0302")]
	[SudokuExplainer(Rating = 5.2, TechniqueDefined = SudokuExplainerTechnique.Jellyfish)]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Jellyfish.html"])]
	Jellyfish,

	/// <summary>
	/// Indicates finned jellyfish.
	/// </summary>
	[Hodoku(Rating = 250, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0312")]
	[SudokuExplainer(RatingValueAdvanced = [5.4])]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
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
	[Hodoku(Rating = 260, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0322")]
	[SudokuExplainer(RatingValueAdvanced = [5.6])]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedJellyfish,

	/// <summary>
	/// Indicates Siamese sashimi jellyfish.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.NormalFish,
		PrimaryStepType = typeof(NormalFishStep),
		StepSearcherType = typeof(NormalFishStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Sashimi],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiJellyfish,

	/// <summary>
	/// Indicates franken jellyfish.
	/// </summary>
	[Hodoku(Rating = 370, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0332")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	FrankenJellyfish,

	/// <summary>
	/// Indicates finned franken jellyfish.
	/// </summary>
	[Hodoku(Rating = 430, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0342")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html",
			"http://sudopedia.enjoysudoku.com/Franken_Jellyfish.html"
		])]
	SiameseSashimiFrankenJellyfish,

	/// <summary>
	/// Indicates mutant jellyfish.
	/// </summary>
	[Hodoku(Rating = 450, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0352")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	MutantJellyfish,

	/// <summary>
	/// Indicates finned mutant jellyfish.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0362")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Jellyfish.html"
		])]
	SiameseSashimiMutantJellyfish,

	/// <summary>
	/// Indicates squirmbag.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0303")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	Squirmbag,

	/// <summary>
	/// Indicates finned squirmbag.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0313")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
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
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0323")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html"])]
	SashimiSquirmbag,

	/// <summary>
	/// Indicates Siamese finned squirmbag.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseFinnedSquirmbag,

	/// <summary>
	/// Indicates Siamese sashimi squirmbag.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html"])]
	SiameseSashimiSquirmbag,

	/// <summary>
	/// Indicates franken squirmbag.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0333")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		Features = TechniqueFeature.OnlyExistInTheory,
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenSquirmbag,

	/// <summary>
	/// Indicates finned franken squirmbag.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0343")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates Siamese finned franken squirmbag.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Franken_Fish.html"
		])]
	SiameseFinnedFrankenSquirmbag,

	/// <summary>
	/// Indicates Siamese sashimi franken squirmbag.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SiameseSashimiFrankenSquirmbag,

	/// <summary>
	/// Indicates mutant squirmbag.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0353")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		Features = TechniqueFeature.OnlyExistInTheory,
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantSquirmbag,

	/// <summary>
	/// Indicates finned mutant squirmbag.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0363")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = [
			"http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"
		])]
	FinnedMutantSquirmbag,

	/// <summary>
	/// Indicates sashimi mutant squirmbag.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantSquirmbag,

	/// <summary>
	/// Indicates Siamese finned mutant squirmbag.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantSquirmbag,

	/// <summary>
	/// Indicates whale.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0304")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	Whale,

	/// <summary>
	/// Indicates finned whale.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0314")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
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
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0324")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html"])]
	SashimiWhale,

	/// <summary>
	/// Indicates Siamese finned whale.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseFinnedWhale,

	/// <summary>
	/// Indicates Siamese sashimi whale.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseSashimiWhale,

	/// <summary>
	/// Indicates franken whale.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0334")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		Features = TechniqueFeature.OnlyExistInTheory,
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenWhale,

	/// <summary>
	/// Indicates finned franken whale.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0344")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenWhale,

	/// <summary>
	/// Indicates Siamese finned franken whale.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SiameseSashimiFrankenWhale,

	/// <summary>
	/// Indicates mutant whale.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0354")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		Features = TechniqueFeature.OnlyExistInTheory,
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantWhale,

	/// <summary>
	/// Indicates finned mutant whale.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0364")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html",
			"http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantWhale,

	/// <summary>
	/// Indicates Siamese finned mutant whale.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SiameseSashimiMutantWhale,

	/// <summary>
	/// Indicates leviathan.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0305")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	Leviathan,

	/// <summary>
	/// Indicates finned leviathan.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0315")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
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
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0325")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Sashimi_Fish.html",])]
	SashimiLeviathan,

	/// <summary>
	/// Indicates Siamese finned leviathan.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2793"])]
	SiameseFinnedLeviathan,

	/// <summary>
	/// Indicates Siamese sashimi leviathan.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.NormalFish,
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html"])]
	SiameseSashimiLeviathan,

	/// <summary>
	/// Indicates franken leviathan.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0335")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		Features = TechniqueFeature.OnlyExistInTheory,
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	FrankenLeviathan,

	/// <summary>
	/// Indicates finned franken leviathan.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0345")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SashimiFrankenLeviathan,

	/// <summary>
	/// Indicates Siamese finned franken leviathan.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Franken_Fish.html"])]
	SiameseSashimiFrankenLeviathan,

	/// <summary>
	/// Indicates mutant leviathan.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0355")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		Features = TechniqueFeature.OnlyExistInTheory,
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	MutantLeviathan,

	/// <summary>
	/// Indicates finned mutant leviathan.
	/// </summary>
	[Hodoku(Rating = 470, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "0365")]
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
		Links = ["http://forum.enjoysudoku.com/the-ultimate-fish-guide-t4993.html", "http://sudopedia.enjoysudoku.com/Mutant_Fish.html"])]
	SashimiMutantLeviathan,

	/// <summary>
	/// Indicates Siamese finned mutant leviathan.
	/// </summary>
	[TechniqueMetadata(
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
		Rating = 3.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ComplexFish,
		PrimaryStepType = typeof(ComplexFishStep),
		StepSearcherType = typeof(ComplexFishStepSearcher),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Sashimi,
			ExtraDifficultyFactorNames.FishShape,
			ExtraDifficultyFactorNames.Cannibalism
		],
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
	[Hodoku(Rating = 160, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0800")]
	[SudokuExplainer(Rating = 4.2, TechniqueDefined = SudokuExplainerTechnique.XyWing)]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/XY-Wing.html"])]
	XyWing,

	/// <summary>
	/// Indicates XYZ-Wing.
	/// </summary>
	[Hodoku(Rating = 180, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0801")]
	[SudokuExplainer(Rating = 4.4, TechniqueDefined = SudokuExplainerTechnique.XyzWing)]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/XYZ-Wing.html"])]
	XyzWing,

	/// <summary>
	/// Indicates WXYZ-Wing.
	/// </summary>
	[Hodoku(Prefix = "0802")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.XyzWing, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/WXYZ-Wing.html"])]
	WxyzWing,

	/// <summary>
	/// Indicates VWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	VwxyzWing,

	/// <summary>
	/// Indicates UVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	UvwxyzWing,

	/// <summary>
	/// Indicates TUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	TuvwxyzWing,

	/// <summary>
	/// Indicates STUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	StuvwxyzWing,

	/// <summary>
	/// Indicates RSTUVWXYZ-Wing.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [double.NaN])]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	RstuvwxyzWing,

	/// <summary>
	/// Indicates incomplete WXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/WXYZ-Wing.html"])]
	IncompleteWxyzWing,

	/// <summary>
	/// Indicates incomplete VWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	IncompleteVwxyzWing,

	/// <summary>
	/// Indicates incomplete UVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	IncompleteUvwxyzWing,

	/// <summary>
	/// Indicates incomplete TUVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	IncompleteTuvwxyzWing,

	/// <summary>
	/// Indicates incomplete STUVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	IncompleteStuvwxyzWing,

	/// <summary>
	/// Indicates incomplete RSTUVWXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RegularWing,
		PrimaryStepType = typeof(RegularWingStep),
		StepSearcherType = typeof(RegularWingStepSearcher),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Incompleteness])]
	IncompleteRstuvwxyzWing,
	#endregion

	//
	// Irregular Wings
	//
	#region Irregular Wings
	/// <summary>
	/// Indicates W-Wing.
	/// </summary>
	[Hodoku(Rating = 150, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0803")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.WWing, RatingValueAdvanced = [4.4])]
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(WWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/W-Wing.html"])]
	WWing,

	/// <summary>
	/// Indicates Multi-Branch W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(MultiBranchWWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
		ExtraFactors = [ExtraDifficultyFactorNames.Size],
		Links = ["http://sudopedia.enjoysudoku.com/W-Wing.html"])]
	MultiBranchWWing,

	/// <summary>
	/// Indicates grouped W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(WWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
		Links = ["http://sudopedia.enjoysudoku.com/W-Wing.html"])]
	GroupedWWing,

	/// <summary>
	/// Indicates M-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(MWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher))]
	MWing,

	/// <summary>
	/// Indicates grouped M-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(MWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher))]
	GroupedMWing,

	/// <summary>
	/// Indicates S-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(SWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher))]
	SWing,

	/// <summary>
	/// Indicates grouped S-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(SWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher))]
	GroupedSWing,

	/// <summary>
	/// Indicates local wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.8,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(LWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
		Links = ["http://forum.enjoysudoku.com/local-wing-t34685.html"])]
	LWing,

	/// <summary>
	/// Indicates grouped local wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.8,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(LWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
		Links = ["http://forum.enjoysudoku.com/local-wing-t34685.html"])]
	GroupedLWing,

	/// <summary>
	/// Indicates hybrid wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(HWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
		Links = ["http://forum.enjoysudoku.com/hybrid-wings-work-in-progress-t34212.html"])]
	HWing,

	/// <summary>
	/// Indicates grouped hybrid wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.IrregularWing,
		PrimaryStepType = typeof(HWingStep),
		StepSearcherType = typeof(IrregularWingStepSearcher),
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
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AlmostLockedCandidates,
		PrimaryStepType = typeof(AlmostLockedCandidatesStep),
		Abbreviation = "ALP",
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedPair,

	/// <summary>
	/// Indicates almost locked triple.
	/// </summary>
	[SudokuExplainer(RatingValueAdvanced = [5.2])]
	[TechniqueMetadata(
		Rating = 5.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AlmostLockedCandidates,
		PrimaryStepType = typeof(AlmostLockedCandidatesStep),
		Abbreviation = "ALT",
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedTriple,

	/// <summary>
	/// Indicates almost locked quadruple.
	/// The technique may not be useful because it'll be replaced with Sue de Coq.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AlmostLockedCandidates,
		PrimaryStepType = typeof(AlmostLockedCandidatesStep),
		Abbreviation = "ALQ",
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.ValueCell],
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedQuadruple,

	/// <summary>
	/// Indicates almost locked triple value type.
	/// The technique may not be often used because it'll be replaced with some kinds of Sue de Coq.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AlmostLockedCandidates,
		PrimaryStepType = typeof(AlmostLockedCandidatesStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.ValueCell],
		Links = ["http://sudopedia.enjoysudoku.com/Almost_Locked_Candidates.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4477"])]
	AlmostLockedTripleValueType,

	/// <summary>
	/// Indicates almost locked quadruple value type.
	/// The technique may not be often used because it'll be replaced with some kinds of Sue de Coq.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AlmostLockedCandidates,
		PrimaryStepType = typeof(AlmostLockedCandidatesStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.ValueCell],
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
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ExtendedSubsetPrinciple,
		PrimaryStepType = typeof(ExtendedSubsetPrincipleStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Size],
		Abbreviation = "ESP",
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
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0600", Aliases = ["Uniqueness Test 1"])]
	[SudokuExplainer(Rating = 4.5, TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle)]
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleType1Step),
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
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0601", Aliases = ["Uniqueness Test 2"])]
	[SudokuExplainer(Rating = 4.5, TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleType2Step),
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
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0602", Aliases = ["Uniqueness Test 3"])]
	[SudokuExplainer(
		TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle, RatingValueOriginal = [4.5, 4.8],
		RatingValueAdvanced = [4.6, 4.9])]
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Hidden],
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
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0603", Aliases = ["Uniqueness Test 4"])]
	[SudokuExplainer(Rating = 4.5, TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		Rating = 4.4, DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair],
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
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0604", Aliases = ["Uniqueness Test 5"])]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleType2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType5,

	/// <summary>
	/// Indicates unique rectangle type 6.
	/// </summary>
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0605", Aliases = ["Uniqueness Test 6"])]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.6])]
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleType6,

	/// <summary>
	/// Indicates hidden unique rectangle.
	/// </summary>
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0606", Aliases = ["Hidden Rectangle"])]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.UniqueRectangle, RatingValueAdvanced = [4.8])]
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(HiddenUniqueRectangleStep),
		Abbreviation = "HUR",
		SecondaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangle2DOr3XStep),
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangle2DOr3XStep),
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
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
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
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
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
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
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
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
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
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
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
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangle4C3,

	/// <summary>
	/// Indicates unique rectangle burred subset.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleBurredSubsetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Size], Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html",
			"http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleBurredSubset,

	/// <summary>
	/// Indicates unique rectangle-XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.WingSize],
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
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.WingSize],
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
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.WingSize],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleWxyzWing,

	/// <summary>
	/// Indicates unique rectangle W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
#if UNIQUE_RECTANGLE_W_WING
		PrimarySupportedType = typeof(UniqueRectangleWWingStep),
#endif
#if !UNIQUE_RECTANGLE_W_WING
		Features = TechniqueFeature.NotImplemented,
#endif
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable], Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleWWing,

	/// <summary>
	/// Indicates unique rectangle sue de coq.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithSueDeCoqStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Isolated,
		ExtraDifficultyFactorNames.Cannibalism,
		ExtraDifficultyFactorNames.Avoidable],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleSueDeCoq,

	/// <summary>
	/// Indicates unique rectangle baba grouping.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.9,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithBabaGroupingStep),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleBabaGrouping,

	/// <summary>
	/// Indicates unique rectangle external type 1.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType1Or2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType1,

	/// <summary>
	/// Indicates unique rectangle external type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType1Or2Step),
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType2,

	/// <summary>
	/// Indicates unique rectangle external type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Avoidable,
		ExtraDifficultyFactorNames.Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType3,

	/// <summary>
	/// Indicates unique rectangle external type 4.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType4Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalType4,

	/// <summary>
	/// Indicates unique rectangle external turbot fish.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalTurbotFishStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalTurbotFish,

	/// <summary>
	/// Indicates unique rectangle external W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.8,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalWWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Avoidable,
		ExtraDifficultyFactorNames.Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalWWing,

	/// <summary>
	/// Indicates unique rectangle external XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalXyWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Avoidable,
		ExtraDifficultyFactorNames.Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
			"http://forum.enjoysudoku.com/unique-rectangles-gallery-t33752.html"
		])]
	UniqueRectangleExternalXyWing,

	/// <summary>
	/// Indicates unique rectangle external almost locked sets XZ rule.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.8,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalAlmostLockedSetsXzStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Avoidable,
		ExtraDifficultyFactorNames.Incompleteness],
		Links = [
			"http://sudopedia.enjoysudoku.com/Unique_Rectangle.html", "http://forum.enjoysudoku.com/uniqueness-type-6-ur-meets-x-wing-t3709-30.html#p26448",
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
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0607")]
	[SudokuExplainer(
		TechniqueDefined = SudokuExplainerTechnique.AvoidableRectangle,
		RatingValueAdvanced = [4.7])] // I think this difficulty may be a mistake.
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleType1Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType1,

	/// <summary>
	/// Indicates avoidable rectangle type 2.
	/// </summary>
	[Hodoku(Rating = 100, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0608")]
	[SudokuExplainer(
		TechniqueDefined = SudokuExplainerTechnique.AvoidableRectangle,
		RatingValueAdvanced = [4.5])] // I think this difficulty may be a mistake.
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleType2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType2,

	/// <summary>
	/// Indicates avoidable rectangle type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Hidden],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType3,

	/// <summary>
	/// Indicates avoidable rectangle type 5.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleType2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleType5,

	/// <summary>
	/// Indicates hidden avoidable rectangle.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(HiddenUniqueRectangleStep),
		Abbreviation = "HAR",
		SecondaryStepType = typeof(UniqueRectangleWithConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair, ExtraDifficultyFactorNames.Avoidable],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	HiddenAvoidableRectangle,

	/// <summary>
	/// Indicates avoidable rectangle + 2D.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangle2DOr3XStep),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangle2D,

	/// <summary>
	/// Indicates avoidable rectangle + 3X.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangle2DOr3XStep),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangle3X,

	/// <summary>
	/// Indicates avoidable rectangle XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.WingSize],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleXyWing,

	/// <summary>
	/// Indicates avoidable rectangle XYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.WingSize],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleXyzWing,

	/// <summary>
	/// Indicates avoidable rectangle WXYZ-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.WingSize],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleWxyzWing,

	/// <summary>
	/// Indicates avoidable rectangle W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
#if UNIQUE_RECTANGLE_W_WING
		PrimarySupportedType = typeof(UniqueRectangleWWingStep),
#endif
#if !UNIQUE_RECTANGLE_W_WING
		Features = TechniqueFeature.NotImplemented,
#endif
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleWWing,

	/// <summary>
	/// Indicates avoidable rectangle sue de coq.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleWithSueDeCoqStep),
		ExtraFactors = [
			ExtraDifficultyFactorNames.Size,
			ExtraDifficultyFactorNames.Isolated,
			ExtraDifficultyFactorNames.Cannibalism,
			ExtraDifficultyFactorNames.Avoidable
		],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleSueDeCoq,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in block.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(AvoidableRectangleWithHiddenSingleStep),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleHiddenSingleBlock,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in row.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(AvoidableRectangleWithHiddenSingleStep),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleHiddenSingleRow,

	/// <summary>
	/// Indicates avoidable rectangle hidden single in column.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(AvoidableRectangleWithHiddenSingleStep),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleHiddenSingleColumn,

	/// <summary>
	/// Indicates avoidable rectangle external type 1.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType1Or2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType1,

	/// <summary>
	/// Indicates avoidable rectangle external type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType1Or2Step),
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType2,

	/// <summary>
	/// Indicates avoidable rectangle external type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType3,

	/// <summary>
	/// Indicates avoidable rectangle external type 4.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalType4Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalType4,

	/// <summary>
	/// Indicates avoidable rectangle external XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.7,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalXyWingStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalXyWing,

	/// <summary>
	/// Indicates avoidable rectangle external W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.8,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalWWingStep),
		Features = TechniqueFeature.NotImplemented,
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"],
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.Incompleteness])]
	AvoidableRectangleExternalWWing,

	/// <summary>
	/// Indicates avoidable rectangle external almost locked sets XZ rule.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.8,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AvoidableRectangle,
		PrimaryStepType = typeof(UniqueRectangleExternalAlmostLockedSetsXzStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Guardian, ExtraDifficultyFactorNames.Avoidable, ExtraDifficultyFactorNames.Incompleteness],
		Links = ["http://sudopedia.enjoysudoku.com/Avoidable_Rectangle.html"])]
	AvoidableRectangleExternalAlmostLockedSetsXz,
	#endregion

	//
	// Unique Loop
	//
	#region Unique Loop
	/// <summary>
	/// Indicates unique loop type 1.
	/// </summary>
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.UniqueLoop, RatingValueOriginal = [4.6, 5.0])]
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueLoop,
		PrimaryStepType = typeof(UniqueLoopType1Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType1,

	/// <summary>
	/// Indicates unique loop type 2.
	/// </summary>
	[SudokuExplainer(
		TechniqueDefined = SudokuExplainerTechnique.UniqueLoop,
		RatingValueOriginal = [4.6, 5.0],
		RatingValueAdvanced = [4.7, 5.1])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueLoop,
		PrimaryStepType = typeof(UniqueLoopType2Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType2,

	/// <summary>
	/// Indicates unique loop type 3.
	/// </summary>
	[SudokuExplainer(
		TechniqueDefined = SudokuExplainerTechnique.UniqueLoop,
		RatingValueOriginal = [4.6, 5.0],
		RatingValueAdvanced = [4.7, 5.1])]
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueLoop,
		PrimaryStepType = typeof(UniqueLoopType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length, ExtraDifficultyFactorNames.Size],
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
	UniqueLoopType3,

	/// <summary>
	/// Indicates unique loop type 4.
	/// </summary>
	[SudokuExplainer(
		TechniqueDefined = SudokuExplainerTechnique.UniqueLoop,
		RatingValueOriginal = [4.6, 5.0],
		RatingValueAdvanced = [4.7, 5.1])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.UniqueLoop,
		PrimaryStepType = typeof(UniqueLoopType4Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=39748#p39748"])]
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
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ExtendedRectangle,
		PrimaryStepType = typeof(ExtendedRectangleType1Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size])]
	ExtendedRectangleType1,

	/// <summary>
	/// Indicates extended rectangle type 2.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0621")]
#endif
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ExtendedRectangle,
		PrimaryStepType = typeof(ExtendedRectangleType2Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size])]
	ExtendedRectangleType2,

	/// <summary>
	/// Indicates extended rectangle type 3.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0622")]
#endif
	[TechniqueMetadata(
		Rating = 4.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ExtendedRectangle,
		PrimaryStepType = typeof(ExtendedRectangleType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.ExtraDigit])]
	ExtendedRectangleType3,

	/// <summary>
	/// Indicates extended rectangle type 4.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Prefix = "0623")]
#endif
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.ExtendedRectangle,
		PrimaryStepType = typeof(ExtendedRectangleType4Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size])]
	ExtendedRectangleType4,
	#endregion

	//
	// Bivalue Universal Grave
	//
	#region Bivalue Universal Grave
	/// <summary>
	/// Indicates bi-value universal grave type 1.
	/// </summary>
	[Hodoku(
		Rating = 100,
		DifficultyLevel = HodokuDifficultyLevel.Hard,
		Prefix = "0610",
		Aliases = ["Bivalue Universal Grave + 1"])]
	[SudokuExplainer(Rating = 5.6, TechniqueDefined = SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		Rating = 5.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveType1Step),
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType1,

	/// <summary>
	/// Indicates bi-value universal grave type 2.
	/// </summary>
	[SudokuExplainer(Rating = 5.7, TechniqueDefined = SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		Rating = 5.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveType2Step),
		ExtraFactors = [ExtraDifficultyFactorNames.ExtraDigit],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType2,

	/// <summary>
	/// Indicates bi-value universal grave type 3.
	/// </summary>
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.BivalueUniversalGrave, RatingValueOriginal = [5.8, 6.1])]
	[TechniqueMetadata(
		Rating = 5.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size, ExtraDifficultyFactorNames.Hidden],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType3,

	/// <summary>
	/// Indicates bi-value universal grave type 4.
	/// </summary>
	[SudokuExplainer(Rating = 5.7, TechniqueDefined = SudokuExplainerTechnique.BivalueUniversalGrave)]
	[TechniqueMetadata(
		Rating = 5.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveType4Step),
		ExtraFactors = [ExtraDifficultyFactorNames.ConjugatePair],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveType4,

	/// <summary>
	/// Indicates bi-value universal grave + n.
	/// </summary>
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.BivalueUniversalGravePlusN, RatingValueAdvanced = [5.7])]
	[TechniqueMetadata(
		Rating = 5.7, DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveMultipleStep),
		Abbreviation = "BUG + n",
		ExtraFactors = [ExtraDifficultyFactorNames.Size],
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGravePlusN,

	/// <summary>
	/// Indicates bi-value universal grave false candidate type.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.7,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveFalseCandidateTypeStep),
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveFalseCandidateType,

	/// <summary>
	/// Indicates bi-value universal grave + n with forcing chains.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		Features = TechniqueFeature.NotImplemented,
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGravePlusNForcingChains,

	/// <summary>
	/// Indicates bi-value universal grave XZ rule.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.8,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
		PrimaryStepType = typeof(BivalueUniversalGraveXzStep),
		Abbreviation = "BUG-XZ",
		Links = ["http://sudopedia.enjoysudoku.com/Bivalue_Universal_Grave.html", "http://forum.enjoysudoku.com/viewtopic.php?t=2352"])]
	BivalueUniversalGraveXzRule,

	/// <summary>
	/// Indicates bi-value universal grave XY-Wing.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BivalueUniversalGrave,
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
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ReverseBivalueUniversalGrave,
		PrimaryStepType = typeof(ReverseBivalueUniversalGraveType1Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType1,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.1,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ReverseBivalueUniversalGrave,
		PrimaryStepType = typeof(ReverseBivalueUniversalGraveType2Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType2,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ReverseBivalueUniversalGrave,
		PrimaryStepType = typeof(ReverseBivalueUniversalGraveType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/Reverse_BUG.html", "http://forum.enjoysudoku.com/viewtopic.php?t=4431"])]
	ReverseBivalueUniversalGraveType3,

	/// <summary>
	/// Indicates reverse bi-value universal grave type 4.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.3,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.ReverseBivalueUniversalGrave,
		PrimaryStepType = typeof(ReverseBivalueUniversalGraveType4Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
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
		Rating = 6.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniquenessClueCover,
		PrimaryStepType = typeof(UniquenessClueCoverStep),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size],
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
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RwDeadlyPattern,
		Features = TechniqueFeature.NotImplemented,
		Links = ["http://forum.enjoysudoku.com/yet-another-crazy-uniqueness-technique-t5589.html"])]
	RwDeadlyPattern,
	#endregion

	//
	// Borescoper's Deadly Pattern
	//
	#region Borescoper's Deadly Pattern
	/// <summary>
	/// Indicates Borescoper's deadly pattern type 1.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.3,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BorescoperDeadlyPattern,
		PrimaryStepType = typeof(BorescoperDeadlyPatternType1Step))]
	BorescoperDeadlyPatternType1,

	/// <summary>
	/// Indicates Borescoper's deadly pattern type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BorescoperDeadlyPattern,
		PrimaryStepType = typeof(BorescoperDeadlyPatternType2Step))]
	BorescoperDeadlyPatternType2,

	/// <summary>
	/// Indicates Borescoper's deadly pattern type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.3,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BorescoperDeadlyPattern,
		PrimaryStepType = typeof(BorescoperDeadlyPatternType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size])]
	BorescoperDeadlyPatternType3,

	/// <summary>
	/// Indicates Borescoper's deadly pattern type 4.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.BorescoperDeadlyPattern,
		PrimaryStepType = typeof(BorescoperDeadlyPatternType4Step))]
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
		Rating = 5.8,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternType1Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.9,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternType2Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType2,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.8,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternType3Step),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size], Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType3,

	/// <summary>
	/// Indicates Qiu's deadly pattern type 4.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternType4Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternType4,

	/// <summary>
	/// Indicates locked Qiu's deadly pattern.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternLockedTypeStep),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	LockedQiuDeadlyPattern,

	/// <summary>
	/// Indicates Qiu's deadly pattern external type 1.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternExternalType1Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
	QiuDeadlyPatternExternalType1,

	/// <summary>
	/// Indicates Qiu's deadly pattern external type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.1,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.QiuDeadlyPattern,
		PrimaryStepType = typeof(QiuDeadlyPatternExternalType2Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/distinction-theory-t35042.html"])]
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
		Rating = 5.3,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueMatrix,
		PrimaryStepType = typeof(UniqueMatrixType1Step),
		Features = TechniqueFeature.HardToBeGenerated)]
	UniqueMatrixType1,

	/// <summary>
	/// Indicates unique matrix type 2.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.4,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueMatrix,
		PrimaryStepType = typeof(UniqueMatrixType2Step),
		Features = TechniqueFeature.HardToBeGenerated)]
	UniqueMatrixType2,

	/// <summary>
	/// Indicates unique matrix type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.3,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueMatrix,
		PrimaryStepType = typeof(UniqueMatrixType3Step),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Size])]
	UniqueMatrixType3,

	/// <summary>
	/// Indicates unique matrix type 4.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.UniqueMatrix,
		PrimaryStepType = typeof(UniqueMatrixType4Step),
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
	[Hodoku(Rating = 250, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "1101")]
	[SudokuExplainer(RatingValueAdvanced = [5.0])]
	[TechniqueMetadata(
		Rating = 5.0, DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.SueDeCoq,
		PrimaryStepType = typeof(SueDeCoqStep),
		Abbreviation = "SdC",
		ExtraFactors = [ExtraDifficultyFactorNames.Isolated, ExtraDifficultyFactorNames.Cannibalism],
		Links = [
			"http://sudopedia.enjoysudoku.com/Sue_de_Coq.html",
			"http://forum.enjoysudoku.com/two-sector-disjoint-subsets-t2033.html",
			"http://forum.enjoysudoku.com/benchmark-sudoku-list-t3834-15.html#p43170"
		])]
	SueDeCoq,

	/// <summary>
	/// Indicates sue de coq with isolated digit.
	/// </summary>
	[Hodoku(Rating = 250, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "1101")]
	[SudokuExplainer(RatingValueAdvanced = [5.0])]
	[TechniqueMetadata(
		Rating = 5.0, DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.SueDeCoq,
		PrimaryStepType = typeof(SueDeCoqStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Isolated, ExtraDifficultyFactorNames.Cannibalism],
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
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.SueDeCoq,
		PrimaryStepType = typeof(SueDeCoq3DimensionStep),
		Links = ["http://sudopedia.enjoysudoku.com/Sue_de_Coq.html"])]
	SueDeCoq3Dimension,

	/// <summary>
	/// Indicates sue de coq cannibalism.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.SueDeCoq,
		PrimaryStepType = typeof(SueDeCoqStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Isolated, ExtraDifficultyFactorNames.Cannibalism],
		Links = ["http://sudopedia.enjoysudoku.com/Sue_de_Coq.html"])]
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
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.Firework,
		PrimaryStepType = typeof(FireworkTripleStep),
		Links = ["http://forum.enjoysudoku.com/fireworks-t39513.html"])]
	FireworkTriple,

	/// <summary>
	/// Indicates firework quadruple.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.3,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.Firework,
		PrimaryStepType = typeof(FireworkQuadrupleStep),
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
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.BrokenWing,
		PrimaryStepType = typeof(GuardianStep),
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
		Rating = 6.1,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.BivalueOddagon,
		PrimaryStepType = typeof(BivalueOddagonType2Step),
		Links = ["http://forum.enjoysudoku.com/technique-share-odd-bivalue-loop-bivalue-oddagon-t33153.html"])]
	BivalueOddagonType2,

	/// <summary>
	/// Indicates bi-value oddagon type 3.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.BivalueOddagon,
		PrimaryStepType = typeof(BivalueOddagonType3Step),
		ExtraFactors = [ExtraDifficultyFactorNames.Size],
		Links = ["http://forum.enjoysudoku.com/technique-share-odd-bivalue-loop-bivalue-oddagon-t33153.html"])]
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
		Rating = 6.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RankTheory,
		PrimaryStepType = typeof(ChromaticPatternType1Step),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType1,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 2.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RankTheory,
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType2,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 3.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RankTheory,
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType3,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) type 4.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RankTheory,
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/chromatic-patterns-t39885.html", "http://forum.enjoysudoku.com/the-tridagon-rule-t39859.html"])]
	ChromaticPatternType4,

	/// <summary>
	/// Indicates chromatic pattern (tri-value oddagon) XZ rule.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.7,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.RankTheory,
		PrimaryStepType = typeof(ChromaticPatternXzStep),
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
	[Hodoku(Rating = 130, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0400")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(Rating = 6.6, TechniqueDefined = SudokuExplainerTechnique.TurbotFish, Aliases = ["Turbot Fish"])]
#endif
	[TechniqueMetadata(
		Rating = 4.0,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.SingleDigitPattern,
		PrimaryStepType = typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/Skyscraper.html"])]
	Skyscraper,

	/// <summary>
	/// Indicates two-string kite.
	/// </summary>
	[Hodoku(Rating = 150, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0401")]
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[SudokuExplainer(Rating = 6.6, TechniqueDefined = SudokuExplainerTechnique.TurbotFish, Aliases = ["Turbot Fish"])]
#endif
	[TechniqueMetadata(
		Rating = 4.1,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.SingleDigitPattern,
		PrimaryStepType = typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/2-String_Kite.html"])]
	TwoStringKite,

	/// <summary>
	/// Indicates turbot fish.
	/// </summary>
	[Hodoku(Rating = 120, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0403")]
	[SudokuExplainer(Rating = 6.6, TechniqueDefined = SudokuExplainerTechnique.TurbotFish)]
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.SingleDigitPattern,
		PrimaryStepType = typeof(TwoStrongLinksStep),
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?t=833"])]
	TurbotFish,

	/// <summary>
	/// Indicates grouped skyscraper.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.2,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.SingleDigitPattern,
		PrimaryStepType = typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/Skyscraper.html"])]
	GroupedSkyscraper,

	/// <summary>
	/// Indicates grouped two-string kite.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.3,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.SingleDigitPattern,
		PrimaryStepType = typeof(TwoStrongLinksStep),
		Links = ["http://sudopedia.enjoysudoku.com/2-String_Kite.html"])]
	GroupedTwoStringKite,

	/// <summary>
	/// Indicates grouped turbot fish.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.4,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.SingleDigitPattern,
		PrimaryStepType = typeof(TwoStrongLinksStep),
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
	[Hodoku(Rating = 120, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0402")]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Hard,
		ContainingGroup = TechniqueGroup.EmptyRectangle,
		PrimaryStepType = typeof(EmptyRectangleStep),
		Abbreviation = "ER",
		Links = ["http://sudopedia.enjoysudoku.com/Empty_Rectangle.html"])]
	EmptyRectangle,
	#endregion

	//
	// Alternating Inference Chain
	//
	#region Chaining
	/// <summary>
	/// Indicates X-Chain.
	/// </summary>
	[Hodoku(Rating = 260, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0701")]
	[SudokuExplainer(RatingValueOriginal = [6.6, 6.9])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/X-Chain.html"])]
	XChain,

	/// <summary>
	/// Indicates Y-Chain.
	/// </summary>
#if COMPATIBLE_ORIGINAL_TECHNIQUE_RULES
	[Hodoku(Rating = 260, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0702")]
#endif
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		ExtraFactors = [ExtraDifficultyFactorNames.Length])]
	YChain,

	/// <summary>
	/// Indicates fishy cycle (X-Cycle).
	/// </summary>
	[Hodoku(Prefix = "0704")]
	[SudokuExplainer(RatingValueOriginal = [6.5, 6.6], Aliases = ["Bidirectional X-Cycle"])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/Fishy_Cycle.html"])]
	FishyCycle,

	/// <summary>
	/// Indicates XY-Chain.
	/// </summary>
	[Hodoku(Rating = 260, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0702")]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented,
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/XY-Chain.html"])]
	XyChain,

	/// <summary>
	/// Indicates XY-Cycle.
	/// </summary>
	[SudokuExplainer(RatingValueOriginal = [6.6, 7.0])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Features = TechniqueFeature.NotImplemented)]
	XyCycle,

	/// <summary>
	/// Indicates XY-X-Chain.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented,
		ExtraFactors = [ExtraDifficultyFactorNames.Length])]
	XyXChain,

	/// <summary>
	/// Indicates remote pair.
	/// </summary>
	[Hodoku(Rating = 110, DifficultyLevel = HodokuDifficultyLevel.Hard, Prefix = "0703")]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented,
		ExtraFactors = [ExtraDifficultyFactorNames.Length])]
	RemotePair,

	/// <summary>
	/// Indicates purple cow.
	/// </summary>
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Features = TechniqueFeature.NotImplemented,
		ExtraFactors = [ExtraDifficultyFactorNames.Length])]
	PurpleCow,

	/// <summary>
	/// Indicates discontinuous nice loop.
	/// </summary>
	[Hodoku(Rating = 280, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0707")]
	[SudokuExplainer(RatingValueOriginal = [7.0, 7.6], Aliases = ["Forcing Chain"])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Abbreviation = "DNL",
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Features = TechniqueFeature.NotImplemented,
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?t=2859"])]
	DiscontinuousNiceLoop,

	/// <summary>
	/// Indicates continuous nice loop.
	/// </summary>
	[Hodoku(Rating = 280, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0706")]
	[SudokuExplainer(RatingValueOriginal = [7.0, 7.3], Aliases = ["Bidirectional Cycle"])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Abbreviation = "CNL",
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/Nice_Loop.html"])]
	ContinuousNiceLoop,

	/// <summary>
	/// Indicates alternating inference chain.
	/// </summary>
	[Hodoku(Rating = 280, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0708")]
	[SudokuExplainer(RatingValueOriginal = [7.0, 7.6])]
	[TechniqueMetadata(
		Rating = 4.6,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		PrimaryStepType = typeof(ForcingChainStep),
		Abbreviation = "AIC",
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Links = ["http://sudopedia.enjoysudoku.com/Alternating_Inference_Chain.html", "http://forum.enjoysudoku.com/viewtopic.php?t=3865"])]
	AlternatingInferenceChain,

	/// <summary>
	/// Indicates grouped X-Chain.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXChain,

	/// <summary>
	/// Indicates grouped fishy cycle (grouped X-Cycle).
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedFishyCycle,

	/// <summary>
	/// Indicates grouped XY-Chain.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXyChain,

	/// <summary>
	/// Indicates grouped XY-Cycle.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXyCycle,

	/// <summary>
	/// Indicates grouped XY-X-Chain.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedXyXChain,

	/// <summary>
	/// Indicates grouped purple cow.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedPurpleCow,

	/// <summary>
	/// Indicates grouped discontinuous nice loop.
	/// </summary>
	[Hodoku(Rating = 300, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0710")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedDiscontinuousNiceLoop,

	/// <summary>
	/// Indicates grouped continuous nice loop.
	/// </summary>
	[Hodoku(Rating = 300, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0709")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedContinuousNiceLoop,

	/// <summary>
	/// Indicates grouped alternating inference chain.
	/// </summary>
	[Hodoku(Rating = 300, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0711")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
		Features = TechniqueFeature.NotImplemented)]
	GroupedAlternatingInferenceChain,

	/// <summary>
	/// Indicates special case that a grouped alternating inference chain has a collision between start and end node.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlternatingInferenceChain,
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
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.NishioForcingChain, RatingValueOriginal = [7.6, 8.1])]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		Links = ["http://sudopedia.enjoysudoku.com/Nishio.html"])]
	NishioForcingChains,

	/// <summary>
	/// Indicates region forcing chains (i.e. house forcing chains).
	/// </summary>
	[Hodoku(Rating = 500, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1301")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.MultipleForcingChain, RatingValueOriginal = [8.2, 8.6])]
	[TechniqueMetadata(
		Rating = 8.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		PrimaryStepType = typeof(RegionForcingChainsStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length])]
	RegionForcingChains,

	/// <summary>
	/// Indicates cell forcing chains.
	/// </summary>
	[Hodoku(Rating = 500, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1301")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.MultipleForcingChain, RatingValueOriginal = [8.2, 8.6])]
	[TechniqueMetadata(
		Rating = 8.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		PrimaryStepType = typeof(CellForcingChainsStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length])]
	CellForcingChains,

	/// <summary>
	/// Indicates dynamic region forcing chains (i.e. dynamic house forcing chains).
	/// </summary>
	[Hodoku(Rating = 500, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1303")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.6, 9.4])]
	[TechniqueMetadata(
		Rating = 8.5,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		PrimaryStepType = typeof(RegionForcingChainsStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Features = TechniqueFeature.HardToBeGenerated)]
	DynamicRegionForcingChains,

	/// <summary>
	/// Indicates dynamic cell forcing chains.
	/// </summary>
	[Hodoku(Rating = 500, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1303")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.6, 9.4])]
	[TechniqueMetadata(
		Rating = 8.5,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		PrimaryStepType = typeof(CellForcingChainsStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Features = TechniqueFeature.HardToBeGenerated)]
	DynamicCellForcingChains,

	/// <summary>
	/// Indicates dynamic contradiction forcing chains.
	/// </summary>
	[Hodoku(Rating = 500, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1304")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.8, 9.4])]
	[TechniqueMetadata(
		Rating = 9.5,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		PrimaryStepType = typeof(BinaryForcingChainsStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Features = TechniqueFeature.HardToBeGenerated)]
	DynamicContradictionForcingChains,

	/// <summary>
	/// Indicates dynamic double forcing chains.
	/// </summary>
	[Hodoku(Rating = 500, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1304")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.DynamicForcingChain, RatingValueOriginal = [8.8, 9.4])]
	[TechniqueMetadata(
		Rating = 9.5,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.ForcingChains,
		PrimaryStepType = typeof(BinaryForcingChainsStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
		Features = TechniqueFeature.HardToBeGenerated)]
	DynamicDoubleForcingChains,

	/// <summary>
	/// Indicates dynamic forcing chains.
	/// </summary>
	[TechniqueMetadata(DifficultyLevel = DifficultyLevel.Nightmare, ContainingGroup = TechniqueGroup.ForcingChains)]
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
		Rating = 8.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.BlossomLoop,
		PrimaryStepType = typeof(BlossomLoopStep),
		Features = TechniqueFeature.HardToBeGenerated,
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
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
	[SudokuExplainer(Rating = 6.2, TechniqueDefined = SudokuExplainerTechnique.AlignedPairExclusion)]
	[TechniqueMetadata(
		Rating = 6.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlignedExclusion,
		PrimaryStepType = typeof(AlignedExclusionStep),
		Abbreviation = "APE",
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = [
			"http://sudopedia.enjoysudoku.com/Subset_Exclusion.html",
			"http://sudopedia.enjoysudoku.com/Aligned_Pair_Exclusion.html",
			"http://forum.enjoysudoku.com/viewtopic.php?t=3882"
		])]
	AlignedPairExclusion,

	/// <summary>
	/// Indicates aligned triple exclusion.
	/// </summary>
	[SudokuExplainer(Rating = 7.5, TechniqueDefined = SudokuExplainerTechnique.AlignedTripletExclusion)]
	[TechniqueMetadata(
		Rating = 7.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlignedExclusion,
		PrimaryStepType = typeof(AlignedExclusionStep),
		Abbreviation = "ATE",
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Subset_Exclusion.html", "http://sudopedia.enjoysudoku.com/Aligned_Pair_Exclusion.html",])]
	AlignedTripleExclusion,

	/// <summary>
	/// Indicates aligned quadruple exclusion.
	/// </summary>
	[TechniqueMetadata(
		Rating = 8.1,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlignedExclusion,
		PrimaryStepType = typeof(AlignedExclusionStep),
		Abbreviation = "AQE",
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Subset_Exclusion.html"])]
	AlignedQuadrupleExclusion,

	/// <summary>
	/// Indicates aligned quintuple exclusion.
	/// </summary>
	[TechniqueMetadata(
		Rating = 8.4,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlignedExclusion,
		PrimaryStepType = typeof(AlignedExclusionStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Subset_Exclusion.html"])]
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
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	XyzLoop,

	/// <summary>
	/// Indicates Siamese XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseXyzLoop,

	/// <summary>
	/// Indicates XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	XyzNiceLoop,

	/// <summary>
	/// Indicates Siamese XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseXyzNiceLoop,

	/// <summary>
	/// Indicates grouped XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	GroupedXyzLoop,

	/// <summary>
	/// Indicates Siamese grouped XYZ loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	SiameseGroupedXyzLoop,

	/// <summary>
	/// Indicates grouped XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
		Links = ["http://forum.enjoysudoku.com/xyz-ring-t42209.html"])]
	GroupedXyzNiceLoop,

	/// <summary>
	/// Indicates Siamese grouped XYZ nice loop.
	/// </summary>
	[TechniqueMetadata(
		Rating = 5.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.XyzRing,
		PrimaryStepType = typeof(XyzRingStep),
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
	[Hodoku(Rating = 300, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0901")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.AlsXz, RatingValueAdvanced = [7.5])]
	[TechniqueMetadata(
		Rating = 5.5,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlmostLockedSetsChainingLike,
		PrimaryStepType = typeof(AlmostLockedSetsXzStep),
		Abbreviation = "ALS-XZ",
		Links = ["http://sudopedia.enjoysudoku.com/ALS-XZ.html"])]
	SinglyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates doubly linked ALS-XZ.
	/// </summary>
	[Hodoku(Rating = 300, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0901")]
	[SudokuExplainer(RatingValueAdvanced = [7.5])]
	[TechniqueMetadata(
		Rating = 5.7,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlmostLockedSetsChainingLike,
		PrimaryStepType = typeof(AlmostLockedSetsXzStep),
		Abbreviation = "ALS-XZ",
		Links = ["http://sudopedia.enjoysudoku.com/ALS-XZ.html"])]
	DoublyLinkedAlmostLockedSetsXzRule,

	/// <summary>
	/// Indicates ALS-XY-Wing.
	/// </summary>
	[Hodoku(Rating = 320, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0902")]
	[SudokuExplainer(TechniqueDefined = SudokuExplainerTechnique.AlsXyWing, RatingValueAdvanced = [8.0])]
	[TechniqueMetadata(
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlmostLockedSetsChainingLike,
		PrimaryStepType = typeof(AlmostLockedSetsXyWingStep),
		Abbreviation = "ALS-XY-Wing",
		Links = ["http://sudopedia.enjoysudoku.com/ALS-XY-Wing.html"])]
	AlmostLockedSetsXyWing,

	/// <summary>
	/// Indicates ALS-W-Wing.
	/// </summary>
	[TechniqueMetadata(
		Rating = 6.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlmostLockedSetsChainingLike,
		PrimaryStepType = typeof(AlmostLockedSetsWWingStep),
		Abbreviation = "ALS-W-Wing")]
	AlmostLockedSetsWWing,

	/// <summary>
	/// Indicates ALS chain.
	/// </summary>
	[Hodoku(Rating = 340, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0903")]
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.AlmostLockedSetsChainingLike,
		Features = TechniqueFeature.NotImplemented,
		Links = ["http://sudopedia.enjoysudoku.com/ALS-XY-Chain.html"])]
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
		Rating = 6.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.EmptyRectangleIntersectionPair,
		PrimaryStepType = typeof(EmptyRectangleIntersectionPairStep),
		Abbreviation = "ERIP",
		Links = ["http://forum.enjoysudoku.com/post288015.html"])]
	EmptyRectangleIntersectionPair,
	#endregion

	//
	// Death Blossom
	//
	#region Death Blossom
	/// <summary>
	/// Indicates death blossom.
	/// </summary>
	[Hodoku(Rating = 360, DifficultyLevel = HodokuDifficultyLevel.Unfair, Prefix = "0904")]
	[TechniqueMetadata(
		Rating = 8.2,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.DeathBlossom,
		PrimaryStepType = typeof(DeathBlossomStep),
		Abbreviation = "DB",
		ExtraFactors = [ExtraDifficultyFactorNames.Petals],
		Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	DeathBlossom,

	/// <summary>
	/// Indicates death blossom (house blooming).
	/// </summary>
	[TechniqueMetadata(
		Rating = 8.3,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.DeathBlossom,
		PrimaryStepType = typeof(HouseDeathBlossomStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Petals],
		Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	HouseDeathBlossom,

	/// <summary>
	/// Indicates death blossom (rectangle blooming).
	/// </summary>
	[TechniqueMetadata(
		Rating = 8.5,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.DeathBlossom,
		PrimaryStepType = typeof(RectangleDeathBlossomStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Petals],
		Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
	RectangleDeathBlossom,

	/// <summary>
	/// Indicates death blossom (A^nLS blooming).
	/// </summary>
	[TechniqueMetadata(
		Rating = 8.7,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.DeathBlossom,
		PrimaryStepType = typeof(NTimesAlmostLockedSetDeathBlossomStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Petals],
		Links = ["http://sudopedia.enjoysudoku.com/Death_Blossom.html"])]
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
		Rating = 7.0,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.Symmetry,
		PrimaryStepType = typeof(GurthSymmetricalPlacementStep),
		Abbreviation = "GSP",
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/viewtopic.php?p=32842#p32842"])]
	GurthSymmetricalPlacement,

	/// <summary>
	/// Indicates extended Gurth's symmetrical placement.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Symmetry,
		Features = TechniqueFeature.NotImplemented | TechniqueFeature.HardToBeGenerated)]
	ExtendedGurthSymmetricalPlacement,

	/// <summary>
	/// Indicates Anti-GSP (Anti- Gurth's Symmetrical Placement).
	/// </summary>
	[TechniqueMetadata(
		Rating = 7.3,
		DifficultyLevel = DifficultyLevel.Fiendish,
		ContainingGroup = TechniqueGroup.Symmetry,
		PrimaryStepType = typeof(AntiGurthSymmetricalPlacementStep),
		SecondaryStepType = typeof(GurthSymmetricalPlacementStep),
		Features = TechniqueFeature.HardToBeGenerated,
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
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetBaseStep),
		Abbreviation = "JE",
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocet,

	/// <summary>
	/// Indicates junior exocet with target conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetConjugatePair,

	/// <summary>
	/// Indicates junior exocet mirror mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetMirrorConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates junior exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(JuniorExocetAdjacentTargetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates junior exocet incompatible pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(JuniorExocetIncompatiblePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.IncompatiblePair],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetIncompatiblePair,

	/// <summary>
	/// Indicates junior exocet target pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(JuniorExocetTargetPairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.TargetPair],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetTargetPair,

	/// <summary>
	/// Indicates junior exocet generalized fish.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(JuniorExocetGeneralizedFishStep),
		ExtraFactors = [ExtraDifficultyFactorNames.GeneralizedFish],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetGeneralizedFish,

	/// <summary>
	/// Indicates junior exocet mirror almost hidden set.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(JuniorExocetMirrorAlmostHiddenSetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.AlmostHiddenSet],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetMirrorAlmostHiddenSet,

	/// <summary>
	/// Indicates junior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetLockedMemberStep),
		ExtraFactors = [ExtraDifficultyFactorNames.LockedMember],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetLockedMember,

	/// <summary>
	/// Indicates junior exocet mirror sync.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(JuniorExocetMirrorSyncStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	JuniorExocetMirrorSync,

	/// <summary>
	/// Indicates senior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.6,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetBaseStep),
		Abbreviation = "SE",
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocet,

	/// <summary>
	/// Indicates senior exocet mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetMirrorConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates senior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ExocetLockedMemberStep),
		ExtraFactors = [ExtraDifficultyFactorNames.LockedMember],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocetLockedMember,

	/// <summary>
	/// Indicates senior exocet true base.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(SeniorExocetTrueBaseStep),
		ExtraFactors = [ExtraDifficultyFactorNames.TrueBase],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	SeniorExocetTrueBase,

	/// <summary>
	/// Indicates weak exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.7,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(WeakExocetStep),
		Abbreviation = "WE",
		ExtraFactors = [ExtraDifficultyFactorNames.MissingStabilityBalancer],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocet,

	/// <summary>
	/// Indicates weak exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.7,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(WeakExocetAdjacentTargetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocetAdjacentTarget,

	/// <summary>
	/// Indicates weak exocet slash.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.7,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(WeakExocetSlashStep),
		ExtraFactors = [ExtraDifficultyFactorNames.SlashElimination],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocetSlash,

	/// <summary>
	/// Indicates weak exocet BZ rectangle.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.7,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(WeakExocetBzRectangleStep),
		ExtraFactors = [ExtraDifficultyFactorNames.BzRectangle],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	WeakExocetBzRectangle,

	/// <summary>
	/// Indicates lame weak exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.7,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(WeakExocetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.MissingStabilityBalancer],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html", "http://forum.enjoysudoku.com/weak-exocet-t39651.html"])]
	LameWeakExocet,

	/// <summary>
	/// Indicates franken junior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.8,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocet,

	/// <summary>
	/// Indicates franken junior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.8,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [ExtraDifficultyFactorNames.LockedMember],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocetLockedMember,

	/// <summary>
	/// Indicates franken junior exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.8,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexJuniorExocetAdjacentTargetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates franken junior exocet mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.8,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexJuniorExocetMirrorConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenJuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates mutant junior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocet,

	/// <summary>
	/// Indicates mutant junior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [ExtraDifficultyFactorNames.LockedMember],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocetLockedMember,

	/// <summary>
	/// Indicates mutant junior exocet adjacent target.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexJuniorExocetAdjacentTargetStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocetAdjacentTarget,

	/// <summary>
	/// Indicates mutant junior exocet mirror conjugate pair.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexJuniorExocetMirrorConjugatePairStep),
		ExtraFactors = [ExtraDifficultyFactorNames.Mirror],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantJuniorExocetMirrorConjugatePair,

	/// <summary>
	/// Indicates franken senior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenSeniorExocet,

	/// <summary>
	/// Indicates franken senior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.0,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [ExtraDifficultyFactorNames.LockedMember],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	FrankenSeniorExocetLockedMember,

	/// <summary>
	/// Indicates advanced franken senior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.8,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(AdvancedComplexSeniorExocetStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	AdvancedFrankenSeniorExocet,

	/// <summary>
	/// Indicates mutant senior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantSeniorExocet,

	/// <summary>
	/// Indicates mutant senior exocet locked member.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.2,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(ComplexExocetLockedMemberStep),
		ExtraFactors = [ExtraDifficultyFactorNames.LockedMember],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	MutantSeniorExocetLockedMember,

	/// <summary>
	/// Indicates advanced mutant senior exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 10.1,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(AdvancedComplexSeniorExocetStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	AdvancedMutantSeniorExocet,

	/// <summary>
	/// Indicates double exocet.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(DoubleExocetBaseStep),
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	DoubleExocet,

	/// <summary>
	/// Indicates double exocet uni-fish pattern.
	/// </summary>
	[TechniqueMetadata(
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		PrimaryStepType = typeof(DoubleExocetGeneralizedFishStep),
		ExtraFactors = [ExtraDifficultyFactorNames.GeneralizedFish],
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
	DoubleExocetGeneralizedFish,

	/// <summary>
	/// Indicates pattern-locked quadruple. This quadruple is a special quadruple: it can only be concluded after both JE and SK-Loop are formed.
	/// </summary>
	[TechniqueMetadata(
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.Exocet,
		Abbreviation = "PLQ",
		Links = ["http://forum.enjoysudoku.com/jexocet-compendium-t32370.html"])]
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
		Rating = 9.6,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.DominoLoop,
		PrimaryStepType = typeof(DominoLoopStep),
		Features = TechniqueFeature.HardToBeGenerated,
		Links = ["http://forum.enjoysudoku.com/domino-loops-sk-loops-beyond-t32789.html"])]
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
		Rating = 9.4,
		DifficultyLevel = DifficultyLevel.Nightmare,
		ContainingGroup = TechniqueGroup.MultisectorLockedSets,
		PrimaryStepType = typeof(MultisectorLockedSetsStep),
		Abbreviation = "MSLS",
		Features = TechniqueFeature.HardToBeGenerated,
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
		Rating = 8.5,
		DifficultyLevel = DifficultyLevel.LastResort,
		ContainingGroup = TechniqueGroup.PatternOverlay,
		PrimaryStepType = typeof(PatternOverlayStep),
		Abbreviation = "POM",
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
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
	[Hodoku(Rating = 10000, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1201")]
	[TechniqueMetadata(
		Rating = 9.0,
		DifficultyLevel = DifficultyLevel.LastResort,
		ContainingGroup = TechniqueGroup.Templating,
		PrimaryStepType = typeof(TemplateStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Templating.html"])]
	TemplateSet,

	/// <summary>
	/// Indicates template delete.
	/// </summary>
	[Hodoku(Rating = 10000, DifficultyLevel = HodokuDifficultyLevel.Extreme, Prefix = "1202")]
	[TechniqueMetadata(
		Rating = 9.0,
		DifficultyLevel = DifficultyLevel.LastResort,
		ContainingGroup = TechniqueGroup.Templating,
		PrimaryStepType = typeof(TemplateStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		Links = ["http://sudopedia.enjoysudoku.com/Templating.html"])]
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
		Rating = 8.0,
		DifficultyLevel = DifficultyLevel.LastResort,
		ContainingGroup = TechniqueGroup.BowmanBingo,
		PrimaryStepType = typeof(BowmanBingoStep),
		Features = TechniqueFeature.WillBeReplacedByOtherTechnique,
		ExtraFactors = [ExtraDifficultyFactorNames.Length],
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
	[Hodoku(Rating = 10000, DifficultyLevel = HodokuDifficultyLevel.Extreme)]
	[SudokuExplainer(
		Rating = (double)AnalyzerResult.MaximumRatingValueTheory,
		TechniqueDefined = SudokuExplainerTechnique.BruteForce,
		Aliases = ["Try & Error"])]
	[TechniqueMetadata(
		Rating = (double)AnalyzerResult.MaximumRatingValueTheory,
		DifficultyLevel = DifficultyLevel.LastResort,
		ContainingGroup = TechniqueGroup.BruteForce,
		PrimaryStepType = typeof(BruteForceStep),
		Abbreviation = "BF",
		Features = TechniqueFeature.OnlyExistInTheory,
		Links = ["http://sudopedia.enjoysudoku.com/Trial_%26_Error.html"])]
	BruteForce,
	#endregion
}
