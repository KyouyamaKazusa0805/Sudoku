namespace SudokuStudio;

/// <summary>
/// <para>Provides with a project-wide window manager type, as a helper class type.</para>
/// <para>
/// This helper class to allow the app to find the Window that contains an arbitrary <see cref="UIElement"/> (GetWindowForElement).
/// To do this, we keep track of all active <see cref="Window"/>s.
/// </para>
/// <para>
/// The app code must call <see cref="CreateWindow{TWindow}"/> rather than "<see langword="new"/> <see cref="Window"/>()" expression
/// so we can keep track of all the relevant windows.
/// </para>
/// <para>In the future, we would like to support this in platform APIs.</para>
/// </summary>
/// <seealso cref="Window"/>
/// <seealso cref="UIElement"/>
internal sealed class WindowManager
{
	/// <summary>
	/// Indicates the list of active windows.
	/// </summary>
	private readonly List<Window> _activeWindows = [];


	/// <summary>
	/// Indicates the currently active windows.
	/// </summary>
	public IEnumerable<Window> ActiveWindows => _activeWindows;


	/// <summary>
	/// Creates a new window, and records it into the property <see cref="ActiveWindows"/>.
	/// </summary>
	/// <typeparam name="TWindow">The type of the window.</typeparam>
	/// <returns>The created window instance.</returns>
	/// <seealso cref="ActiveWindows"/>
	public TWindow CreateWindow<TWindow>() where TWindow : Window, new()
	{
		var newWindow = new TWindow();

		TrackWindow(newWindow);
		return newWindow;
	}

	/// <summary>
	/// Gets the target window that displays the current <see cref="UIElement"/>.
	/// </summary>
	/// <param name="element">The UI element.</param>
	/// <returns>The target window. If none found, <see langword="null"/>.</returns>
	public Window? GetWindowForElement(UIElement element)
	{
		if (element.XamlRoot is not null)
		{
			foreach (var window in _activeWindows)
			{
				if (element.XamlRoot == window.Content.XamlRoot)
				{
					return window;
				}
			}
		}

		return null;
	}

#if false
	/// <summary>
	/// Find the target element via its name, using the speified UI element as the root of the XAML.
	/// </summary>
	/// <param name="element">The UI element.</param>
	/// <param name="name">The name to be searched for.</param>
	/// <returns>The found element. If none found, <see langword="null"/>.</returns>
	public UIElement? FindElementByName(UIElement element, string name)
		=> element.XamlRoot is { Content: FrameworkElement f } && f.FindName(name) is UIElement ele ? ele : null;
#endif

	/// <summary>
	/// Try to track the window, recording it into the property <see cref="ActiveWindows"/>.
	/// </summary>
	/// <param name="window">The window.</param>
	private void TrackWindow(Window window)
	{
		window.Closed += (_, _) => _activeWindows.Remove(window);

		_activeWindows.Add(window);
	}
}
