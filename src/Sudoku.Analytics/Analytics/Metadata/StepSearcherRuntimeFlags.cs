namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents a list of cases that describes some cases that <see cref="StepSearcher"/> is partially allowed
/// in searching or gathering operation. Fields in this type can be used
/// by <see cref="StepSearcherAttribute.RuntimeFlags"/> property assigning.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
/// <seealso cref="StepSearcher"/>
/// <seealso cref="StepSearcherAttribute.RuntimeFlags"/>
[Flags]
public enum StepSearcherRuntimeFlags
{
	/// <summary>
	/// Indicates the step searcher can be called anywhere as long it is enabled. This is also the default value of this enumeration type.
	/// </summary>
	Default = 0,

	/// <summary>
	/// Indicates a <see cref="StepSearcher"/> will produce high time complexity,
	/// meaning it will be disabled if a user want to disable high time-complexity algorithms.
	/// </summary>
	TimeComplexity = 1 << 0,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> will produce high space complexity,
	/// meaning it will be disabled if a user want to disable high space-complexity algorithms.
	/// </summary>
	SpaceComplexity = 1 << 1,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> will only produce direct techniques,
	/// which won't be used in indirect views, i.e. all candidates are shown.
	/// </summary>
	DirectTechniquesOnly = 1 << 2,

	/// <summary>
	/// Indicates the <see cref="StepSearcher"/> will only produce indirect techniques,
	/// which won't be used in direct views, i.e. all candidates aren't shown.
	/// </summary>
	IndirectTechniquesOnly = 1 << 3
}
