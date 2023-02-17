namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="LogicalSolver"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="LogicalSolver"/>
[AttachedProperty<bool>("EnableFullHouse", DefaultValue = true)]
[AttachedProperty<bool>("EnableLastDigit", DefaultValue = true)]
[AttachedProperty<bool>("HiddenSinglesInBlockFirst", DefaultValue = true)]
[AttachedProperty<bool>("AllowIncompleteUniqueRectangles", DefaultValue = true)]
[AttachedProperty<bool>("SearchForExtendedUniqueRectangles", DefaultValue = true)]
[AttachedProperty<bool>("SearchExtendedBivalueUniversalGraveTypes", DefaultValue = true)]
[AttachedProperty<bool>("AllowCollisionOnAlsXz", DefaultValue = true)]
[AttachedProperty<bool>("AllowLoopedPatternsOnAlsXz", DefaultValue = true)]
[AttachedProperty<bool>("AllowCollisionOnAlsXyWing", DefaultValue = true)]
[AttachedProperty<int>("MaxSizeOfRegularWing", DefaultValue = 5)]
[AttachedProperty<int>("MaxSizeOfComplexFish", DefaultValue = 5)]
[AttachedProperty<bool>("TemplateDeleteOnly")]
[AttachedProperty<int>("BowmanBingoMaxLength", DefaultValue = 64)]
[AttachedProperty<bool>("CheckAlmostLockedQuadruple")]
[AttachedProperty<bool>("CheckAdvancedJuniorExocet", DefaultValue = true)]
[AttachedProperty<bool>("CheckAdvancedSeniorExocet", DefaultValue = true)]
[AttachedProperty<bool>("SolverIsFullApplying")]
[AttachedProperty<bool>("SolverIgnoreSlowAlgorithms")]
[AttachedProperty<bool>("SolverIgnoreHighAllocationAlgorithms")]
public static partial class LogicalSolverProperties
{
	[Callback]
	private static void EnableFullHousePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SingleStepSearcher_EnableFullHouse = (bool)e.NewValue;

	[Callback]
	private static void EnableLastDigitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SingleStepSearcher_EnableLastDigit = (bool)e.NewValue;

	[Callback]
	private static void HiddenSinglesInBlockFirstPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SingleStepSearcher_HiddenSinglesInBlockFirst = (bool)e.NewValue;

	[Callback]
	private static void AllowIncompleteUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = (bool)e.NewValue;

	[Callback]
	private static void SearchForExtendedUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = (bool)e.NewValue;

	[Callback]
	private static void SearchExtendedBivalueUniversalGraveTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).BivalueUniversalGraveStepSearcher_SearchExtendedTypes = (bool)e.NewValue;

	[Callback]
	private static void AllowCollisionOnAlsXzPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedSetsXzStepSearcher_AllowCollision = (bool)e.NewValue;

	[Callback]
	private static void AllowLoopedPatternsOnAlsXzPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = (bool)e.NewValue;

	[Callback]
	private static void AllowCollisionOnAlsXyWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedSetsXyWingStepSearcher_AllowCollision = (bool)e.NewValue;

	[Callback]
	private static void MaxSizeOfRegularWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).RegularWingStepSearcher_MaxSize = (int)e.NewValue;

	[Callback]
	private static void MaxSizeOfComplexFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).ComplexFishStepSearcher_MaxSize = (int)e.NewValue;

	[Callback]
	private static void TemplateDeleteOnlyPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).TemplateStepSearcher_TemplateDeleteOnly = (bool)e.NewValue;

	[Callback]
	private static void BowmanBingoMaxLengthPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).BowmanBingoStepSearcher_MaxLength = (int)e.NewValue;

	[Callback]
	private static void CheckAlmostLockedQuadruplePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = (bool)e.NewValue;

	[Callback]
	private static void CheckAdvancedJuniorExocetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).JuniorExocetStepSearcher_CheckAdvanced = (bool)e.NewValue;

	[Callback]
	private static void CheckAdvancedSeniorExocetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SeniorExocetStepSearcher_CheckAdvanced = (bool)e.NewValue;

	[Callback]
	private static void SolverIsFullApplyingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).IsFullApplying = (bool)e.NewValue;

	[Callback]
	private static void SolverIgnoreSlowAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).IgnoreSlowAlgorithms = (bool)e.NewValue;

	[Callback]
	private static void SolverIgnoreHighAllocationAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).IgnoreHighAllocationAlgorithms = (bool)e.NewValue;
}
