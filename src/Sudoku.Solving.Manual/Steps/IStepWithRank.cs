namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a step whose technique used is related to the rank theory,
/// which contains the property <c>Rank</c>.
/// </summary>
public interface IStepWithRank : IStep
{
	/// <summary>
	/// Indicates the rank of the technique structure.
	/// </summary>
	/// <returns>
	/// The return value can be separated with the following cases:
	/// <list type="table">
	/// <listheader>
	/// <term>Range</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><c><![CDATA[> 1]]></c></term>
	/// <description>
	/// The technique cannot be applied any eliminations except the intersection
	/// that all possible branches can see.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c><![CDATA[1]]></c></term>
	/// <description>
	/// The technique can be applied eliminations that is like a normal AIC.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c><![CDATA[0]]></c></term>
	/// <description>
	/// The technique can be applied eliminations that is like a nice loop. For example,
	/// a Sue de Coq.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c><![CDATA[<= 1]]></c></term>
	/// <description>
	/// The negative rank value cannot be formed because the technique structure is at an invalid status.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Please note that sometimes a complex structure may not contain a valid and stable rank value.
	/// You can visit <see href="http://sudoku.allanbarker.com/sweb/old/wL03.htm">this link</see>
	/// for more details about the mixed-rank structures.
	/// </remarks>
	int Rank { get; }
}
