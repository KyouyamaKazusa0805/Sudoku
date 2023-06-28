namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Indicates the type is a runnable <see cref="StepSearcher"/> with polymorphism,
/// which means the step searcher type marked this attribute can be used for derived by children types.
/// This attribute is also a "signal" to tell source generators not to create primary constructor invocation clause.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PolymorphismAttribute : Attribute;
