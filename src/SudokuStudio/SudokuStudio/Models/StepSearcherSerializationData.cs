namespace SudokuStudio.Models;

/// <summary>
/// Defines a serialization data of a step searcher.
/// </summary>
[DependencyProperty<bool>("IsEnabled", DefaultValue = true, DocSummary = "Indicates whether the step searcher is enabled.")]
[DependencyProperty<string>("Name", DocSummary = "Indicates the name of the step searcher.")]
[DependencyProperty<string>("TypeName", DocSummary = "Indicates the type name of the step searcher. This property can be used for creating instances via reflection using method <see cref=\"Activator.CreateInstance(Type)\"/>.")]
[GeneratedOverloadingOperator(GeneratedOperator.EqualityOperators)]
public sealed partial class StepSearcherSerializationData :
	DependencyObject,
	IEquatable<StepSearcherSerializationData>,
	IEqualityOperators<StepSearcherSerializationData, StepSearcherSerializationData, bool>
{
	[GeneratedOverriddingMember(GeneratedEqualsBehavior.AsCastAndCallingOverloading)]
	public override partial bool Equals(object? obj);

	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] StepSearcherSerializationData? other)
		=> other is not null && IsEnabled == other.IsEnabled && Name == other.Name && TypeName == other.TypeName;

	[GeneratedOverriddingMember(GeneratedGetHashCodeBehavior.CallingHashCodeCombine, "IsEnabled", "Name", "TypeName")]
	public override partial int GetHashCode();

	[GeneratedOverriddingMember(GeneratedToStringBehavior.RecordLike, "IsEnabled", "Name", "TypeName")]
	public override partial string ToString();

	/// <summary>
	/// Creates a list of <see cref="IStepSearcher"/> instances.
	/// </summary>
	/// <returns>A list of <see cref="IStepSearcher"/> instances.</returns>
	public IStepSearcher[] CreateStepSearchers()
		=> StepSearcherPool.GetStepSearchers(typeof(IStepSearcher).Assembly.GetType($"Sudoku.Solving.Logical.StepSearchers.{TypeName}")!, true);
}
