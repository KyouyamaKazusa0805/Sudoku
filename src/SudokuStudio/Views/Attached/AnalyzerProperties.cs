namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="Analyzer"/> instance's interaction.
/// </summary>
/// <remarks>
/// All names of attached properties in this type can be corresponded to the target property in one <see cref="StepSearcher"/>,
/// via <see cref="SettingItemNameAttribute"/>.
/// </remarks>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="Analyzer"/>
public static partial class AnalyzerProperties
{
	/// <summary>
	/// Indicates the anonymous name for getters.
	/// </summary>
	private const string GetSetterName = "Get";


	[Default]
	private static readonly List<Technique> IttoryuSupportedTechniquesDefaultValue = [
		Technique.FullHouse,
		Technique.HiddenSingleBlock,
		Technique.HiddenSingleRow,
		Technique.HiddenSingleColumn,
		Technique.NakedSingle
	];


	/// <inheritdoc cref="Analyzer.IsFullApplying"/>
	[DependencyProperty]
	public static partial bool AnalyzerIsFullApplying { get; set; }

	/// <inheritdoc cref="Analyzer.IgnoreSlowAlgorithms"/>
	[DependencyProperty]
	public static partial bool AnalyzerIgnoresSlowAlgorithms { get; set; }

	/// <inheritdoc cref="Analyzer.IgnoreHighAllocationAlgorithms"/>
	[DependencyProperty]
	public static partial bool AnalyzerIgnoresHighAllocationAlgorithms { get; set; }


	/// <inheritdoc cref="DisorderedIttoryuFinder.SupportedTechniques"/>
	[DependencyProperty]
	public static partial List<Technique> IttoryuSupportedTechniques { get; set; }


	/// <inheritdoc cref="SingleStepSearcher.EnableFullHouse"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool EnableFullHouse { get; set; }

	/// <inheritdoc cref="SingleStepSearcher.EnableLastDigit"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool EnableLastDigit { get; set; }

	/// <inheritdoc cref="SingleStepSearcher.HiddenSinglesInBlockFirst"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool HiddenSinglesInBlockFirst { get; set; }

	/// <inheritdoc cref="SingleStepSearcher.UseIttoryuMode"/>
	[DependencyProperty]
	public static partial bool AnalyzerUseIttoryuMode { get; set; }

	/// <inheritdoc cref="DirectIntersectionStepSearcher.AllowDirectPointing"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowDirectPointing { get; set; }

	/// <inheritdoc cref="DirectIntersectionStepSearcher.AllowDirectClaiming"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowDirectClaiming { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectLockedSubset"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowDirectLockedSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectNakedSubset"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowDirectNakedSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectLockedHiddenSubset"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowDirectLockedHiddenSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.AllowDirectHiddenSubset"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowDirectHiddenSubset { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.DirectNakedSubsetMaxSize"/>
	[DependencyProperty(DefaultValue = 2)]
	public static partial int DirectNakedSubsetMaxSize { get; set; }

	/// <inheritdoc cref="DirectSubsetStepSearcher.DirectHiddenSubsetMaxSize"/>
	[DependencyProperty(DefaultValue = 2)]
	public static partial int DirectHiddenSubsetMaxSize { get; set; }

	/// <inheritdoc cref="ComplexSingleStepSearcher.NakedSubsetMaxSize"/>
	[DependencyProperty(DefaultValue = 4)]
	public static partial int NakedSubsetMaxSizeInComplexSingle { get; set; }

	/// <inheritdoc cref="ComplexSingleStepSearcher.HiddenSubsetMaxSize"/>
	[DependencyProperty(DefaultValue = 4)]
	public static partial int HiddenSubsetMaxSizeInComplexSingle { get; set; }

	/// <inheritdoc cref="NormalFishStepSearcher.DisableFinnedOrSashimiXWing"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool DisableFinnedOrSashimiXWing { get; set; }

	/// <inheritdoc cref="GroupedTwoStrongLinksStepSearcher.DisableGroupedTurbotFish"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool DisableGroupedTurbotFish { get; set; }

	/// <inheritdoc cref="NormalFishStepSearcher.AllowSiamese"/>
	[DependencyProperty]
	public static partial bool AllowSiameseNormalFish { get; set; }

	/// <inheritdoc cref="ComplexFishStepSearcher.AllowSiamese"/>
	[DependencyProperty]
	public static partial bool AllowSiameseComplexFish { get; set; }

	/// <inheritdoc cref="ComplexFishStepSearcher.MaxSize"/>
	[DependencyProperty(DefaultValue = 5)]
	public static partial int MaxSizeOfComplexFish { get; set; }

	/// <inheritdoc cref="RegularWingStepSearcher.MaxSearchingPivotsCount"/>
	[DependencyProperty(DefaultValue = 5)]
	public static partial int MaxSizeOfRegularWing { get; set; }

	/// <inheritdoc cref="UniqueRectangleStepSearcher.AllowIncompleteUniqueRectangles"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowIncompleteUniqueRectangles { get; set; }

	/// <inheritdoc cref="UniqueRectangleStepSearcher.SearchForExtendedUniqueRectangles"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool SearchForExtendedUniqueRectangles { get; set; }

	/// <inheritdoc cref="BivalueUniversalGraveStepSearcher.SearchExtendedTypes"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool SearchExtendedBivalueUniversalGraveTypes { get; set; }

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckValueTypes"/>
	[DependencyProperty]
	public static partial bool AlmostLockedCandidatesCheckValueTypes { get; set; }

	/// <inheritdoc cref="AlmostLockedCandidatesStepSearcher.CheckAlmostLockedQuadruple"/>
	[DependencyProperty]
	public static partial bool CheckAlmostLockedQuadruple { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowCollision"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowCollisionOnAlmostLockedSetsXzRule { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsXzStepSearcher.AllowLoopedPatterns"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowLoopedPatternsOnAlmostLockedSetsXzRule { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsXyWingStepSearcher.AllowCollision"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowCollisionOnAlmostLockedSetsXyWing { get; set; }

	/// <inheritdoc cref="AlmostLockedSetsWWingStepSearcher.AllowCollision"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool AllowCollisionOnAlmostLockedSetsWWing { get; set; }

	/// <inheritdoc cref="DeathBlossomStepSearcher.SearchExtendedTypes"/>
	[DependencyProperty]
	public static partial bool SearchExtendedDeathBlossomTypes { get; set; }

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.AllowPartiallyUsedTypes"/>
	[DependencyProperty(DefaultValue = true)]
	public static partial bool SearchForReverseBugPartiallyUsedTypes { get; set; }

	/// <inheritdoc cref="ReverseBivalueUniversalGraveStepSearcher.MaxSearchingEmptyCellsCount"/>
	[DependencyProperty(DefaultValue = 2)]
	public static partial int ReverseBugMaxSearchingEmptyCellsCount { get; set; }

	/// <inheritdoc cref="XyzRingStepSearcher.AllowSiamese"/>
	[DependencyProperty]
	public static partial bool AllowSiameseXyzRing { get; set; }

	/// <inheritdoc cref="AlignedExclusionStepSearcher.MaxSearchingSize"/>
	[DependencyProperty(DefaultValue = 3)]
	public static partial int AlignedExclusionMaxSearchingSize { get; set; }

	/// <inheritdoc cref="TemplateStepSearcher.TemplateDeleteOnly"/>
	[DependencyProperty]
	public static partial bool TemplateDeleteOnly { get; set; }

	/// <inheritdoc cref="BowmanBingoStepSearcher.MaxLength"/>
	[DependencyProperty(DefaultValue = 64)]
	public static partial int BowmanBingoMaxLength { get; set; }


	/// <summary>
	/// Sets the specified property in a <see cref="StepSearcher"/> with the target value via attached properties
	/// stored in type <see cref="AnalyzerProperties"/>.
	/// </summary>
	/// <typeparam name="TAnalyzerOrCollector">
	/// The type of the instance. The type must implement <see cref="IAnalyzerOrCollector{TSelf, TContext, TResult}"/>.
	/// </typeparam>
	/// <typeparam name="TContext">The type of the context.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="this">The analyzer instance.</param>
	/// <param name="attachedPropertyValue">The attached property.</param>
	/// <param name="methodName">The name of the property <paramref name="attachedPropertyValue"/>.</param>
	/// <param name="propertyMatched">
	/// A <see cref="bool"/> value indicating whether the property in <see cref="StepSearcher"/> collection is found,
	/// and the target type of that property is same as argument <paramref name="attachedPropertyValue"/>.
	/// </param>
	/// <returns>The same reference as <paramref name="this"/>.</returns>
	/// <seealso cref="AnalyzerProperties"/>
	public static TAnalyzerOrCollector WithRuntimeIdentifierSetter<TAnalyzerOrCollector, TContext, TResult>(
		this TAnalyzerOrCollector @this,
		object attachedPropertyValue,
		string methodName,
		out bool propertyMatched
	)
		where TAnalyzerOrCollector : IAnalyzerOrCollector<TAnalyzerOrCollector, TContext, TResult>, allows ref struct
		where TContext : allows ref struct
		where TResult : allows ref struct
	{
		var targetStepSearcherCollection = @this.ResultStepSearchers;
		foreach (var searcher in targetStepSearcherCollection)
		{
			if (searcher.GetType().GetProperties().FirstOrDefault(methodNameMatcher) is { } propertyInfo)
			{
				propertyInfo.SetValue(searcher, attachedPropertyValue);
				propertyMatched = true;
				return @this;
			}


			bool methodNameMatcher(PropertyInfo property)
				=> property.GetCustomAttribute<SettingItemNameAttribute>() is { Identifier: var identifier }
				&& methodName[GetSetterName.Length..] == identifier;
		}

		propertyMatched = false;
		return @this;
	}

	/// <summary>
	/// Calls the method <see cref="WithRuntimeIdentifierSetter"/> for all properties in type <see cref="AnalyzerProperties"/>.
	/// </summary>
	/// <typeparam name="TAnalyzerOrCollector">
	/// The type of the instance. The type must implement <see cref="IAnalyzerOrCollector{TSelf, TContext, TResult}"/>.
	/// </typeparam>
	/// <typeparam name="TContext">The type of the context.</typeparam>
	/// <typeparam name="TResult">The type of the result.</typeparam>
	/// <param name="this">The analyzer instance.</param>
	/// <param name="attachedPane">Indicates the <see cref="SudokuPane"/> instance that all properties in this type attached to.</param>
	/// <returns>The same reference with argument <paramref name="this"/>.</returns>
	/// <exception cref="InvalidOperationException">Throws when the matched property is invalid.</exception>
	/// <seealso cref="WithRuntimeIdentifierSetter"/>
	public static TAnalyzerOrCollector WithRuntimeIdentifierSetters<TAnalyzerOrCollector, TContext, TResult>(this TAnalyzerOrCollector @this, SudokuPane attachedPane)
		where TAnalyzerOrCollector : IAnalyzerOrCollector<TAnalyzerOrCollector, TContext, TResult>, allows ref struct
		where TContext : allows ref struct
		where TResult : allows ref struct
	{
		foreach (var methodInfo in typeof(AnalyzerProperties).GetMethods(BindingFlags.Static | BindingFlags.Public))
		{
			if (methodInfo.Name.StartsWith(GetSetterName))
			{
				@this.WithRuntimeIdentifierSetter<TAnalyzerOrCollector, TContext, TResult>(
					methodInfo.Invoke(null, [attachedPane])!,
					methodInfo.Name,
					out _
				);
			}
		}
		return @this;
	}

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
	private static void AnalyzerUseIttoryuModePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<SingleStepSearcher>(d, s => s.UseIttoryuMode = (bool)e.NewValue);

	[Callback]
	private static void NakedSubsetMaxSizeInComplexSinglePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ComplexSingleStepSearcher>(d, s => s.NakedSubsetMaxSize = (int)e.NewValue);

	[Callback]
	private static void AllowDirectPointingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectIntersectionStepSearcher>(d, s => s.AllowDirectPointing = (bool)e.NewValue);

	[Callback]
	private static void AllowDirectClaimingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectIntersectionStepSearcher>(d, s => s.AllowDirectClaiming = (bool)e.NewValue);

	[Callback]
	private static void AllowDirectLockedSubsetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectSubsetStepSearcher>(d, s => s.AllowDirectLockedSubset = (bool)e.NewValue);

	[Callback]
	private static void AllowDirectLockedHiddenSubsetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectSubsetStepSearcher>(d, s => s.AllowDirectLockedHiddenSubset = (bool)e.NewValue);

	[Callback]
	private static void AllowDirectNakedSubsetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectSubsetStepSearcher>(d, s => s.AllowDirectNakedSubset = (bool)e.NewValue);

	[Callback]
	private static void AllowDirectHiddenSubsetPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectSubsetStepSearcher>(d, s => s.AllowDirectHiddenSubset = (bool)e.NewValue);

	[Callback]
	private static void DirectNakedSubsetMaxSizePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectSubsetStepSearcher>(d, s => s.DirectNakedSubsetMaxSize = (int)e.NewValue);

	[Callback]
	private static void DirectHiddenSubsetMaxSizePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DirectSubsetStepSearcher>(d, s => s.DirectHiddenSubsetMaxSize = (int)e.NewValue);

	[Callback]
	private static void HiddenSubsetMaxSizeInComplexSinglePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ComplexSingleStepSearcher>(d, s => s.HiddenSubsetMaxSize = (int)e.NewValue);

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
	private static void AllowCollisionOnAlmostLockedSetsXzRulePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXzStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

	[Callback]
	private static void AllowLoopedPatternsOnAlmostLockedSetsXzRulePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXzStepSearcher>(d, s => s.AllowLoopedPatterns = (bool)e.NewValue);

	[Callback]
	private static void AllowCollisionOnAlmostLockedSetsXyWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXyWingStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

	[Callback]
	private static void AllowCollisionOnAlmostLockedSetsWWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsWWingStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

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
	private static void CheckAlmostLockedCandidatesValueTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedCandidatesStepSearcher>(d, s => s.CheckValueTypes = (bool)e.NewValue);

	[Callback]
	private static void DisableFinnedOrSashimiXWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<NormalFishStepSearcher>(d, s => s.DisableFinnedOrSashimiXWing = (bool)e.NewValue);

	[Callback]
	private static void DisableGroupedTurbotFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<GroupedTwoStrongLinksStepSearcher>(d, s => s.DisableGroupedTurbotFish = (bool)e.NewValue);

	[Callback]
	private static void AllowSiameseNormalFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<NormalFishStepSearcher>(d, s => s.AllowSiamese = (bool)e.NewValue);

	[Callback]
	private static void AllowSiameseComplexFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ComplexFishStepSearcher>(d, s => s.AllowSiamese = (bool)e.NewValue);

	[Callback]
	private static void AllowSiameseXyzRingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<XyzRingStepSearcher>(d, s => s.AllowSiamese = (bool)e.NewValue);

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
	private static void SearchExtendedDeathBlossomTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<DeathBlossomStepSearcher>(d, s => s.SearchExtendedTypes = (bool)e.NewValue);

	[Callback]
	private static void AnalyzerIsFullApplyingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).IsFullApplying = (bool)e.NewValue;

	[Callback]
	private static void AnalyzerIgnoresSlowAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).WithIgnoreHighTimeComplexityStepSearchers((bool)e.NewValue);

	[Callback]
	private static void AnalyzerIgnoresHighAllocationAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).WithIgnoreHighSpaceComplexityStepSearchers((bool)e.NewValue);

	private static void A<TStepSearcher>(DependencyObject d, Action<TStepSearcher> action) where TStepSearcher : StepSearcher
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).AddStepSearcherSetter(action);
}
