using System.Windows.Input;

namespace Sudoku.Forms
{
	internal static class CustomCommands
	{
		public static RoutedUICommand QuitCommand
		{
			get
			{
				return new RoutedUICommand(
					"Quit",
					"Quit",
					typeof(MainWindow),
					new InputGestureCollection
					{
							new KeyGesture(Key.F4, ModifierKeys.Alt)
					});
			}
		}
	}
}
