namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for source generation on properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NotifyBackingFieldAttribute : Attribute
{
	/// <summary>
	/// Indicates whether the source generator doesn't emit the source code to trigger changed event
	/// <see cref="INotifyPropertyChanged.PropertyChanged"/>.
	/// </summary>
	/// <seealso cref="INotifyPropertyChanged.PropertyChanged"/>
	public bool DoNotEmitPropertyChangedEventTrigger { get; init; } = false;

	/// <summary>
	/// Indicates whether the source generator use comparison inferring to generate source code for comparing two objects.
	/// </summary>
	public bool DisableComparison { get; init; } = false;

	/// <summary>
	/// Indicates the accessibility of the generated property. The default value is <see cref="GeneralizedAccessibility.Public"/>.
	/// </summary>
	/// <seealso cref="GeneralizedAccessibility.Public"/>
	public GeneralizedAccessibility Accessibility { get; init; } = GeneralizedAccessibility.Public;
}
