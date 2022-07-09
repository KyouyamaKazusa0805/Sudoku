namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Defines an attribute that can be applied to a preference item property,
/// indicating the property is not exposed outside for users' usages.
/// </summary>
/// <remarks>
/// This attribute is not necessary, which means if you found an option isn't related to user,
/// you can mark with this attribute. The attribute only influences the option initialization
/// and displaying to the settings page. For more information please visit the initialization method
/// in type <see cref="MainWindow.ConstructPreferenceItems"/>.
/// </remarks>
/// <seealso cref="MainWindow.ConstructPreferenceItems"/>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class BackgroundPreferenceAttribute : Attribute
{
}
