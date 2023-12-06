using SudokuStudio.Views.Controls;

namespace SudokuStudio.Interaction;

/// <summary>
/// Provides an event handler that is used by <see cref="TechniqueView.CurrentSelectedTechniqueChanged"/>.
/// </summary>
/// <param name="sender">An object which triggers the event.</param>
/// <param name="e">The event data provided.</param>
/// <seealso cref="TechniqueView.CurrentSelectedTechniqueChanged"/>
public delegate void TechniqueViewCurrentSelectedTechniqueChangedEventHandler(TechniqueView sender, TechniqueViewCurrentSelectedTechniqueChangedEventArgs e);
