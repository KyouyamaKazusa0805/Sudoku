namespace Sudoku.UI.Data.Configuration;

/// <summary>
/// Creates a tuple of data that describes for a preference item.
/// </summary>
/// <param name="Name">The name of the item. The value is the real name that is used for displaying.</param>
/// <param name="Description">The description of the item.</param>
/// <param name="RawName">
/// The raw name of the item. The value is the metadata name of the preference grouping name
/// stored in the type <see cref="PreferenceGroupNames"/>.
/// </param>
/// <param name="SettingItem">The setting item.</param>
/// <param name="OrderingIndex">
/// The ordering index that describes the actual position in a whole preference group.
/// </param>
internal readonly record struct PreferenceItemInfo(string Name, string? Description, string RawName, SettingItem SettingItem, int OrderingIndex);