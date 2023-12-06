using System.SourceGeneration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sudoku.Analytics.Categorization;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using SudokuStudio.Views.Controls;

namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that can select multiple techniques.
/// </summary>
[DependencyProperty<Technique>("CurrentSelectedTechnique", Accessibility = Accessibility.Internal, DocSummary = "Indicates the currently selected technique.")]
public sealed partial class TechniqueSelectionPage : Page
{
	/// <summary>
	/// Initializes a <see cref="TechniqueSelectionPage"/> instance.
	/// </summary>
	public TechniqueSelectionPage() => InitializeComponent();


	private void Page_Loaded(object sender, RoutedEventArgs e)
		=> TechniqueCoreView.SelectedTechniques = ((App)Application.Current).Preference.UIPreferences.GeneratorSelectedTechniques;

	private void TechniqueCoreView_CurrentSelectedTechniqueChanged(TechniqueView sender, TechniqueViewCurrentSelectedTechniqueChangedEventArgs e)
		=> CurrentSelectedTechnique = e.Technique;

	private void TechniqueCoreView_SelectedTechniquesChanged(TechniqueView sender, TechniqueViewSelectedTechniquesChangedEventArgs e)
		=> ((App)Application.Current).Preference.UIPreferences.GeneratorSelectedTechniques = e.TechniqueSet;
}
