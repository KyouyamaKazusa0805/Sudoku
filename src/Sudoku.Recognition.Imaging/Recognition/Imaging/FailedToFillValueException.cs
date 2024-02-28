namespace Sudoku.Recognition.Imaging;

/// <summary>
/// Indicates the exception that throws when the value is failed to fill into a cell.
/// </summary>
/// <param name="cell">Indicates the wrong cell.</param>
/// <param name="digit">Indicates the wrong digit.</param>
public sealed partial class FailedToFillValueException([PrimaryConstructorParameter] Cell cell, [PrimaryConstructorParameter] Digit digit) : Exception
{
	/// <inheritdoc/>
	public override string Message
		=> string.Format(ResourceDictionary.Get("Message_FailedToFillValueException"), (CellMap)Cell, Digit + 1);

	/// <inheritdoc/>
	public override string? HelpLink => null;
}
