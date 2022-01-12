namespace Sudoku.UI.Controls;

/// <summary>
/// Defines a label control that only displays a cell.
/// </summary>
public partial class CellDigitLabel
{
	/// <summary>
	/// Indicates the current cell status.
	/// </summary>
	private CellStatus _status;


	/// <summary>
	/// Initializes a <see cref="CellDigitLabel"/> instance.
	/// </summary>
#nullable disable
	public CellDigitLabel() => InitializeComponent();
#nullable restore


	/// <summary>
	/// Gets or sets the current digit.
	/// </summary>
	/// <value>The value to set. The possible values are between -1 and 8.</value>
	/// <returns>The digit that the current label displays.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the value is not between -1 and 8.</exception>
	public sbyte Digit
	{
		get => _LabelMain.Text is var s && string.IsNullOrEmpty(s) ? (sbyte)-1 : sbyte.Parse(s);

		set
		{
			string valueStr = value.ToString();
			if (valueStr == _LabelMain.Text)
			{
				return;
			}

			if (value is < -1 or >= 9)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			_LabelMain.Text = value == -1 ? string.Empty : valueStr;
		}
	}

	/// <summary>
	/// Indicates the current status of the cell.
	/// </summary>
	/// <value>
	/// The value to set. The possible values are <see cref="CellStatus.Given"/>
	/// or <see cref="CellStatus.Modifiable"/>.
	/// </value>
	/// <returns>The current status.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the value is not <see cref="CellStatus.Given"/> or <see cref="CellStatus.Modifiable"/>.
	/// </exception>
	public CellStatus Status
	{
		get => _status;

		set
		{
			if (_status == value)
			{
				return;
			}

			if (_status is not (CellStatus.Given or CellStatus.Modifiable))
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			_status = value;
		}
	}
}
