using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;
using SudokuStudio.BindableSource;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a technique toggle button.
/// </summary>
[DependencyProperty<TechniqueView>("ParentView", IsNullable = true, DocSummary = "Indicates the parent view.")]
[DependencyProperty<TechniqueViewBindableSource>("Source", DocSummary = "Indicates the source.")]
public sealed partial class TechniqueToggleButton : ToggleButton
{
	/// <summary>
	/// Initializes a <see cref="TechniqueToggleButton"/> instance.
	/// </summary>
	public TechniqueToggleButton()
	{
		DefaultStyleKey = typeof(ToggleButton);
		BorderThickness = new(1.5);
	}


	/// <inheritdoc/>
	protected override void OnToggle()
	{
		if (ParentView is { SelectionMode: not TechniqueViewSelectionMode.None })
		{
			base.OnToggle();
		}
	}
}
