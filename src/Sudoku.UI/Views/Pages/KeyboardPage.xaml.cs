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
		new(R["KeyboardPage_ControlO"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.O) }),
		new(R["KeyboardPage_ControlS"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.S) }),
		new(R["KeyboardPage_ControlC"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.C) }),
		new(R["KeyboardPage_ControlV"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.V) }),
		new(R["KeyboardPage_ControlTab"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.Tab) }),
		new(R["KeyboardPage_ControlShiftTab"]!, new List<KeyboardPair>(1) { new(Modifier.Control | Modifier.Shift, Key.Tab) }),
		new(R["KeyboardPage_ControlZ"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.Z) }),
		new(R["KeyboardPage_ControlY"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.Y) }),
		new(R["KeyboardPage_ControlH"]!, new List<KeyboardPair>(1) { new(Modifier.Control, Key.H) }),
		new(R["KeyboardPage_Number0"]!, new List<KeyboardPair>(2)
		{
			new(Modifier.Control, Key.Number0),
			new(Modifier.Control, Key.NumberPad0)
		}),
		new(R["KeyboardPage_OtherNumbers"]!, new List<KeyboardPair>(18)
		{
			new(Modifier.Control, Key.Number1),
			new(Modifier.Control, Key.Number2),
			new(Modifier.Control, Key.Number3),
			new(Modifier.Control, Key.Number4),
			new(Modifier.Control, Key.Number5),
			new(Modifier.Control, Key.Number6),
			new(Modifier.Control, Key.Number7),
			new(Modifier.Control, Key.Number8),
			new(Modifier.Control, Key.Number9),
			new(Modifier.Control, Key.NumberPad1),
			new(Modifier.Control, Key.NumberPad2),
			new(Modifier.Control, Key.NumberPad3),
			new(Modifier.Control, Key.NumberPad4),
			new(Modifier.Control, Key.NumberPad5),
			new(Modifier.Control, Key.NumberPad6),
			new(Modifier.Control, Key.NumberPad7),
			new(Modifier.Control, Key.NumberPad8),
			new(Modifier.Control, Key.NumberPad9)
		}),
		new(R["KeyboardPage_ShiftOtherNumbers"]!, new List<KeyboardPair>(18)
		{
			new(Modifier.Control | Modifier.Shift, Key.Number1),
			new(Modifier.Control | Modifier.Shift, Key.Number2),
			new(Modifier.Control | Modifier.Shift, Key.Number3),
			new(Modifier.Control | Modifier.Shift, Key.Number4),
			new(Modifier.Control | Modifier.Shift, Key.Number5),
			new(Modifier.Control | Modifier.Shift, Key.Number6),
			new(Modifier.Control | Modifier.Shift, Key.Number7),
			new(Modifier.Control | Modifier.Shift, Key.Number8),
			new(Modifier.Control | Modifier.Shift, Key.Number9),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad1),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad2),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad3),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad4),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad5),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad6),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad7),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad8),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad9)
		}),
		new(R["KeyboardPage_ControlNumbers"]!, new List<KeyboardPair>(12)
		{
			new(Modifier.Control, Key.Number1),
			new(Modifier.Control, Key.Number2),
			new(Modifier.Control, Key.Number3),
			new(Modifier.Control, Key.Number4),
			new(Modifier.Control, Key.Number5),
			new(Modifier.Control, Key.Number6),
			new(Modifier.Control, Key.NumberPad1),
			new(Modifier.Control, Key.NumberPad2),
			new(Modifier.Control, Key.NumberPad3),
			new(Modifier.Control, Key.NumberPad4),
			new(Modifier.Control, Key.NumberPad5),
			new(Modifier.Control, Key.NumberPad6),
		}),
		new(R["KeyboardPage_ControlShiftNumbers"]!, new List<KeyboardPair>(8)
		{
			new(Modifier.Control | Modifier.Shift, Key.Number1),
			new(Modifier.Control | Modifier.Shift, Key.Number2),
			new(Modifier.Control | Modifier.Shift, Key.Number3),
			new(Modifier.Control | Modifier.Shift, Key.Number4),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad1),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad2),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad3),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad4)
		}),
		new(R["KeyboardPage_ControlBack"]!, new List<KeyboardPair>(3)
		{
			new(Modifier.Control, Key.Number0),
			new(Modifier.Control, Key.NumberPad0),
			new(Modifier.Control, Key.Back)
		}),
		new(R["KeyboardPage_ControlShiftBack"]!, new List<KeyboardPair>(3)
		{
			new(Modifier.Control | Modifier.Shift, Key.Number0),
			new(Modifier.Control | Modifier.Shift, Key.NumberPad0),
			new(Modifier.Control | Modifier.Shift, Key.Back)
		}),
		new(R["KeyboardPage_ControlLeft"]!, new List<KeyboardPair>(2)
		{
			new(Modifier.Control, Key.Left),
			new(Modifier.Control, Key.Right),
		}),
		new(R["KeyboardPage_ControlUp"]!, new List<KeyboardPair>(2)
		{
			new(Modifier.Control, Key.Up),
			new(Modifier.Control, Key.Down),
		}),
		new(R["KeyboardPage_ControlShiftLeft"]!, new List<KeyboardPair>(2)
		{
			new(Modifier.Control | Modifier.Shift, Key.Left),
			new(Modifier.Control | Modifier.Shift, Key.Right),
		}),
		new(R["KeyboardPage_ControlShiftUp"]!, new List<KeyboardPair>(2)
		{
			new(Modifier.Control | Modifier.Shift, Key.Up),
			new(Modifier.Control | Modifier.Shift, Key.Down),
		})
	};


	/// <summary>
	/// Initializes an <see cref="KeyboardPage"/> instance.
	/// </summary>
	public KeyboardPage() => InitializeComponent();
}
