using System.Reflection;
using Microsoft.UI.Xaml;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Metadata;
using Sudoku.Analytics.StepSearchers;
using SudokuStudio.ComponentModel;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="Analyzer"/> instance's interaction.
/// </summary>
/// <remarks>
/// All names of attached properties in this type can be corresponded to the target property in one <see cref="StepSearcher"/>,
/// via <see cref="RuntimeIdentifierAttribute"/>.
/// </remarks>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="Analyzer"/>
[AttachedProperty<bool>(RuntimeIdentifier.EnableFullHouse, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.EnableLastDigit, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.HiddenSinglesInBlockFirst, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.AnalyzerUseIttoryuMode)]
[AttachedProperty<bool>(RuntimeIdentifier.AllowIncompleteUniqueRectangles, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.SearchForExtendedUniqueRectangles, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.SearchExtendedBivalueUniversalGraveTypes, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXzRule, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.AllowLoopedPatternsOnAlmostLockedSetXzRule, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.AllowCollisionOnAlmostLockedSetXyWing, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.SearchForReverseBugPartiallyUsedTypes, DefaultValue = true)]
[AttachedProperty<bool>(RuntimeIdentifier.DisableFinnedOrSashimiXWing, DefaultValue = true)]
[AttachedProperty<Count>(RuntimeIdentifier.ReverseBugMaxSearchingEmptyCellsCount, DefaultValue = 2)]
[AttachedProperty<Count>(RuntimeIdentifier.AlignedExclusionMaxSearchingSize, DefaultValue = 3)]
[AttachedProperty<Count>(RuntimeIdentifier.MaxSizeOfRegularWing, DefaultValue = 5)]
[AttachedProperty<Count>(RuntimeIdentifier.MaxSizeOfComplexFish, DefaultValue = 5)]
[AttachedProperty<bool>(RuntimeIdentifier.TemplateDeleteOnly)]
[AttachedProperty<Count>(RuntimeIdentifier.BowmanBingoMaxLength, DefaultValue = 64)]
[AttachedProperty<bool>(RuntimeIdentifier.CheckValueTypes)]
[AttachedProperty<bool>(RuntimeIdentifier.CheckAlmostLockedQuadruple)]
[AttachedProperty<bool>(RuntimeIdentifier.LogicalSolverIsFullApplying)]
[AttachedProperty<bool>(RuntimeIdentifier.LogicalSolverIgnoresSlowAlgorithms)]
[AttachedProperty<bool>(RuntimeIdentifier.LogicalSolverIgnoresHighAllocationAlgorithms)]
[AttachedProperty<List<Technique>>(RuntimeIdentifier.IttoryuSupportedTechniques)]
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


	/// <summary>
	/// Sets the specified property in a <see cref="StepSearcher"/> with the target value via attached properties
	/// stored in type <see cref="AnalyzerProperties"/>.
	/// </summary>
	/// <typeparam name="T">The type of the instance. The type must implement <see cref="AnalyzerOrCollector"/>.</typeparam>
	/// <param name="this">The analyzer instance.</param>
	/// <param name="attachedPropertyValue">The attached property.</param>
	/// <param name="methodName">The name of the property <paramref name="attachedPropertyValue"/>.</param>
	/// <param name="propertyMatched">
	/// A <see cref="bool"/> value indicating whether the property in <see cref="StepSearcher"/> collection is found,
	/// and the target type of that property is same as argument <paramref name="attachedPropertyValue"/>.
	/// </param>
	/// <returns>The same reference as <paramref name="this"/>.</returns>
	/// <seealso cref="AnalyzerProperties"/>
	public static T WithRuntimeIdentifierSetter<T>(this T @this, object attachedPropertyValue, string methodName, out bool propertyMatched)
		where T : AnalyzerOrCollector
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
				=> property.GetCustomAttribute<RuntimeIdentifierAttribute>() is { RuntimeIdentifier: var identifier }
				&& methodName[GetSetterName.Length..] == identifier;
		}

		propertyMatched = false;
		return @this;
	}

	/// <summary>
	/// Calls the method <see cref="WithRuntimeIdentifierSetter{T}(T, object, string?, out bool)"/>
	/// for all properties in type <see cref="AnalyzerProperties"/>.
	/// </summary>
	/// <param name="this">The analyzer instance.</param>
	/// <param name="attachedPane">Indicates the <see cref="SudokuPane"/> instance that all properties in this type attached to.</param>
	/// <returns>The same reference with argument <paramref name="this"/>.</returns>
	/// <exception cref="InvalidOperationException">Throws when the matched property is invalid.</exception>
	/// <seealso cref="WithRuntimeIdentifierSetter{T}(T, object, string?, out bool)"/>
	public static T WithRuntimeIdentifierSetters<T>(this T @this, SudokuPane attachedPane) where T : AnalyzerOrCollector
	{
		foreach (var methodInfo in typeof(AnalyzerProperties).GetMethods(BindingFlags.Static | BindingFlags.Public))
		{
			if (!methodInfo.Name.StartsWith(GetSetterName))
			{
				continue;
			}

			@this.WithRuntimeIdentifierSetter(
				methodInfo.Invoke(null, [attachedPane]) ?? throw new InvalidOperationException("The argument cannot be null."),
				methodInfo.Name,
				out _
			);
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
	private static void AllowIncompleteUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<UniqueRectangleStepSearcher>(d, s => s.AllowIncompleteUniqueRectangles = (bool)e.NewValue);

	[Callback]
	private static void SearchForExtendedUniqueRectanglesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<UniqueRectangleStepSearcher>(d, s => s.SearchForExtendedUniqueRectangles = (bool)e.NewValue);

	[Callback]
	private static void SearchExtendedBivalueUniversalGraveTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<BivalueUniversalGraveStepSearcher>(d, s => s.SearchExtendedTypes = (bool)e.NewValue);

	[Callback]
	private static void AllowCollisionOnAlmostLockedSetXzRulePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXzStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

	[Callback]
	private static void AllowLoopedPatternsOnAlmostLockedSetXzRulePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXzStepSearcher>(d, s => s.AllowLoopedPatterns = (bool)e.NewValue);

	[Callback]
	private static void AllowCollisionOnAlmostLockedSetXyWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedSetsXyWingStepSearcher>(d, s => s.AllowCollision = (bool)e.NewValue);

	[Callback]
	private static void MaxSizeOfRegularWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<RegularWingStepSearcher>(d, s => s.MaxSearchingPivotsCount = (Count)e.NewValue);

	[Callback]
	private static void MaxSizeOfComplexFishPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ComplexFishStepSearcher>(d, s => s.MaxSize = (Count)e.NewValue);

	[Callback]
	private static void TemplateDeleteOnlyPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<TemplateStepSearcher>(d, s => s.TemplateDeleteOnly = (bool)e.NewValue);

	[Callback]
	private static void BowmanBingoMaxLengthPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<BowmanBingoStepSearcher>(d, s => s.MaxLength = (Count)e.NewValue);

	[Callback]
	private static void CheckAlmostLockedQuadruplePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedCandidatesStepSearcher>(d, s => s.CheckAlmostLockedQuadruple = (bool)e.NewValue);

	[Callback]
	private static void CheckValueTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlmostLockedCandidatesStepSearcher>(d, s => s.CheckValueTypes = (bool)e.NewValue);

	[Callback]
	private static void DisableFinnedOrSashimiXWingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<NormalFishStepSearcher>(d, s => s.DisableFinnedOrSashimiXWing = (bool)e.NewValue);

	[Callback]
	private static void SearchForReverseBugPartiallyUsedTypesPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ReverseBivalueUniversalGraveStepSearcher>(d, s => s.AllowPartiallyUsedTypes = (bool)e.NewValue);

	[Callback]
	private static void ReverseBugMaxSearchingEmptyCellsCountPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<ReverseBivalueUniversalGraveStepSearcher>(d, s => s.MaxSearchingEmptyCellsCount = (Count)e.NewValue);

	[Callback]
	private static void AlignedExclusionMaxSearchingSizePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> A<AlignedExclusionStepSearcher>(d, s => s.MaxSearchingSize = (Count)e.NewValue);

	[Callback]
	private static void LogicalSolverIsFullApplyingPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).IsFullApplying = (bool)e.NewValue;

	[Callback]
	private static void LogicalSolverIgnoresSlowAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var analyzer = SudokuPaneBindable.GetAnalyzer((SudokuPane)d);
		analyzer.WithAlgorithmLimits((bool)e.NewValue, analyzer.IgnoreHighAllocationAlgorithms);
	}

	[Callback]
	private static void LogicalSolverIgnoresHighAllocationAlgorithmsPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		var analyzer = SudokuPaneBindable.GetAnalyzer((SudokuPane)d);
		analyzer.WithAlgorithmLimits(analyzer.IgnoreSlowAlgorithms, (bool)e.NewValue);
	}

	private static void A<T>(DependencyObject d, Action<T> action) where T : StepSearcher
		=> SudokuPaneBindable.GetAnalyzer((SudokuPane)d).WithStepSearcherSetters(action);
}
