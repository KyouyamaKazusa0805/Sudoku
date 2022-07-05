using Key = Windows.System.VirtualKey;
using Modifier = Windows.System.VirtualKeyModifiers;

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
		new(R["KeyboardPage_ControlO"]!, new(Modifier.Control, Key.O)),
		new(R["KeyboardPage_ControlS"]!, new(Modifier.Control, Key.S)),
		new(R["KeyboardPage_ControlC"]!, new(Modifier.Control, Key.C)),
		new(R["KeyboardPage_ControlV"]!, new(Modifier.Control, Key.V)),
		new(R["KeyboardPage_ControlTab"]!, new(Modifier.Control, Key.Tab)),
		new(R["KeyboardPage_ControlShiftTab"]!, new(Modifier.Control | Modifier.Shift, Key.Tab)),
		new(R["KeyboardPage_ControlZ"]!, new(Modifier.Control, Key.Z)),
		new(R["KeyboardPage_ControlY"]!, new(Modifier.Control, Key.Y)),
		new(R["KeyboardPage_ControlH"]!, new(Modifier.Control, Key.H)),
		new(R["KeyboardPage_Number0"]!, new(Modifier.Control, Key.Number0)),
		new(
			R["KeyboardPage_OtherNumbers"]!,
			new(
				Modifier.Control,
				new[]
				{
					Key.Number1, Key.Number2, Key.Number3,
					Key.Number4, Key.Number5, Key.Number6,
					Key.Number7, Key.Number8, Key.Number9
				}
			)
		),
		new(
			R["KeyboardPage_ShiftOtherNumbers"]!,
			new(
				Modifier.Control | Modifier.Shift,
				new[]
				{
					Key.Number1, Key.Number2, Key.Number3,
					Key.Number4, Key.Number5, Key.Number6,
					Key.Number7, Key.Number8, Key.Number9
				}
			)
		),
		new(
			R["KeyboardPage_ControlNumbers"]!,
			new(Modifier.Control, new[] { Key.Number1, Key.Number2, Key.Number3, Key.Number4, Key.Number5, Key.Number6 })
		),
		new(R["KeyboardPage_ControlShiftNumbers"]!, new(Modifier.Control | Modifier.Shift, new[] { Key.Number1, Key.Number2, Key.Number3, Key.Number4 })),
		new(R["KeyboardPage_ControlBack"]!, new(Modifier.Control, new[] { Key.Number0, Key.Back })),
		new(R["KeyboardPage_ControlShiftBack"]!, new(Modifier.Control | Modifier.Shift, new[] { Key.Number0, Key.Back })),
		new(R["KeyboardPage_ControlLeft"]!, new(Modifier.Control, new[] { Key.Left, Key.Right })),
		new(R["KeyboardPage_ControlUp"]!, new(Modifier.Control, new[] { Key.Up, Key.Down })),
		new(R["KeyboardPage_ControlShiftLeft"]!, new(Modifier.Control | Modifier.Shift, new[] { Key.Left, Key.Right })),
		new(R["KeyboardPage_ControlShiftUp"]!, new(Modifier.Control | Modifier.Shift, new[] { Key.Up, Key.Down }))
	};


	/// <summary>
	/// Initializes an <see cref="KeyboardPage"/> instance.
	/// </summary>
	public KeyboardPage() => InitializeComponent();
}
