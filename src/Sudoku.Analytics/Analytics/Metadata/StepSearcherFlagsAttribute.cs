namespace Sudoku.Analytics.Metadata;

/// <summary>
/// <para>
/// Represents an attribute type that contains condition flags,
/// describing the condition cases that the step searcher can run on.
/// </para>
/// <para>
/// For example, Deadly Patterns are unavailable for Sukaku puzzles because we cannot determine
/// whether a candidate is having been removed before.
/// </para>
/// </summary>
/// <param name="flags">
/// Indicates the flags to be set. You can use <see cref="StepSearcherFlags"/>.<see langword="operator"/>
/// |(<see cref="StepSearcherFlags"/>, <see cref="StepSearcherFlags"/>) to merge multiple flags.
/// </param>
/// <seealso cref="StepSearcherFlags"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class StepSearcherFlagsAttribute([RecordParameter] StepSearcherFlags flags) : Attribute;
