namespace Sudoku.UI.Data;

/// <summary>
/// Defines an attribute that is used onto a settable property, to tell the runtime and the user
/// that the property is bound with a UI control, as a preference item.
/// </summary>
/// <typeparam name="TControl">Indicates the type of the control.</typeparam>
/// <typeparam name="TConverter">Indicates the type of the converter.</typeparam>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class PreferenceItemAttribute<TControl, TConverter> : Attribute
where TControl : FrameworkElement
where TConverter : PreferenceItemConverter<TControl>, new()
{
	/// <summary>
	/// Initializes the <see cref="PreferenceItemAttribute{TControl, TConverter}"/> instance
	/// via the specified control name.
	/// </summary>
	/// <param name="controlName">The control name.</param>
	public PreferenceItemAttribute(string controlName) => ControlName = controlName;


	/// <summary>
	/// Indicates the control name that the preference item applied to.
	/// </summary>
	public string ControlName { get; }
}
