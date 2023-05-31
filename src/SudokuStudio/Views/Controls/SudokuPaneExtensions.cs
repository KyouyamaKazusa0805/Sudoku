namespace SudokuStudio.Views.Controls;

/// <summary>
/// Provides with extension methods on <see cref="SudokuPane"/>.
/// </summary>
/// <seealso cref="SudokuPane"/>
public static class SudokuPaneExtensions
{
	/// <inheritdoc cref="UpdateViewUnit(SudokuPane, ViewUnitBindableSource?)"/>
	public static void UpdateViewUnit(this SudokuPane @this) => @this.UpdateViewUnit(@this.ViewUnit);

	/// <summary>
	/// Update view unit (add view nodes or remove view nodes).
	/// </summary>
	/// <param name="this">The sudoku pane.</param>
	/// <param name="viewUnit">The view unit bindable source.</param>
	public static void UpdateViewUnit(this SudokuPane @this, ViewUnitBindableSource? viewUnit)
	{
		RenderableFactory.RemoveViewUnitControls(@this);

		if (viewUnit is not null)
		{
			RenderableFactory.AddViewUnitControls(@this, viewUnit);
		}
	}
}
