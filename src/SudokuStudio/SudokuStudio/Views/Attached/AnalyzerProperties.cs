namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="Analyzer"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="Analyzer"/>
[AttachedProperty<bool>("EnableFullHouse", DefaultValue = true)]
[AttachedProperty<bool>("EnableLastDigit", DefaultValue = true)]
[AttachedProperty<bool>("HiddenSinglesInBlockFirst", DefaultValue = true)]
[AttachedProperty<bool>("UseIttoryuMode")]
[AttachedProperty<bool>("AllowIncompleteUniqueRectangles", DefaultValue = true)]
[AttachedProperty<bool>("SearchForExtendedUniqueRectangles", DefaultValue = true)]
[AttachedProperty<bool>("SearchExtendedBivalueUniversalGraveTypes", DefaultValue = true)]
[AttachedProperty<bool>("AllowCollisionOnAlsXz", DefaultValue = true)]
[AttachedProperty<bool>("AllowLoopedPatternsOnAlsXz", DefaultValue = true)]
[AttachedProperty<bool>("AllowCollisionOnAlsXyWing", DefaultValue = true)]
[AttachedProperty<bool>("SearchForReverseBugPartiallyUsedTypes", DefaultValue = true)]
[AttachedProperty<int>("ReverseBugMaxSearchingEmptyCellsCount", DefaultValue = 2)]
[AttachedProperty<int>("AlignedExclusionMaxSearchingSize", DefaultValue = 3)]
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
public static partial class AnalyzerProperties
{
	[Callback]
	private static void EnableFullHousePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<SingleStepSearcher>(d, s => s.EnableFullHouse = (bool)e.NewValue);

	[Callback]
	private static void EnableLastDigitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<SingleStepSearcher>(d, s => s.EnableLastDigit = (bool)e.NewValue);

	[Callback]
	private static void HiddenSinglesInBlockFirstPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<SingleStepSearcher>(d, s => s.HiddenSinglesInBlockFirst = (bool)e.NewValue);

	[Callback]
	private static void UseIttoryuModePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<SingleStepSearcher>(d, s => s.UseIttoryuMode = (bool)e.NewValue);

	[Callback]
	private static void AllowIncompleteUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<UniqueRectangleStepSearcher>(d, s => s.AllowIncompleteUniqueRectangles = (bool)e.NewValue);

	[Callback]
	private static void SearchForExtendedUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<UniqueRectangleStepSearcher>(d, s => s.SearchForExtendedUniqueRectangles = (bool)e.NewValue);

	[Callback]
	private static void SearchExtendedBivalueUniversalGraveTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<BivalueUniversalGraveStepSearcher>(d, s => s.SearchExtendedTypes = (bool)e.NewValue);

	[Callback]
	private static void AllowCollisionOnAlsXzPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXzStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

	[Callback]
	private static void AllowLoopedPatternsOnAlsXzPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXzStepSearcher>(d, s => s.AllowLoopedPatterns = (bool)e.NewValue);

	[Callback]
	private static void AllowCollisionOnAlsXyWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXyWingStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

	[Callback]
	private static void MaxSizeOfRegularWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<RegularWingStepSearcher>(d, s => s.MaxSearchingPivotsCount = (int)e.NewValue);

	[Callback]
	private static void MaxSizeOfComplexFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ComplexFishStepSearcher>(d, s => s.MaxSize = (int)e.NewValue);

	[Callback]
	private static void TemplateDeleteOnlyPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<TemplateStepSearcher>(d, s => s.TemplateDeleteOnly = (bool)e.NewValue);

	[Callback]
	private static void BowmanBingoMaxLengthPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<BowmanBingoStepSearcher>(d, s => s.MaxLength = (int)e.NewValue);

	[Callback]
	private static void CheckAlmostLockedQuadruplePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedCandidatesStepSearcher>(d, s => s.CheckAlmostLockedQuadruple = (bool)e.NewValue);

	[Callback]
	private static void CheckAdvancedJuniorExocetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<JuniorExocetStepSearcher>(d, s => s.CheckAdvanced = (bool)e.NewValue);

	[Callback]
	private static void CheckAdvancedSeniorExocetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<SeniorExocetStepSearcher>(d, s => s.CheckAdvanced = (bool)e.NewValue);

	[Callback]
	private static void SearchForReverseBugPartiallyUsedTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ReverseBivalueUniversalGraveStepSearcher>(d, s => s.AllowPartiallyUsedTypes = (bool)e.NewValue);

	[Callback]
	private static void ReverseBugMaxSearchingEmptyCellsCountPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ReverseBivalueUniversalGraveStepSearcher>(d, s => s.MaxSearchingEmptyCellsCount = (int)e.NewValue);

	[Callback]
	private static void AlignedExclusionMaxSearchingSizePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlignedExclusionStepSearcher>(d, s => s.MaxSearchingSize = (int)e.NewValue);

	[Callback]
	private static void SolverIsFullApplyingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).IsFullApplying = (bool)e.NewValue;

	[Callback]
	private static void SolverIgnoreSlowAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var analyzer = SudokuPaneBindable.GetAnalyzer((SudokuPane)d);
		analyzer.WithAlgorithmLimits((bool)e.NewValue, analyzer.IgnoreHighAllocationAlgorithms);
	}

	[Callback]
	private static void SolverIgnoreHighAllocationAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var analyzer = SudokuPaneBindable.GetAnalyzer((SudokuPane)d);
		analyzer.WithAlgorithmLimits(analyzer.IgnoreSlowAlgorithms, (bool)e.NewValue);
	}

	private static void A<T>(DependencyObject d, Action<T> action) where T : StepSearcher
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).WithStepSearcherSetters(action);
}
