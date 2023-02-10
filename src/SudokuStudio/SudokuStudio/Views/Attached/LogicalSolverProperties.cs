namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="LogicalSolver"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="LogicalSolver"/>
[AttachedProperty<bool>("EnableFullHouse", DefaultValue = true, CallbackMethodName = nameof(EnableFullHousePropertyCallback))]
[AttachedProperty<bool>("EnableLastDigit", DefaultValue = true, CallbackMethodName = nameof(EnableLastDigitPropertyCallback))]
[AttachedProperty<bool>("HiddenSinglesInBlockFirst", DefaultValue = true, CallbackMethodName = nameof(HiddenSinglesInBlockFirstPropertyCallback))]
[AttachedProperty<bool>("AllowIncompleteUniqueRectangles", DefaultValue = true, CallbackMethodName = nameof(AllowIncompleteUniqueRectanglesPropertyCallback))]
[AttachedProperty<bool>("SearchForExtendedUniqueRectangles", DefaultValue = true, CallbackMethodName = nameof(SearchForExtendedUniqueRectanglesPropertyCallback))]
[AttachedProperty<bool>("SearchExtendedBivalueUniversalGraveTypes", DefaultValue = true, CallbackMethodName = nameof(SearchExtendedBivalueUniversalGraveTypesPropertyCallback))]
[AttachedProperty<bool>("AllowCollisionOnAlsXz", DefaultValue = true, CallbackMethodName = nameof(AllowCollisionOnAlsXzPropertyCallback))]
[AttachedProperty<bool>("AllowLoopedPatternsOnAlsXz", DefaultValue = true, CallbackMethodName = nameof(AllowLoopedPatternsOnAlsXzPropertyCallback))]
[AttachedProperty<bool>("AllowCollisionOnAlsXyWing", DefaultValue = true, CallbackMethodName = nameof(AllowCollisionOnAlsXyWingPropertyCallback))]
[AttachedProperty<int>("MaxSizeOfRegularWing", DefaultValue = 5, CallbackMethodName = nameof(MaxSizeOfRegularWingPropertyCallback))]
[AttachedProperty<int>("MaxSizeOfComplexFish", DefaultValue = 5, CallbackMethodName = nameof(MaxSizeOfComplexFishPropertyCallback))]
[AttachedProperty<bool>("TemplateDeleteOnly", CallbackMethodName = nameof(TemplateDeleteOnlyPropertyCallback))]
[AttachedProperty<int>("BowmanBingoMaxLength", DefaultValue = 64, CallbackMethodName = nameof(BowmanBingoMaxLengthPropertyCallback))]
[AttachedProperty<bool>("CheckAlmostLockedQuadruple", CallbackMethodName = nameof(CheckAlmostLockedQuadruplePropertyCallback))]
[AttachedProperty<bool>("CheckAdvancedJuniorExocet", DefaultValue = true, CallbackMethodName = nameof(CheckAdvancedJuniorExocetPropertyCallback))]
[AttachedProperty<bool>("CheckAdvancedSeniorExocet", DefaultValue = true, CallbackMethodName = nameof(CheckAdvancedSeniorExocetPropertyCallback))]
[AttachedProperty<bool>("SolverIsFullApplying", CallbackMethodName = nameof(SolverIsFullApplyingPropertyCallback))]
[AttachedProperty<bool>("SolverIgnoreSlowAlgorithms", CallbackMethodName = nameof(SolverIgnoreSlowAlgorithmsPropertyCallback))]
[AttachedProperty<bool>("SolverIgnoreHighAllocationAlgorithms", CallbackMethodName = nameof(SolverIgnoreHighAllocationAlgorithmsPropertyCallback))]
public static partial class LogicalSolverProperties
{
	private static void EnableFullHousePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SingleStepSearcher_EnableFullHouse = (bool)e.NewValue;

	private static void EnableLastDigitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SingleStepSearcher_EnableLastDigit = (bool)e.NewValue;

	private static void HiddenSinglesInBlockFirstPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SingleStepSearcher_HiddenSinglesInBlockFirst = (bool)e.NewValue;

	private static void AllowIncompleteUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = (bool)e.NewValue;

	private static void SearchForExtendedUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = (bool)e.NewValue;

	private static void SearchExtendedBivalueUniversalGraveTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).BivalueUniversalGraveStepSearcher_SearchExtendedTypes = (bool)e.NewValue;

	private static void AllowCollisionOnAlsXzPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedSetsXzStepSearcher_AllowCollision = (bool)e.NewValue;

	private static void AllowLoopedPatternsOnAlsXzPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = (bool)e.NewValue;

	private static void AllowCollisionOnAlsXyWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedSetsXyWingStepSearcher_AllowCollision = (bool)e.NewValue;

	private static void MaxSizeOfRegularWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).RegularWingStepSearcher_MaxSize = (int)e.NewValue;

	private static void MaxSizeOfComplexFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).ComplexFishStepSearcher_MaxSize = (int)e.NewValue;

	private static void TemplateDeleteOnlyPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).TemplateStepSearcher_TemplateDeleteOnly = (bool)e.NewValue;

	private static void BowmanBingoMaxLengthPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).BowmanBingoStepSearcher_MaxLength = (int)e.NewValue;

	private static void CheckAlmostLockedQuadruplePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = (bool)e.NewValue;

	private static void CheckAdvancedJuniorExocetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).JuniorExocetStepSearcher_CheckAdvanced = (bool)e.NewValue;

	private static void CheckAdvancedSeniorExocetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).SeniorExocetStepSearcher_CheckAdvanced = (bool)e.NewValue;

	private static void SolverIsFullApplyingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).IsFullApplying = (bool)e.NewValue;

	private static void SolverIgnoreSlowAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).IgnoreSlowAlgorithms = (bool)e.NewValue;

	private static void SolverIgnoreHighAllocationAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetProgramSolver((SudokuPane)d).IgnoreHighAllocationAlgorithms = (bool)e.NewValue;
}
