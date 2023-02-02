namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="TechniqueGroupViewStepChosenEventHandler"/>.
/// </summary>
/// <seealso cref="TechniqueGroupViewStepChosenEventHandler"/>
public sealed class TechniqueGroupViewStepChosenEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a <see cref="TechniqueGroupViewStepChosenEventArgs"/> instance via the specified step.
	/// </summary>
	/// <param name="step">The step.</param>
	public TechniqueGroupViewStepChosenEventArgs(IStep step) => ChosenStep = step;


	/// <summary>
	/// Indicates the chosen step.
	/// </summary>
	public IStep ChosenStep { get; }
}
