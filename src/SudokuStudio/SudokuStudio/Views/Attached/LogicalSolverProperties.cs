#pragma warning disable CS1591

namespace SudokuStudio.Views.Attached;

using static SudokuPaneBindable;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances, for <see cref="LogicalSolver"/> instance's interaction.
/// </summary>
/// <seealso cref="SudokuPane"/>
/// <seealso cref="LogicalSolver"/>
public static class LogicalSolverProperties
{
	public static readonly DependencyProperty EnableFullHouseProperty =
		DependencyProperty.RegisterAttached(
			"EnableFullHouse",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).SingleStepSearcher_EnableFullHouse = (bool)e.NewValue)
		);

	public static readonly DependencyProperty EnableLastDigitProperty =
		DependencyProperty.RegisterAttached(
			"EnableLastDigit",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).SingleStepSearcher_EnableLastDigit = (bool)e.NewValue)
		);

	public static readonly DependencyProperty HiddenSinglesInBlockFirstProperty =
		DependencyProperty.RegisterAttached(
			"HiddenSinglesInBlockFirst",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).SingleStepSearcher_HiddenSinglesInBlockFirst = (bool)e.NewValue)
		);

	public static readonly DependencyProperty AllowIncompleteUniqueRectanglesProperty =
		DependencyProperty.RegisterAttached(
			"AllowIncompleteUniqueRectangles",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(
				true,
				static (d, e) => GetProgramSolver((SudokuPane)d).UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = (bool)e.NewValue
			)
		);

	public static readonly DependencyProperty SearchForExtendedUniqueRectanglesProperty =
		DependencyProperty.RegisterAttached(
			"SearchForExtendedUniqueRectangles",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(
				true,
				static (d, e) =>
					GetProgramSolver((SudokuPane)d).UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = (bool)e.NewValue
			)
		);

	public static readonly DependencyProperty SearchExtendedBivalueUniversalGraveTypesProperty =
		DependencyProperty.RegisterAttached(
			"SearchExtendedBivalueUniversalGraveTypes",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(
				true,
				static (d, e) => GetProgramSolver((SudokuPane)d).BivalueUniversalGraveStepSearcher_SearchExtendedTypes = (bool)e.NewValue
			)
		);

	public static readonly DependencyProperty AllowCollisionOnAlsXzProperty =
		DependencyProperty.RegisterAttached(
			"AllowCollisionOnAlsXz",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).AlmostLockedSetsXzStepSearcher_AllowCollision = (bool)e.NewValue)
		);

	public static readonly DependencyProperty AllowLoopedPatternsOnAlsXzProperty =
		DependencyProperty.RegisterAttached(
			"AllowLoopedPatternsOnAlsXz",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = (bool)e.NewValue)
		);

	public static readonly DependencyProperty AllowCollisionOnAlsXyWingProperty =
		DependencyProperty.RegisterAttached(
			"AllowCollisionOnAlsXyWing",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).AlmostLockedSetsXyWingStepSearcher_AllowCollision = (bool)e.NewValue)
		);

	public static readonly DependencyProperty MaxSizeOfRegularWingProperty =
		DependencyProperty.RegisterAttached(
			"MaxSizeOfRegularWing",
			typeof(int),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).RegularWingStepSearcher_MaxSize = (int)e.NewValue)
		);

	public static readonly DependencyProperty MaxSizeOfComplexFishProperty =
		DependencyProperty.RegisterAttached(
			"MaxSizeOfComplexFish",
			typeof(int),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).ComplexFishStepSearcher_MaxSize = (int)e.NewValue)
		);

	public static readonly DependencyProperty TemplateDeleteOnlyProperty =
		DependencyProperty.RegisterAttached(
			"TemplateDeleteOnly",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).TemplateStepSearcher_TemplateDeleteOnly = (bool)e.NewValue)
		);

	public static readonly DependencyProperty BowmanBingoMaxLengthProperty =
		DependencyProperty.RegisterAttached(
			"BowmanBingoMaxLength",
			typeof(int),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).BowmanBingoStepSearcher_MaxLength = (int)e.NewValue)
		);

	public static readonly DependencyProperty CheckAlmostLockedQuadrupleProperty =
		DependencyProperty.RegisterAttached(
			"CheckAlmostLockedQuadruple",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(
				true,
				static (d, e) =>
					GetProgramSolver((SudokuPane)d).AlmostLockedCandidatesStepSearcher_CheckAlmostLockedQuadruple = (bool)e.NewValue
			)
		);

	public static readonly DependencyProperty CheckAdvancedJuniorExocetProperty =
		DependencyProperty.RegisterAttached(
			"CheckAdvancedJuniorExocet",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).JuniorExocetStepSearcher_CheckAdvanced = (bool)e.NewValue)
		);

	public static readonly DependencyProperty CheckAdvancedSeniorExocetProperty =
		DependencyProperty.RegisterAttached(
			"CheckAdvancedSeniorExocet",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).SeniorExocetStepSearcher_CheckAdvanced = (bool)e.NewValue)
		);

	public static readonly DependencyProperty SolverIsFullApplyingProperty =
		DependencyProperty.RegisterAttached(
			"SolverIsFullApplying",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).IsFullApplying = (bool)e.NewValue)
		);

	public static readonly DependencyProperty SolverIgnoreSlowAlgorithmsProperty =
		DependencyProperty.RegisterAttached(
			"SolverIgnoreSlowAlgorithms",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).IgnoreSlowAlgorithms = (bool)e.NewValue)
		);

	public static readonly DependencyProperty SolverIgnoreHighAllocationAlgorithmsProperty =
		DependencyProperty.RegisterAttached(
			"SolverIgnoreHighAllocationAlgorithms",
			typeof(bool),
			typeof(SudokuPaneBindable),
			new(true, static (d, e) => GetProgramSolver((SudokuPane)d).IgnoreHighAllocationAlgorithms = (bool)e.NewValue)
		);


	public static void SetEnableFullHouse(DependencyObject obj, bool value) => obj.SetValue(EnableFullHouseProperty, value);

	public static void SetEnableLastDigit(DependencyObject obj, bool value) => obj.SetValue(EnableLastDigitProperty, value);

	public static void SetHiddenSinglesInBlockFirst(DependencyObject obj, bool value) => obj.SetValue(HiddenSinglesInBlockFirstProperty, value);

	public static void SetAllowIncompleteUniqueRectangles(DependencyObject obj, bool value)
		=> obj.SetValue(AllowIncompleteUniqueRectanglesProperty, value);

	public static void SetSearchForExtendedUniqueRectangles(DependencyObject obj, bool value)
		=> obj.SetValue(SearchForExtendedUniqueRectanglesProperty, value);

	public static void SetSearchExtendedBivalueUniversalGraveTypes(DependencyObject obj, bool value)
		=> obj.SetValue(SearchExtendedBivalueUniversalGraveTypesProperty, value);

	public static void SetAllowCollisionOnAlsXz(DependencyObject obj, bool value) => obj.SetValue(AllowCollisionOnAlsXzProperty, value);

	public static void SetAllowLoopedPatternsOnAlsXz(DependencyObject obj, bool value)
		=> obj.SetValue(AllowLoopedPatternsOnAlsXzProperty, value);

	public static void SetAllowCollisionOnAlsXyWing(DependencyObject obj, bool value)
		=> obj.SetValue(AllowCollisionOnAlsXyWingProperty, value);

	public static void SetMaxSizeOfRegularWing(DependencyObject obj, int value) => obj.SetValue(MaxSizeOfRegularWingProperty, value);

	public static void SetMaxSizeOfComplexFish(DependencyObject obj, int value) => obj.SetValue(MaxSizeOfComplexFishProperty, value);

	public static void SetTemplateDeleteOnly(DependencyObject obj, bool value) => obj.SetValue(TemplateDeleteOnlyProperty, value);

	public static void SetBowmanBingoMaxLength(DependencyObject obj, int value) => obj.SetValue(BowmanBingoMaxLengthProperty, value);

	public static void SetCheckAlmostLockedQuadruple(DependencyObject obj, bool value)
		=> obj.SetValue(CheckAlmostLockedQuadrupleProperty, value);

	public static void SetCheckAdvancedJuniorExocet(DependencyObject obj, bool value)
		=> obj.SetValue(CheckAdvancedJuniorExocetProperty, value);

	public static void SetCheckAdvancedSeniorExocet(DependencyObject obj, bool value)
		=> obj.SetValue(CheckAdvancedSeniorExocetProperty, value);

	public static void SetSolverIsFullApplying(DependencyObject obj, bool value) => obj.SetValue(SolverIsFullApplyingProperty, value);

	public static void SetSolverIgnoreSlowAlgorithms(DependencyObject obj, bool value)
		=> obj.SetValue(SolverIgnoreSlowAlgorithmsProperty, value);

	public static void SetSolverIgnoreHighAllocationAlgorithms(DependencyObject obj, bool value)
		=> obj.SetValue(SolverIgnoreHighAllocationAlgorithmsProperty, value);

	public static bool GetEnableFullHouse(DependencyObject obj) => (bool)obj.GetValue(EnableFullHouseProperty);

	public static bool GetEnableLastDigit(DependencyObject obj) => (bool)obj.GetValue(EnableLastDigitProperty);

	public static bool GetHiddenSinglesInBlockFirst(DependencyObject obj) => (bool)obj.GetValue(HiddenSinglesInBlockFirstProperty);

	public static bool GetAllowIncompleteUniqueRectangles(DependencyObject obj) => (bool)obj.GetValue(AllowIncompleteUniqueRectanglesProperty);

	public static bool GetSearchForExtendedUniqueRectangles(DependencyObject obj)
		=> (bool)obj.GetValue(SearchForExtendedUniqueRectanglesProperty);

	public static bool GetSearchExtendedBivalueUniversalGraveTypes(DependencyObject obj)
		=> (bool)obj.GetValue(SearchExtendedBivalueUniversalGraveTypesProperty);

	public static bool GetAllowCollisionOnAlsXz(DependencyObject obj) => (bool)obj.GetValue(AllowCollisionOnAlsXzProperty);

	public static bool GetAllowLoopedPatternsOnAlsXz(DependencyObject obj) => (bool)obj.GetValue(AllowLoopedPatternsOnAlsXzProperty);

	public static bool GetAllowCollisionOnAlsXyWing(DependencyObject obj) => (bool)obj.GetValue(AllowCollisionOnAlsXyWingProperty);

	public static int GetMaxSizeOfRegularWing(DependencyObject obj) => (int)obj.GetValue(MaxSizeOfRegularWingProperty);

	public static int GetMaxSizeOfComplexFish(DependencyObject obj) => (int)obj.GetValue(MaxSizeOfComplexFishProperty);

	public static bool GetTemplateDeleteOnly(DependencyObject obj) => (bool)obj.GetValue(TemplateDeleteOnlyProperty);

	public static int GetBowmanBingoMaxLength(DependencyObject obj) => (int)obj.GetValue(BowmanBingoMaxLengthProperty);

	public static bool GetCheckAlmostLockedQuadruple(DependencyObject obj) => (bool)obj.GetValue(CheckAlmostLockedQuadrupleProperty);

	public static bool GetCheckAdvancedJuniorExocet(DependencyObject obj) => (bool)obj.GetValue(CheckAdvancedJuniorExocetProperty);

	public static bool GetCheckAdvancedSeniorExocet(DependencyObject obj) => (bool)obj.GetValue(CheckAdvancedSeniorExocetProperty);

	public static bool GetSolverIsFullApplying(DependencyObject obj) => (bool)obj.GetValue(SolverIsFullApplyingProperty);

	public static bool GetSolverIgnoreSlowAlgorithms(DependencyObject obj) => (bool)obj.GetValue(SolverIgnoreSlowAlgorithmsProperty);

	public static bool GetSolverIgnoreHighAllocationAlgorithms(DependencyObject obj)
		=> (bool)obj.GetValue(SolverIgnoreHighAllocationAlgorithmsProperty);
}
