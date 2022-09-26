namespace Sudoku.Solving.Logical.Steps.Specialized;

/// <summary>
/// Defines a step whose technique used is the elementary one.
/// </summary>
/// <remarks>
/// Here we define that the techniques often appearing and commonly to be used as below are elementary:
/// <list type="bullet">
/// <item>Full House, Last Digit, Hidden Single, Naked Single</item>
/// <item>Pointing, Claiming</item>
/// <item>Naked Pair, Naked Triple, Naked Quadruple</item>
/// <item>Naked Pair (+), Naked Triple (+), Naked Quadruple (+)</item>
/// <item>Hidden Pair, Hidden Triple, Hidden Quadruple</item>
/// <item>Locked Pair, Locked Triple</item>
/// </list>
/// </remarks>
public interface IElementaryStep : IStep
{
}
