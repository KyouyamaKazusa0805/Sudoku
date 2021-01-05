using System.Windows.Media;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Indicates the event handler when a color is picking.
	/// </summary>
	/// <param name="color">(<see langword="in"/> parameter) The color.</param>
	public delegate void PickingColorHandlerEventHandler(in Color color);
}
