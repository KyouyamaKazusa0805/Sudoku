namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a serialization data of a step searcher.
/// </summary>
[TypeImpl(TypeImplFlag.Object_ToString)]
public sealed partial class StepSearcherInfo : DependencyObject
{
	/// <summary>
	/// Indicates whether the technique option is not fixed and can be used for drag-and-drop operation.
	/// </summary>
	public bool CanDrag => !CreateStepSearcher().Metadata.IsOrderingFixed;

	/// <summary>
	/// Indicates whether the technique option is not read-only and can be used for toggle operation.
	/// </summary>
	public bool CanToggle => !CreateStepSearcher().Metadata.IsReadOnly;


	/// <summary>
	/// Indicates whether the step searcher is enabled.
	/// </summary>
	[DependencyProperty(DefaultValue = true)]
	[StringMember]
	public partial bool IsEnabled { get; set; }

	/// <summary>
	/// Indicates the name of the step searcher.
	/// </summary>
	[DependencyProperty]
	[StringMember]
	public partial string Name { get; set; }

	/// <summary>
	/// Indicates the type name of the step searcher.
	/// This property can be used for creating instances via reflection using getMetaProperties <see cref="Activator.CreateInstance(Type)"/>.
	/// </summary>
	/// <seealso cref="Activator.CreateInstance(Type)"/>
	[DependencyProperty]
	[StringMember]
	public partial string TypeName { get; set; }


	/// <summary>
	/// Creates a list of <see cref="StepSearcher"/> instances.
	/// </summary>
	/// <returns>A list of <see cref="StepSearcher"/> instances.</returns>
	public StepSearcher CreateStepSearcher() => StepSearcherPool.GetStepSearcher(TypeName);
}
