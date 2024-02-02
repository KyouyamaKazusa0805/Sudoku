namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="TechniqueViewSelectedTechniquesChangedEventHandler"/>.
/// </summary>
/// <param name="techniqueSet">The technique set to be assigned.</param>
/// <seealso cref="TechniqueViewSelectedTechniquesChangedEventHandler"/>
public sealed partial class TechniqueViewSelectedTechniquesChangedEventArgs([PrimaryCosntructorParameter] TechniqueSet techniqueSet) : EventArgs;
