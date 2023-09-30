using Microsoft.UI.Xaml.Controls.Primitives;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a technique toggle button.
/// </summary>
[DependencyProperty<TechniqueView>("ParentView", IsNullable = true, DocSummary = "Indicates the parent view.")]
public sealed partial class TechniqueToggleButton : ToggleButton
{
	/// <summary>
	/// Initializes a <see cref="TechniqueToggleButton"/> instance.
	/// </summary>
	public TechniqueToggleButton() => DefaultStyleKey = typeof(ToggleButton);


	/// <inheritdoc/>
	protected override void OnToggle()
	{
		if (ParentView is null || ParentView.SelectionMode == TechniqueViewSelectionMode.None)
		{
			return;
		}

		base.OnToggle();
	}
}
