namespace Sudoku.CodeGenerating;

/// <summary>
/// Defines a preference route attribute that marks onto a preference item, to tell the source generator
/// that the preference item is bound with a control (e.g. a <c>ToggleSwitch</c>), and creates a method
/// that delegated to the XAML file.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class PreferenceRouteAttribute : Attribute
{
}
