namespace Microsoft.UI.Xaml;

/// <summary>
/// Defines a set of commonly-used methods.
/// </summary>
internal static class CommonMethods
{
	/// <summary>
	/// To hide a control.
	/// </summary>
	/// <typeparam name="TUIElement">The type of the control.</typeparam>
	/// <param name="control">The control instance.</param>
	public static void HideControl<TUIElement>(TUIElement control) where TUIElement : UIElement
		=> control.Visibility = Visibility.Collapsed;
}
