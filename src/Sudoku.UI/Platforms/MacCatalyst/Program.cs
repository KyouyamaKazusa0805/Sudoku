using UIKit;

namespace Sudoku.UI;

/// <summary>
/// Indicates the main method provider class that is running on Mac catalyst platform.
/// </summary>
public class Program
{
	/// <summary>
	/// This is the main entry point of the application.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	private static void Main(string[] args) =>
		// if you want to use a different Application Delegate class from "AppDelegate"
		// you can specify it here.
		UIApplication.Main(args, null, typeof(AppDelegate));
}
