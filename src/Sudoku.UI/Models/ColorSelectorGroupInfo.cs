namespace Sudoku.UI.Models;

/// <summary>
/// Defines a pair of info that describes for a color selector group.
/// </summary>
public sealed class ColorSelectorGroupInfo : INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the color.
	/// </summary>
	private Color _color;


	/// <summary>
	/// Indicates the current index.
	/// </summary>
	public required int Index { get; init; }

	/// <summary>
	/// Indicates the color.
	/// </summary>
	public required Color Color
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _color;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_color = value;

			SettingItem.SetPreferenceForIndex(Index, _color);

			PropertyChanged?.Invoke(this, new(nameof(Color)));
		}
	}

	/// <summary>
	/// Indicates the setting item used.
	/// </summary>
	public required ColorSelectorGroupSettingItem SettingItem { get; init; }


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;
}
