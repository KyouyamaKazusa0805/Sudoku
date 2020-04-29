using System.Windows.Controls;
using Sudoku.Drawing.Layers;

namespace Sudoku.Windows
{
	partial class MainWindow
	{
		private void TabControlInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_tabControlInfo.SelectedItem == _tabItemDrawing)
			{
				_layerCollection.Remove(typeof(ViewLayer).Name);

				UpdateImageGrid();
			}
		}
	}
}
