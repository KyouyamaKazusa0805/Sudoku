namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a bindable source for a group of <see cref="Technique"/> instances.
/// </summary>
/// <seealso cref="Technique"/>
[DependencyProperty<bool>("ShowSelectAllButton", DocSummary = "Indicates whether the \"Select all\" button should be shown.")]
[DependencyProperty<TechniqueGroup>("Group", DocSummary = "Indicates the group of the techniques.")]
[DependencyProperty<TechniqueViewBindableSource[]>("Items", DocSummary = "Indicates the items belonging to the group.")]
public sealed partial class TechniqueViewGroupBindableSource : DependencyObject
{
	/// <summary>
	/// Indicates the full name of the group.
	/// </summary>
	public string GroupFullName => (GroupName, ShortenedName) switch { var (a, b) => a == b ? a : $"{a} ({b})" };

	/// <summary>
	/// Indicates the group name.
	/// </summary>
	public string GroupName => Group.GetName(App.CurrentCulture);

	/// <summary>
	/// Indicates the shortened name of the group.
	/// </summary>
	public string ShortenedName => Group.GetShortenedName(App.CurrentCulture);
}
