using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;

namespace Sudoku.UI.Views.Windows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// The app window instance. The method will be initialized by method <see cref="CustomizeTitleBar"/>,
	/// and be immutable after the method called.
	/// </summary>
	/// <seealso cref="CustomizeTitleBar"/>
	private AppWindow _appWindow;


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	public MainWindow()
	{
		// Intializes the controls.
		InitializeComponent();

		// To customize the title bar if available.
		CustomizeTitleBar();
	}


	/// <summary>
	/// Customize the title bar if available.
	/// </summary>
	[MemberNotNull(nameof(_appWindow))]
	private void CustomizeTitleBar()
	{
		// Title bar customization.
		_appWindow = getAppWindowForCurrentWindow();

		// Check to see if customization is supported.
		// Currently only supported on Windows 11.
		if (AppWindowTitleBar.IsCustomizationSupported() && _appWindow.TitleBar is var titleBar)
		{
			// Hide default title bar.
			titleBar.ExtendsContentIntoTitleBar = true;

			// Sets the background color on "those" three buttons to transparent.
			titleBar.ButtonBackgroundColor = Colors.Transparent;
			titleBar.ButtonForegroundColor = Colors.Black;
			titleBar.ButtonHoverBackgroundColor = Colors.LightGray;
			titleBar.ButtonHoverForegroundColor = Colors.Black;
			titleBar.ButtonPressedBackgroundColor = Colors.DimGray;
			titleBar.ButtonPressedForegroundColor = Colors.Black;
			titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
			titleBar.ButtonInactiveForegroundColor = Colors.Gray;
		}
		else
		{
			// Title bar customization using these APIs is currently
			// supported only on Windows 11. In other cases, hide
			// the custom title bar element.
			_cAppTitleBar.Visibility = Visibility.Collapsed;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		AppWindow getAppWindowForCurrentWindow()
		{
			var hWnd = WindowNative.GetWindowHandle(this);
			var wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
			return AppWindow.GetFromWindowId(wndId);
		}
	}
}
