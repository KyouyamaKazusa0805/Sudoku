namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="TechniqueGroupViewStepChosenEventHandler"/>.
/// </summary>
/// <param name="step">The step.</param>
/// <seealso cref="TechniqueGroupViewStepChosenEventHandler"/>
public sealed partial class TechniqueGroupViewStepChosenEventArgs([RecordParameter(GeneratedMemberName = "ChosenStep")] Step step) : EventArgs;
