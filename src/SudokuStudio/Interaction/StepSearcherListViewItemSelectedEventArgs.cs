namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="TechniqueGroupViewStepAppliedEventHandler"/>.
/// </summary>
/// <param name="selectedSearcherInfo">The selected searcher's information.</param>
/// <seealso cref="TechniqueGroupViewStepAppliedEventHandler"/>
public sealed partial class StepSearcherListViewItemSelectedEventArgs([Property] StepSearcherInfo selectedSearcherInfo) : EventArgs;
