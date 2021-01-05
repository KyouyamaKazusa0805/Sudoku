using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Indicates the event handler triggering when the color is start to pick.
	/// </summary>
	/// <param name="color">(<see langword="in"/> parameter) The color.</param>
	public delegate void PickingColorEventHandler(in WColor color);
}
