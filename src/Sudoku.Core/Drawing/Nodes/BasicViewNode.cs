namespace Sudoku.Drawing.Nodes;

/// <summary>
/// Defines a basic view node type that provides with basic displaying elements from a grid.
/// </summary>
/// <param name="identifier"><inheritdoc/></param>
public abstract class BasicViewNode(ColorIdentifier identifier) : ViewNode(identifier);
