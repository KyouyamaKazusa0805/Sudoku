namespace Sudoku.UI.Data.Metadata;

/// <summary>
/// Provides an attribute that can be applied to a preference item,
/// indicating the data of the arguments for the preference.
/// </summary>
/// <typeparam name="TSettingItem">The type of the setting item.</typeparam>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class PreferenceAttribute<TSettingItem> : Attribute where TSettingItem : SettingItem
{
	/// <summary>
	/// Initializes a <see cref="PreferenceAttribute{TSettingItem}"/> instance via the specified data.
	/// </summary>
	/// <param name="args">The values.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="args"/> is not of an even length,
	/// or the key in argument <paramref name="args"/> is <see langword="null"/>.
	/// </exception>
	public PreferenceAttribute(params object?[] args)
	{
		if ((args.Length & 1) != 0)
		{
			throw new ArgumentException($"The argument '{nameof(args)}' must be of an even length.", nameof(args));
		}

		var data = new (string, object?)[args.Length >> 1];
		for (int i = 0, j = 0; i < args.Length - 1; i += 2, j++)
		{
			data[j] = (args[i] as string ?? throw new ArgumentException("The value cannot be null."), args[i + 1]);
		}

		Data = data;
	}


	/// <summary>
	/// Indicates the data.
	/// </summary>
	public (string Key, object? Value)[] Data { get; }

	/// <summary>
	/// Indicates the item type.
	/// </summary>
	public Type ItemType => typeof(TSettingItem);
}
