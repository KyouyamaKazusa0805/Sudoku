namespace Sudoku.UI.Views.Pages;

/// <summary>
/// A page that can be used on its own or navigated to within a <see cref="Frame"/>.
/// </summary>
/// <seealso cref="Frame"/>
[Page]
public sealed partial class KeyboardPage : Page
{
	/// <summary>
	/// The keyboard data.
	/// </summary>
	private readonly IList<KeyboardKeyHintInfo> _data = new List<KeyboardKeyHintInfo>
	{
		new(R["KeyboardPage_ControlO"]!, new(ModifierKey.Control, Key.O)),
		new(R["KeyboardPage_ControlS"]!, new(ModifierKey.Control, Key.S)),
		new(R["KeyboardPage_ControlC"]!, new(ModifierKey.Control, Key.C)),
		new(R["KeyboardPage_ControlV"]!, new(ModifierKey.Control, Key.V)),
		new(R["KeyboardPage_ControlTab"]!, new(ModifierKey.Control, Key.Tab)),
		new(R["KeyboardPage_ControlShiftTab"]!, new(ModifierKey.Control | ModifierKey.Shift, Key.Tab)),
		new(R["KeyboardPage_ControlZ"]!, new(ModifierKey.Control, Key.Z)),
		new(R["KeyboardPage_ControlY"]!, new(ModifierKey.Control, Key.Y)),
		new(R["KeyboardPage_ControlH"]!, new(ModifierKey.Control, Key.H)),
		new(R["KeyboardPage_Number0"]!, new(ModifierKey.Control, Key.Number0)),
		new(
			R["KeyboardPage_OtherNumbers"]!,
			new(
				ModifierKey.Control,
				Key.Number1, Key.Number2, Key.Number3, Key.Number4, Key.Number5,
				Key.Number6, Key.Number7, Key.Number8, Key.Number9
			)
		),
		new(
			R["KeyboardPage_ShiftOtherNumbers"]!,
			new(
				ModifierKey.Control | ModifierKey.Shift,
				Key.Number1, Key.Number2, Key.Number3, Key.Number4, Key.Number5,
				Key.Number6, Key.Number7, Key.Number8, Key.Number9
			)
		),
		new(
			R["KeyboardPage_ControlNumbers"]!,
			new(ModifierKey.Control, Key.Number1, Key.Number2, Key.Number3, Key.Number4, Key.Number5, Key.Number6)
		),
		new(
			R["KeyboardPage_ControlShiftNumbers"]!,
			new(ModifierKey.Control | ModifierKey.Shift, Key.Number1, Key.Number2, Key.Number3, Key.Number4)
		),
		new(R["KeyboardPage_ControlBack"]!, new(ModifierKey.Control, Key.Number0, Key.Back)),
		new(R["KeyboardPage_ControlShiftBack"]!, new(ModifierKey.Control | ModifierKey.Shift, Key.Number0, Key.Back)),
		new(R["KeyboardPage_ControlLeft"]!, new(ModifierKey.Control, Key.Left, Key.Right)),
		new(R["KeyboardPage_ControlUp"]!, new(ModifierKey.Control, Key.Up, Key.Down)),
		new(R["KeyboardPage_ControlShiftLeft"]!, new(ModifierKey.Control | ModifierKey.Shift, Key.Left, Key.Right)),
		new(R["KeyboardPage_ControlShiftUp"]!, new(ModifierKey.Control | ModifierKey.Shift, Key.Up, Key.Down)),
		new(R["KeyboardPage_Minus"]!, new(ModifierKey.None, (Key)189)),
		new(R["KeyboardPage_Equals"]!, new(ModifierKey.None, (Key)187)),
		new(R["KeyboardPage_Escape"]!, new(ModifierKey.None, Key.Escape))
	};


	/// <summary>
	/// Initializes an <see cref="KeyboardPage"/> instance.
	/// </summary>
	public KeyboardPage() => InitializeComponent();
}
