namespace SudokuStudio.ComponentModel;

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
	public bool DisableEventTrigger { get; init; } = false;

	/// <summary>
	/// Indicates whether the source generator doesn't emit the source code to apply <see cref="DebuggerStepThroughAttribute"/>.
	/// </summary>
	/// <seealso cref="DebuggerStepThroughAttribute"/>
	public bool DisableDebuggerStepThrough { get; init; } = false;

	/// <summary>
	/// Indicates the customized comparison rule to be used to compare two objects.
	/// </summary>
	public EqualityComparisonMode ComparisonMode { get; init; } = EqualityComparisonMode.Intelligent;

	/// <summary>
	/// Indicates the accessibility of the generated property. The default value is <see cref="GeneralizedAccessibility.Public"/>.
	/// </summary>
	/// <seealso cref="GeneralizedAccessibility.Public"/>
	public GeneralizedAccessibility Accessibility { get; init; } = GeneralizedAccessibility.Public;
}
