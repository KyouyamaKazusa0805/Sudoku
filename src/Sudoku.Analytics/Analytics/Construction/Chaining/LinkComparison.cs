namespace Sudoku.Analytics.Construction.Chaining;

/// <summary>
/// Represents a comparison rule to be used on comparing with two <see cref="Link"/> instances.
/// </summary>
/// <seealso cref="Link"/>
public enum LinkComparison
{
	/// <summary>
	/// Indicates the comparing rule is undirected, which means two <see cref="Link"/> instances will be compared,
	/// without any direction checking; i.e. <c><![CDATA[node1 -> node2]]></c> is equal to <c><![CDATA[node2 -> node1]]></c>.
	/// This is the default option on comparing.
	/// </summary>
	Undirected,

	/// <summary>
	/// Indicates the comparing rule is directed, which means two <see cref="Link"/> instances will be compared,
	/// with considerations on direction; i.e. <c><![CDATA[node1 -> node2]]></c> is not equal to <c><![CDATA[node2 -> node1]]></c>.
	/// </summary>
	Directed
}
