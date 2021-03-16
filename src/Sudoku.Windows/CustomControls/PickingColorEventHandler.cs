using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.CustomControls
{
	/// <summary>
	/// Indicates the event handler triggering when the color is start to pick.
	/// </summary>
	/// <param name="color">The color.</param>
	public delegate void PickingColorEventHandler(in WColor color);
}
