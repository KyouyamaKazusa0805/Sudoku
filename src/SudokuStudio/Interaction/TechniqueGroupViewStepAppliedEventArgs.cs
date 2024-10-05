namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="TechniqueGroupViewStepAppliedEventHandler"/>.
/// </summary>
/// <param name="step">The step.</param>
/// <seealso cref="TechniqueGroupViewStepAppliedEventHandler"/>
public sealed partial class TechniqueGroupViewStepAppliedEventArgs([Property(NamingRule = "Chosen>@")] Step step) : EventArgs;
