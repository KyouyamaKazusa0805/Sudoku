namespace Sudoku.UI.Data.Configuration;

/// <summary>
/// Provides an internally way to get named value.
/// </summary>
internal static class NamedValueLookup
{
	/// <summary>
	/// Gets attribute arguments of a property via the property name.
	/// </summary>
	/// <typeparam name="TSettingItem">The type of the setting item.</typeparam>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The attribute arguments of a property.</returns>
	/// <exception cref="InvalidOperationException">Throws when the specified property cannot be found.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static PreferenceAttribute<TSettingItem>? GetAttributeArguments<TSettingItem>(string propertyName)
		where TSettingItem : SettingItem
	{
		var pi = typeof(Preference).GetProperty(propertyName) ?? throw new InvalidOperationException();
		return pi.GetCustomAttribute<PreferenceAttribute<TSettingItem>>();
	}

	/// <summary>
	/// Gets the setting preference name of the item via the property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The name of the preference.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string GetItemNameString(string propertyName)
		// Leading underscore characters '_' are intentional.
		// The character suggests the option is reserved by author himself.
		// We should ignore the extra underscores.
		=> R[$"SettingsPage_ItemName_{propertyName.TrimStart('_')}"]!;

	/// <summary>
	/// Gets the setting preference description of the item via the property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The description of the preference.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string? GetItemDescriptionString(string propertyName)
		// Leading underscore characters '_' are intentional.
		// The character suggests the option is reserved by author himself.
		// We should ignore the extra underscores.
		=> R[$"SettingsPage_ItemDescription_{propertyName.TrimStart('_')}"];

	/// <summary>
	/// Gets the named value, or throw an <see cref="InvalidOperationException"/> if none found.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static TNotNull GetNamedValue<TNotNull>(this (string Key, object? Value)[] @this, string key)
		where TNotNull : notnull
		=> (TNotNull?)@this.FirstOrDefault(p => p.Key == key).Value ?? throw new InvalidOperationException("The value is invalid.");

	/// <summary>
	/// Gets the named value, or return <see langword="default"/>(<typeparamref name="TNotNull"/>) if none found.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static TNotNull GetNamedValue<TNotNull>(this (string Key, object? Value)[] @this, string key, TNotNull defaultValue)
		where TNotNull : notnull
		=> (TNotNull?)@this.FirstOrDefault(p => p.Key == key).Value ?? defaultValue;
}
