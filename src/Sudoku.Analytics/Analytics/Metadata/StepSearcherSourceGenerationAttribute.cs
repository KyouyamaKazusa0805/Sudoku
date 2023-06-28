namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an attribute type that can be detected by source generator, controlling for the generation behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StepSearcherSourceGenerationAttribute : Attribute
{
	/// <summary>
	/// Indicates whether the current step searcher can be used for deriving. This property controls the behavior on generated members.
	/// </summary>
	public bool CanDeriveTypes { get; init; }
}
