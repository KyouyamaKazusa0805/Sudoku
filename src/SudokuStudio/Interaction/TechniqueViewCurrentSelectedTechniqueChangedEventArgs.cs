namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="TechniqueViewCurrentSelectedTechniqueChangedEventHandler"/>.
/// </summary>
/// <param name="technique">The technique to be assigned.</param>
/// <param name="isSelected">Indicates whether the field is selected.</param>
/// <seealso cref="TechniqueViewCurrentSelectedTechniqueChangedEventHandler"/>
public sealed partial class TechniqueViewCurrentSelectedTechniqueChangedEventArgs([Property] Technique technique, [Property] bool isSelected) : EventArgs;
