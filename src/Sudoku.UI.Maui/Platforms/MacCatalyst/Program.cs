using UIKit;

namespace Sudoku.UI.Maui;

/// <summary>
/// The program type.
/// </summary>
public class Program
{
	/// <summary>
	/// This is the main entry point of the application.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	private static void Main(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
