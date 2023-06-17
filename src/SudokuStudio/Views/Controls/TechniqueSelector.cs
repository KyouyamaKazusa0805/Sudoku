namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents a technique selector instance.
/// </summary>
[DependencyProperty<Technique>("SelectedTechnique", DocSummary = "Indicates the selected technique.")]
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
			select new TechniqueBindingSource(field, field == 0 ? GetString("TechniqueSelector_NoTechniqueSelected") : field.GetRealOrRawName());
		DisplayMemberPath = nameof(TechniqueBindingSource.DisplayName);
	}


	[Callback]
	private static void SelectedTechniquePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (TechniqueSelector { ItemsSource: TechniqueBindingSource[] source } instance, { NewValue: Technique t })
			&& Array.FindIndex(source, element => element.Technique == t) is var foundTechnique and not -1)
		{
			instance.SelectedItem = source[foundTechnique];
		}
	}
}

/// <summary>
/// Indicates the techique binding source.
/// </summary>
/// <param name="Technique">The technique enumeration field.</param>
/// <param name="DisplayName">The display name.</param>
file readonly record struct TechniqueBindingSource(Technique Technique, string DisplayName);

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Try to fetch the technique name from resource dictionary; if none found, return the enumeration field name itself.
	/// </summary>
	public static string GetRealOrRawName(this Technique @this)
	{
		try
		{
			return @this.GetName();
		}
		catch (ResourceNotFoundException)
		{
			return @this.ToString();
		}
	}
}
