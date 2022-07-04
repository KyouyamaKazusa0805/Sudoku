namespace Sudoku.UI.Data.Configuration;

internal readonly record struct PreferenceItemInfo(string Name, string? Description, string RawName, SettingItem SettingItem, int OrderingIndex);