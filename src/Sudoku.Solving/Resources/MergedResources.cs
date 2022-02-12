using Resource = System.Resources.ResourceManager;

namespace Sudoku.Resources;

/// <summary>
/// Defines a merged resource dictionary.
/// </summary>
internal sealed class MergedResources
{
	/// <summary>
	/// The shared instance.
	/// </summary>
	public static readonly MergedResources R = new();


	/// <summary>
	/// The default manager. If the key cannot be found in the list <see cref="_managers"/>,
	/// this field will be looked up.
	/// </summary>
	private readonly Resource _default;

	/// <summary>
	/// The inner list.
	/// </summary>
	private readonly IDictionary<int, Resource> _managers;

	/// <summary>
	/// Indicates the current LCID.
	/// </summary>
	private int _lcid = 1033;


	/// <summary>
	/// Initializes a <see cref="MergedResources"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private MergedResources()
	{
		_default = Resources1033.ResourceManager;

		(_managers = new Dictionary<int, Resource>()).Add(2052, Resources2052.ResourceManager);
	}


	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public string? this[string key]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _lcid != 1033 && _managers[_lcid].GetString(key) is { } r1 ? r1 : _default.GetString(key);
	}


	/// <summary>
	/// Change the language to the specified one that is represented as the language LCID value.
	/// </summary>
	/// <param name="lcid">The LCID value. The default LCID for the resource dictionary is 1033.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the specified LCID cannot be found in the current resource dictionary.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeLanguage(int lcid) =>
		_lcid = lcid != 1033 && _managers.ContainsKey(lcid)
			? lcid
			: throw new ArgumentException("The specified LCID has already been stored in the current manager.", nameof(lcid));
}
