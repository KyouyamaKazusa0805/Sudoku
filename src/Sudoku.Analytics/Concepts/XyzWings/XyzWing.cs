namespace Sudoku.Concepts;

/// <summary>
/// Represents an XYZ-Wing pattern.
/// </summary>
/// <param name="digitsMask">Indicates all digits.</param>
/// <param name="zDigit">Indicates the digit Z.</param>
public sealed partial class XyzWing([PrimaryConstructorParameter] Mask digitsMask, [PrimaryConstructorParameter] Digit zDigit);
