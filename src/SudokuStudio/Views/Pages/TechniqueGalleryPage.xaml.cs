namespace SudokuStudio.Views.Pages;

/// <summary>
/// A page that displays all techniques.
/// </summary>
public sealed partial class TechniqueGalleryPage : Page
{
	/// <summary>
	/// Initializes a <see cref="TechniqueGalleryPage"/> instance.
	/// </summary>
	public TechniqueGalleryPage() => InitializeComponent();


	/// <summary>
	/// Indicates the currently selected technique.
	/// </summary>
	[DependencyProperty]
	internal partial Technique CurrentSelectedTechnique { get; set; }


	private void TechniqueCoreView_CurrentSelectedTechniqueChanged(TechniqueView sender, TechniqueViewCurrentSelectedTechniqueChangedEventArgs e)
		=> CurrentSelectedTechnique = e.Technique;
}
