namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a technique selector instance.
/// </summary>
public sealed partial class TechniqueSelector : ComboBox
{
	/// <summary>
	/// Initializes a <see cref="TechniqueSelector"/> instance.
	/// </summary>
	public TechniqueSelector() => InitializeComponents();


	/// <summary>
	/// Initializes member data.
	/// </summary>
	private void InitializeComponents()
	{
		DefaultStyleKey = typeof(ComboBox);
		ItemsSource =
			from field in Enum.GetValues<Technique>()
			select new TechniqueBindableSource(field, field == 0 ? GetString("TechniqueSelector_NoTechniqueSelected") : field.GetName());
		DisplayMemberPath = nameof(TechniqueBindableSource.DisplayName);
		SelectedValuePath = nameof(TechniqueBindableSource.Technique);
	}
}
